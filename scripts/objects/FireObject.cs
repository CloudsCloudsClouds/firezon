using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

[GlobalClass]
public partial class FireObject : Area3D
{
    [Export] public PackedScene FireParticles { get; set; }
    [Export] public int HitPoints { get; set; } = 800;
    [Export] public int LifePoints { get; set; } = 1600;
    [Export] public bool IsOnFire { get; set; } = false;

    private GpuParticles3D particles;
    private int stepId = 0;

    public override void _Ready()
    {
        if (IsOnFire)
        {
            Ignite();
        }

        Random random = new Random();
        stepId = random.Next(0, 30);
    }

    public override void _PhysicsProcess(double delta)
    {
        // Increment stepId and process only every 30 frames
        stepId++;
        if (stepId < 30) return;
        stepId = 0;

        if (!IsOnFire)
        {
            // Check overlapping areas only if not on fire
            var overlappingAreas = GetOverlappingAreas();
            foreach (var area in overlappingAreas)
            {
                if (area is FireObject fireObject)
                {
                    HitPoints -= fireObject.Damage();
                    if (HitPoints <= 0)
                    {
                        Ignite();
                        break;
                    }
                }
            }
        }
        else
        {
            // Decrease life points if on fire
            LifePoints -= 30;
            if (LifePoints <= 0)
            {
                Extinguish();
            }
        }
    }

    private void Ignite()
    {
        IsOnFire = true;
        particles = FireParticles.Instantiate<GpuParticles3D>();
        AddChild(particles);
    }

    private async void Extinguish()
    {
        // Hide the object and stop particles
        if (GetNode("grass_leafsLarge2") is Node3D grass)
        {
            grass.Visible = false;
        }
        if (particles != null)
        {
            particles.Emitting = false;
        }

        // Wait for 2 seconds before freeing the object
        await ToSignal(GetTree().CreateTimer(2), "timeout");
        QueueFree();
    }

    public int Damage()
    {
        return IsOnFire ? 30 : 0;
    }
}
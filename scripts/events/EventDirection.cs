using Godot;
using System;

class EventDirection
{
    public Vector3 Direction { get; set; }
    public Vector3 TrueDirection { get; set; } = Vector3.Zero;
    public float Strenght { get; set; }

    public EventDirection(Vector3 dir, float strenght)
    {
        Direction = dir;
        
        if (!Direction.IsNormalized()) 
        {
            Direction = Direction.Normalized();
        }
        Strenght = strenght;
    }

    public EventDirection(Vector3 dir, Vector3 true_dir, float strenght)
    {
        Direction = dir;
        TrueDirection = true_dir;
        if (!Direction.IsNormalized())
            Direction = Direction.Normalized();
        if (!TrueDirection.IsNormalized())
            TrueDirection = TrueDirection.Normalized();
        Strenght = strenght;
    }

    public Vector3 GetDirForce()
    {
        return Direction * Strenght;
    }
}
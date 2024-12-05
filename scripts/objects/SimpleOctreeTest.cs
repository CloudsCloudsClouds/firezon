using Godot;
using System;

public partial class SimpleOctreeTest : Node3D
{
	// Called when the node enters the scene tree for the first time.

	Octree worldOctree;
	public override void _Ready()
	{
		// Create a root node for your game world
		worldOctree = new Octree(Vector3.Zero, 100f, 9);

		// Insert some points
		worldOctree.Insert(new Vector3(1.0f, 2.0f, 3.0f));
		worldOctree.Insert(new Vector3(5.0f, 6.0f, 7.0f));

		// Print the entire tree structure
		worldOctree.PrintTreeStructure();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		worldOctree.DrawTree();
	}
}

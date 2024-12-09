using Godot;
using System;
using System.Collections.Generic;
using GlobalEnums;
using System.Linq;

public partial class FireOctreeTest : Node3D
{
	Octree WorldOctree;
	SpatialOctree rootOctree;
	float LeafStep;
	int counter = 0;
	
	[Export]
	public Vector3[] FireStarts;
	List<Vector3> FireNodes = new List<Vector3>{};
	List<Vector3> BurningNodes = new List<Vector3>{};
	List<Vector3> BurnedNodes = new List<Vector3>{};
	List<Vector3> SoakedNodes = new List<Vector3>{};
	List<Vector3> HealthyNodes = new List<Vector3>{};
	public override void _Ready()
	{
		rootOctree = new SpatialOctree(Vector3.Zero, 100f, 0, 10);

		// Manually subdivide or let point insertion handle it
		rootOctree.Subdivide();

		// Find specific regions and track their state
		var leafNode = rootOctree.FindDeepestNodeContaining(new Vector3(25, 30, 40));
		GD.Print(leafNode == null);
		GD.Print(leafNode.Center, " ", leafNode.IsLeaf, " ", leafNode.Size);
		leafNode.State = CellState.Activated;

		// Retrieve all activated regions
		var activatedRegions = rootOctree.GetLeafNodesByState(CellState.Activated);
		rootOctree.PrintTreeStructure();	
	}

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    /* public override void _PhysicsProcess(double delta)
    {
        counter = Mathf.PosMod(counter + 1, 120);
		
		if (counter == 0)
		{
			HealthyNodes = WorldOctree.GetLeafPositionsByState(CELL_STATE.HEALTHY);
		}

		if (counter == 30)
		{
			FireNodes = WorldOctree.GetLeafPositionsByState(CELL_STATE.BURNING);
			foreach (var pos in FireNodes)
			{
				Vector3[] Dirs = new Vector3[]{
					StepPos(Vector3.Up, pos, LeafStep),
					StepPos(Vector3.Down, pos, LeafStep),
					StepPos(Vector3.Left, pos, LeafStep),
					StepPos(Vector3.Right, pos, LeafStep),
					StepPos(Vector3.Forward, pos, LeafStep),
					StepPos(Vector3.Back, pos, LeafStep),
				};

				foreach (var dir in Dirs)
				{
					WorldOctree.Insert(dir);
					
					if (HealthyNodes.Contains(dir))
					{
						HealthyNodes.Remove(dir);
						WorldOctree.Insert(dir);
						var nod = WorldOctree.FindNodeByPosition(dir);
						nod.State = CELL_STATE.HEATHING;
					}
				}
			}
		}

		if (counter == 60)
		{
			BurningNodes = WorldOctree.GetLeafPositionsByState(CELL_STATE.HEATHING);

			foreach (var dir in BurningNodes)
			{
				BurningNodes.Remove(dir);
				var node = WorldOctree.FindNodeByPosition(dir);
				node.State = CELL_STATE.BURNING;
			}
		}

		if (counter == 90)
		{
			BurnedNodes = WorldOctree.GetLeafPositionsByState(CELL_STATE.BURNED);

			foreach (var dir in FireNodes)
			{
				FireNodes.Remove(dir);
				var node = WorldOctree.FindNodeByPosition(dir);
				node.State = CELL_STATE.BURNED;
			}
		}

		WorldOctree.DebugDrawTree();
    }

	Vector3 StepPos(Vector3 Dir, Vector3 Pos, float step)
	{
		return (Dir * step) + Pos;
	} */

    public override void _PhysicsProcess(double delta)
    {
        rootOctree.DrawTree();
    }
}

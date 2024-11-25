using GlobalEnums;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

public partial class MultigridTest : Node3D
{
	[Export]
	Vector3[] vector3s;
	Multigrid Multigrid { get; set; }
	Dictionary<Vector3, FireCell> FireCells { get; set; }
	int counter = 0;
	public override void _Ready()
	{
		Multigrid = new Multigrid(10, 90);
		foreach (Vector3 v in vector3s)
		{
            var cell_pos = Multigrid.GetOrCreateCell(v, 9);
			GD.Print(cell_pos.Index * cell_pos.Size);
			var randi = new Random();
			var rand = randi.Next(0, 6);
			var cpos = cell_pos.Index * cell_pos.Size;

            FireCells.Add(cpos, new FireCell(cpos, (byte)rand, true, false));
        }

        FireCells.Values.ToList().ForEach(cell => cell.Life = 400);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _PhysicsProcess(double delta)
	{
		Multigrid.DrawAllCells();
		counter = Mathf.PosMod(counter + 1, 60);

		var icount = counter / 10;
		
	}

	private struct FireCell
	{
		public Vector3 Position { get; set; }
		public byte Family { get; set; }
		public int Life { get; set; }
		public bool OnFire { get; set; }
		public bool Burned { get; set; }

		public FireCell(Vector3 position, byte family, bool onFire, bool burned)
		{
			Position = position;
			Family = family;
			OnFire = onFire;
			Burned = burned;
			Life = 300;
		}
	}
}



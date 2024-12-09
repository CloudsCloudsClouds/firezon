using System;
using Godot;

public class SimpleGrid
{
    public Vector3 CellSize { get; set; }

    /// <summary>
    /// Creates a new SimpleGrid with the given cell size.
    /// </summary>
    /// <param name="cellSize">The size of each cell in the grid.</param>
    public SimpleGrid(Vector3 cellSize)
    {
        CellSize = cellSize;
    }

    /// <summary>
    /// Snaps a given position to the nearest cell in the grid.
    /// </summary>
    /// <param name="position">The position to snap to the grid.</param>
    /// <returns>The snapped position.</returns>
    public Vector3 SnapToCell(Vector3 position)
    {
        return new Vector3(
            Mathf.Floor(position.X / CellSize.X) * CellSize.X + CellSize.X / 2,
            Mathf.Floor(position.Y / CellSize.Y) * CellSize.Y + CellSize.Y / 2,
            Mathf.Floor(position.Z / CellSize.Z) * CellSize.Z + CellSize.Z / 2
        );
    }
}
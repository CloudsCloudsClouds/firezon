using Godot;
using System.Collections.Generic;

class Multigrid
{
    private int levels; // Number of grid levels
    private float rootCellSize; // Size of the coarsest grid cell
    private Dictionary<Vector3, GridCell>[] grids; // Array of dictionaries for each level

    public Multigrid(int levels, float rootCellSize)
    {
        this.levels = levels;
        this.rootCellSize = rootCellSize;
        grids = new Dictionary<Vector3, GridCell>[levels];

        for (int i = 0; i < levels; i++)
            grids[i] = new Dictionary<Vector3, GridCell>();
    }

    // Adds or retrieves a cell at the specified position and level
    public GridCell GetOrCreateCell(Vector3 position, int level)
    {
        Vector3 cellIndex = GetCellIndex(position, level);
        if (!grids[level].ContainsKey(cellIndex))
            grids[level][cellIndex] = new GridCell(level, cellIndex, GetCellSize(level));

        return grids[level][cellIndex];
    }

    // Gets the index of the cell containing the position at the given level
    private Vector3 GetCellIndex(Vector3 position, int level)
    {
        float cellSize = GetCellSize(level);
        return new Vector3(
            Mathf.FloorToInt(position.X / cellSize),
            Mathf.FloorToInt(position.Y / cellSize),
            Mathf.FloorToInt(position.Z / cellSize)
        );
    }

    // Gets the size of a cell at a specific level
    private float GetCellSize(int level)
    {
        return rootCellSize / Mathf.Pow(2, level);
    }
}

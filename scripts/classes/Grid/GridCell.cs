using Godot;
using System;
using System.Collections.Generic;
class GridCell
{
    public int Level { get; private set; } // The level of this cell
    public Vector3 Index { get; private set; } // Index of the cell at its level
    public float Size { get; private set; } // Physical size of the cell
    private Dictionary<Vector3, GridCell> finerGrid; // Optional finer grid for subdivisions

    public GridCell(int level, Vector3 index, float size)
    {
        Level = level;
        Index = index;
        Size = size;
        finerGrid = null; // Only created when necessary
    }

    // Subdivide this cell into a finer grid
    public void Subdivide()
    {
        if (finerGrid == null)
            finerGrid = new Dictionary<Vector3, GridCell>();
    }

    // Get or create a finer cell
    public GridCell GetOrCreateFinerCell(Vector3 position, float finerCellSize)
    {
        if (finerGrid == null)
            Subdivide();

        Vector3 localPos = position - (Vector3)Index * Size; // Offset within this cell
        Vector3 finerIndex = new Vector3(
            Mathf.FloorToInt(localPos.X / finerCellSize),
            Mathf.FloorToInt(localPos.Y / finerCellSize),
            Mathf.FloorToInt(localPos.Z / finerCellSize)
        );

        if (!finerGrid.ContainsKey(finerIndex))
            finerGrid[finerIndex] = new GridCell(Level + 1, finerIndex, finerCellSize);

        return finerGrid[finerIndex];
    }
}

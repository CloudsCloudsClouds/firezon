using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot;

namespace ProyectoFirezon.scripts.classes.Grid
{
    internal class SimpleGrid
    {
        private Dictionary<Vector3, Cell> cells = new Dictionary<Vector3, Cell>();
        private Dictionary<Cell, IGridObject[]> objects = new Dictionary<Cell, IGridObject[]>();
        private float cellSize;

        /// <summary>
        /// Initializes the SimpleGrid with the given cell size.
        /// </summary>
        /// <param name="size">The size of each grid cell.</param>
        public SimpleGrid(float size)
        {
            cellSize = size;
        }

        /// <summary>
        /// Transforms a world position into a grid cell index based on the cell size.
        /// </summary>
        /// <param name="position">The world position to be transformed into a grid cell index.</param>
        /// <returns>The grid index corresponding to the position.</returns>
        public Vector3 TransformGrid(Vector3 position)
        {
            Vector3 index = new Vector3(
                Mathf.FloorToInt(position.X / cellSize),
                Mathf.FloorToInt(position.Y / cellSize),
                Mathf.FloorToInt(position.Z / cellSize)
            );

            return index;
        }

        /// <summary>
        /// Gets or creates a cell at the specified world position.
        /// </summary>
        /// <param name="position">The world position of the cell.</param>
        /// <returns>The cell at the transformed grid position.</returns>
        public Cell GetOrCreateCell(Vector3 position)
        {
            Vector3 index = TransformGrid(position);

            if (!cells.ContainsKey(index))
                cells[index] = new Cell(index * cellSize, cellSize);

            return cells[index];
        }

        /// <summary>
        /// Deletes cells from the grid that do not contain any objects.
        /// </summary>
        public void DeleteEmpty()
        {
            var cellsNotInObjects = cells.Values.Where(cell => !objects.ContainsKey(cell)).ToList();
            foreach (var cell in cellsNotInObjects)
            {
                var rkey = cells.FirstOrDefault(pair => pair.Value == cell).Key;
                cells.Remove(rkey);
            }
        }

        /// <summary>
        /// Gets the cell at the specified grid index.
        /// </summary>
        /// <param name="index">The index of the cell.</param>
        /// <returns>The cell at the specified index, or null if the cell does not exist.</returns>
        public Cell GetCellAt(Vector3 index)
        {
            if (cells.ContainsKey(index))
            {
                return cells[index];
            }
            return null;
        }

        /// <summary>
        /// Adds an object to the grid based on its AABB (Axis-Aligned Bounding Box).
        /// </summary>
        /// <param name="obj">The object to add to the grid.</param>
        public void AddObject(ref IGridObject obj)
        {
            // Get the AABB of the object
            if (obj == null) return;
            Aabb oaabb = obj.Bounds;

            var points = CollidingObjects(oaabb, obj.Position);

            foreach (var point in points)
            {
                objects[point].Append(obj);
            }
        }

        /// <summary>
        /// Determines which cells the AABB of an object collides with in the grid.
        /// </summary>
        /// <param name="box">The AABB of the object.</param>
        /// <param name="pos">The position of the object in the world.</param>
        /// <returns>A list of cells that the object collides with.</returns>
        public List<Cell> CollidingObjects(Aabb box, Vector3 pos)
        {
            List<Cell> list = new List<Cell>();
            Vector3 minIndex = pos / cellSize;
            Vector3 maxIndex = (pos + box.Size) / cellSize;

            // Iterate over the range of grid indices the object spans
            for (int x = Mathf.FloorToInt(minIndex.X); x <= Mathf.FloorToInt(maxIndex.X); x++)
            {
                for (int y = Mathf.FloorToInt(minIndex.Y); y <= Mathf.FloorToInt(maxIndex.Y); y++)
                {
                    for (int z = Mathf.FloorToInt(minIndex.Z); z <= Mathf.FloorToInt(maxIndex.Z); z++)
                    {
                        // Create a cell reference from the indices (could be a position or index key)
                        Vector3 cellIndex = new Vector3(x, y, z);
                        Cell cell = GetCellAt(cellIndex); // Assuming this method returns the cell at the given index

                        // Add the object to the cell if it's not already there
                        if (cell != null)
                        {
                            list.Add(cell);
                        }
                    }
                }
            }

            return list;
        }

        /// <summary>
        /// Updates the grid, checking whether objects have moved to new cells and should be reallocated.
        /// </summary>
        /// <param name="family">A byte identifier for the objects to be updated (optional, for grouping objects).</param>
        public void Update(byte family)
        {
            // Create a list to store keys to remove
            List<Cell> keysToRemove = new List<Cell>();

            // Iterate over all the cells in the objects dictionary
            foreach (var kvp in objects.Where(cell => cell.Key.family == family))
            {
                Cell cell = kvp.Key;
                List<IGridObject> cellObjects = kvp.Value.ToList();

                // Create a new list to track objects to remove or update
                List<IGridObject> objectsToUpdate = new List<IGridObject>();

                foreach (IGridObject obj in cellObjects.ToList())
                {
                    if (obj == null)
                    {
                        cellObjects.Remove(obj);
                    }
                    else
                    {
                        Vector3 newCellIndex = GetCellIndex(obj.Bounds.Position);

                        // If the object moves to a different cell
                        if (!cell.Position.Equals(newCellIndex))
                        {
                            // Remove the object from the current cell's list
                            cellObjects.Remove(obj);

                            // Add the object to the new cell
                            var newCell = GetOrCreateCell(newCellIndex);
                            var newCellObjects = objects.ContainsKey(newCell) ? objects[newCell].ToList() : new List<IGridObject>();
                            newCellObjects.Add(obj);
                            objects[newCell] = newCellObjects.ToArray();
                        }
                    }

                    // If the cell is empty, mark the key for removal
                    if (cellObjects.Count == 0)
                    {
                        keysToRemove.Add(cell);
                    }
                }
            }

            // Remove any cells that no longer have objects in them
            foreach (var key in keysToRemove)
            {
                objects.Remove(key);
            }
        }

        /// <summary>
        /// Retrieves the grid index corresponding to a world position.
        /// </summary>
        /// <param name="position">The world position to get the grid index for.</param>
        /// <returns>The grid index corresponding to the position.</returns>
        private Vector3 GetCellIndex(Vector3 position)
        {
            return new Vector3(
                Mathf.FloorToInt(position.X / cellSize),
                Mathf.FloorToInt(position.Y / cellSize),
                Mathf.FloorToInt(position.Z / cellSize)
            );
        }

        /// <summary>
        /// Checks if the specified object is contained within the cell at the given position.
        /// </summary>
        /// <param name="obj">The object to check for.</param>
        /// <param name="pos">The world position of the cell to check.</param>
        /// <returns>True if the object is contained within the cell at the given position; otherwise, false.</returns>
        public bool CellContainsObject(ref IGridObject obj, Vector3 pos)
        {
            if (obj == null) return false;

            var npos = TransformGrid(pos); // Ensure TransformGrid is doing what's expected
            var cell = GetCellAt(npos);
            if (cell == null) return false;

            // Check if the cell contains the object (make sure objects dictionary contains the key)
            if (objects.ContainsKey(cell) && objects[cell].Contains(obj))
            {
                return true;
            }

            return false;
        }
    }
}

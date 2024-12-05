using Godot;
using System;
using GlobalEnums;
using System.Collections.Generic;
using System.Linq;
public class Octree 
{
    Vector3 Position { set; get; }
    float Size { set; get; }
    CELL_STATE State { set; get; } = CELL_STATE.HEALTHY;
    int Depth { set; get; }

    private Octree[] Children { get; set; }
    public bool HasChildren => Children != null;

    bool isLeaf;

    public Octree(Vector3 pos, float size, int res)
    {
        Position = pos;
        Size = size;
        Depth = res;
        
        isLeaf = res == 0 ? true : false;
        Children = null;
    }


    public void Subdivide()
    {
        if (isLeaf)
            return;
        
        float halfSize = Size / 2;
        Children = new Octree[8];

        Vector3[] OctantOffset = new Vector3[]
        {
            new Vector3(0, 0, 0),           // Front Bottom Left
            new Vector3(halfSize, 0, 0),    // Front Bottom Right
            new Vector3(0, halfSize, 0),    // Front Top Left
            new Vector3(halfSize, halfSize, 0), // Front Top Right
            new Vector3(0, 0, halfSize),    // Back Bottom Left
            new Vector3(halfSize, 0, halfSize), // Back Bottom Right
            new Vector3(0, halfSize, halfSize), // Back Top Left
            new Vector3(halfSize, halfSize, halfSize) // Back Top Right
        };

        for (int i = 0; i < 8; i++)
        {
            Vector3 childPosition = Position + OctantOffset[i];
            int lessRes = Depth - 1;
            Children[i] = new Octree(childPosition, halfSize, lessRes);
        }
    }


    // Method to add an object or mark a region
    public void Insert(Vector3 point)
    {
        // If point is not in this node, do nothing
        if (!ContainsPoint(point))
            return;

        // If no children, try to subdivide
        if (!HasChildren)
        {
            Subdivide();
        }

        // Try to insert into child nodes
        if (HasChildren)
        {
            foreach (var child in Children)
            {
                child.Insert(point);
            }
        }
    }

    public List<Vector3> GetLeafPositionByState(List<Vector3> OGList, CELL_STATE state)
    {
        if (HasChildren)
        {
            foreach (var child in Children)
            {
                if (child.isLeaf)
                {
                    if (child.State == state)
                        OGList.Append(child.Position);
                }
                else
                {
                    OGList = child.GetLeafPositionByState(OGList, state);
                }
            }
        }

        return OGList;
    }

    public float GetLeafSize()
    {
        float lsize = (float) (Size / Math.Pow(2, Depth));
        return lsize;
    }

    public bool ContainsPoint(Vector3 point)
    {
        return point.X >= Position.X && point.X < Position.X + Size &&
               point.Y >= Position.Y && point.Y < Position.Y + Size &&
               point.Z >= Position.Z && point.Z < Position.Z + Size;
    }


    public void PrintTreeStructure(int depth = 0)
    {
        // Indent based on depth for visual hierarchy
        string indent = new string(' ', depth * 2);
        GD.Print($"{indent} Node at {Position}, Size: {Size}, State: {State}");

        if (HasChildren)
        {
            foreach (var child in Children)
            {
                child.PrintTreeStructure(depth + 1);
            }
        }
    }

    public void DrawTree()
    {
        DrawNode();

        if (HasChildren)
        {
            foreach (var child in Children)
            {
                child.DrawTree();
            }
        }
    }

    private void DrawNode()
    {
        var col = GetColorByDepth(Depth);
        var size = new Vector3(Size, Size, Size);
        DebugDraw3D.DrawBox(Position, Quaternion.Identity, size, col, true);
    }

    private static Color GetColorByDepth(int depth)
    {
        float hue = (depth * 0.1f) % 1.0f;
        return Color.FromHsv(hue, 0.8f, 0.8f, 1.0f);
    }
}
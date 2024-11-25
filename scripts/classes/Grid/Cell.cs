using System;
using Godot;

public class Cell
{
    public Vector3 Position { get; private set; }
    public float Size { get; private set; }
    public byte family;

    public Cell(Vector3 position, float size)
    {
        Position = position;
        Size = size;
        var rand = new Random();
        family = (byte)rand.Next(0, 6);
    }
}

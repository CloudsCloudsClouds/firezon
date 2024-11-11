using Godot;

public class EventFacingDirection
{
    public Vector3 Direction { get; set; }
    public Vector2 Velocity { get; set; }

    public EventFacingDirection(Vector3 dir, Vector2 vel)
    {
        Direction = dir;
        Velocity = vel;
        if (!Direction.IsNormalized())
            Direction = Direction.Normalized();
    }
}
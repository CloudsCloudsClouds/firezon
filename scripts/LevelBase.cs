using Godot;

partial class LevelBase : Node3D
{
    public override void _PhysicsProcess(double delta)
    {
        if (Input.IsActionJustPressed("ui_fullscreen"))
        {
            if (DisplayServer.WindowGetMode() == DisplayServer.WindowMode.Fullscreen)
                DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
            else
                DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
        }
    }
}
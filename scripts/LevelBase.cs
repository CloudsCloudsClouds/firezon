using Godot;

partial class LevelBase : Node3D
{
    public override void _Ready()
    {
        DebugDraw2D.CreateFpsGraph("FPS");
    }


    public override void _PhysicsProcess(double delta)
    {
        if (Input.IsActionJustPressed("ui_fullscreen"))
        {
            if (DisplayServer.WindowGetMode() == DisplayServer.WindowMode.Fullscreen)
                DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
            else
                DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
        }

        if (Input.IsActionJustPressed("ui_cancel"))
        {
            if (Input.GetMouseMode() == Input.MouseModeEnum.Captured)
                Input.SetMouseMode(Input.MouseModeEnum.Visible);
            else
                Input.SetMouseMode(Input.MouseModeEnum.Captured);
        }
    }
}
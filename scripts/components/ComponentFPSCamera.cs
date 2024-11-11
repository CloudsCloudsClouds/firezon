using System;
using System.Linq;
using GlobalEnums;
using Godot;

[Tool]
[GlobalClass]
public partial class ComponentFPSCamera : IComponent
{
    public override WorldEntity Entity { get; set; }
    [Export]
    public override TYPE_OF_UPDATE TypeOfUpdate { get; set; }
    double delta;
    Vector2 MouseSpeed;
    Camera3D MyCamera;
    private Vector2 CameraVelocity = Vector2.Zero;

    [Export]
    public float CameraSensitivity = 0.05f;
    [Export]
    public bool HeadTilt = false;
    [Export]
    public float HeadTiltAmmount = 1f;
    float x_input = 0;

    public override void _Input(InputEvent @event)
    {
        if(!Engine.IsEditorHint())
        {
            if (@event is InputEventMouseMotion && Input.GetMouseMode() == Input.MouseModeEnum.Captured)
                HandleCameraMovement((InputEventMouseMotion)@event);
        }
    }

    public override void Init()
    {
        MyCamera = GetChildren().OfType<Camera3D>().First();
        SetPhysicsProcess(false);
        Entity.EventBus.Subscribe<EventDirection>(event_DirectionInput);
    }

    public override void Update(double delta)
    {
        this.delta = delta;
        EventFacingDirection new_direction = new EventFacingDirection(MyCamera.GetRotation(), CameraVelocity);
        Entity.EventBus.Publish<EventFacingDirection>(new_direction);
    }

    public override string[] _GetConfigurationWarnings()
    {
        var children = GetChildren().OfType<Camera3D>().ToList();
        string[] warning = Array.Empty<string>();

        if (children.Count == 0)
        {
            warning = new string[] { "No Camera3D found as children \nAdd a Camera3D as a children of this node" };
        }

        return warning;
    }


    void HandleCameraMovement(InputEventMouseMotion @event)
	{
		RotateY(Mathf.DegToRad(-@event.Relative.X * CameraSensitivity));
		MyCamera.RotateX(Mathf.DegToRad(-@event.Relative.Y * CameraSensitivity));

        CameraVelocity.X = -@event.Relative.X * CameraSensitivity;
		CameraVelocity.Y = -@event.Relative.Y * CameraSensitivity;

		var newRotation = MyCamera.Rotation;
		newRotation.X = Mathf.Clamp(MyCamera.Rotation.X, Mathf.DegToRad(-89), Mathf.DegToRad(89));
        newRotation.Y = 0;
		MyCamera.Rotation = newRotation;
	}

    void event_DirectionInput(EventDirection @event)
    {
        if (HeadTilt)
		{
            var new_rotation = MyCamera.Rotation;
            x_input = @event.TrueDirection.X;
            var multiplier = (float)Mathf.Clamp(Mathf.Remap(Math.Abs(MyCamera.RotationDegrees.X), 0, 85, 1.5, 0), 0, 1.5f);
            
            new_rotation.Z = Mathf.Lerp(new_rotation.Z, -x_input * HeadTiltAmmount * multiplier, 10f * (float)delta);
            MyCamera.Rotation = new_rotation;
		}
    }
}

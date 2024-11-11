using System;
using System.Linq;
using GlobalEnums;
using Godot;

[Tool]
[GlobalClass]
public partial class ComponentFPSCamera : IComponent
{
    public override WorldEntity Entity { get; set; }
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
    public bool weaponTilt = false;

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseMotion && Input.GetMouseMode() == Input.MouseModeEnum.Captured)
            HandleCameraMovement((InputEventMouseMotion)@event);
    }

    public override void Init()
    {
        MyCamera = GetChildren().OfType<Camera3D>().First();
        SetPhysicsProcess(false);
        SetProcessMode(ProcessModeEnum.Disabled);
    }

    public override void Update(double delta)
    {
        
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

		if (weaponTilt)
		{
			CameraVelocity.X = -@event.Relative.X * CameraSensitivity;
			CameraVelocity.Y = -@event.Relative.Y * CameraSensitivity;

            // TODO Weapon Tilt
		}

		var newRotation = MyCamera.Rotation;
		newRotation.X = Mathf.Clamp(MyCamera.Rotation.X, Mathf.DegToRad(-89), Mathf.DegToRad(89));
		MyCamera.Rotation = newRotation;
	}
}

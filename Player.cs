using Godot;

namespace SimpleMirrorDemo;

public partial class Player : CharacterBody3D
{
    [Export] public float Gravity { get; set; } = 9.8f;
    [Export] public float Speed { get; set; } = 4f;
    [Export] public float RunSpeed { get; set; } = 8f;
    [Export] public float RotationSpeed { get; set; } = 0.001f;

    private Vector3 _targetVelocity = Vector3.Zero;
    private Node3D _rotationHelper;

    public override void _Ready()
    {
        _rotationHelper = FindChild("RotationHelper") as Node3D;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (Input.IsKeyPressed(Key.Escape))
        {
            Input.MouseMode = Input.MouseModeEnum.Visible;
        }

        if (Input.IsMouseButtonPressed(MouseButton.Left))
        {
            Input.MouseMode = Input.MouseModeEnum.Captured;
        }

        _targetVelocity.Z = 0;
        _targetVelocity.X = 0;

        var direction = Vector3.Zero;
        if (Input.IsActionPressed("move_forward"))
        {
            direction = Vector3.Forward;
        }
        else if (Input.IsActionPressed("move_back"))
        {
            direction = Vector3.Back;
        }

        if (Input.IsActionPressed("move_left"))
        {
            direction = Vector3.Left;
        }
        else if (Input.IsActionPressed("move_right"))
        {
            direction = Vector3.Right;
        }

        direction = direction.Rotated(Vector3.Up, Rotation.Y);
        direction = direction.Normalized();

        var speed = Input.IsActionPressed("run") ? RunSpeed : Speed;

        _targetVelocity.X = direction.X * speed;
        _targetVelocity.Z = direction.Z * speed;

        if (!IsOnFloor())
        {
            _targetVelocity.Y -= Gravity * (float)delta;
        } else if (Input.IsActionPressed("jump"))
        {
            _targetVelocity.Y = 5f;
        }

        Velocity = _targetVelocity;
        MoveAndSlide();
    }

    public override void _Input(InputEvent e)
    {
        if (Input.MouseMode != Input.MouseModeEnum.Captured)
        {
            return;
        }

        if (e is InputEventMouseMotion inputEvent)
        {
            _rotationHelper.RotateX(-inputEvent.Relative.Y * RotationSpeed);

            // dead area
            _rotationHelper.Rotation = _rotationHelper.Rotation.X switch
            {
                > Mathf.Pi / 4 => new Vector3(Mathf.Pi / 4, _rotationHelper.Rotation.Y, _rotationHelper.Rotation.Z),
                < -Mathf.Pi / 4 => new Vector3(-Mathf.Pi / 4, _rotationHelper.Rotation.Y, _rotationHelper.Rotation.Z),
                _ => _rotationHelper.Rotation
            };

            RotateY(-inputEvent.Relative.X * RotationSpeed);
        }
    }
}
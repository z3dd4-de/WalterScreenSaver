using Godot;
using System;

public partial class DvdSprite : Sprite2D
{
    [Export] public float Speed = 300f;

    [Export] public Sprite2D Background1;
    [Export] public Sprite2D Background2;

    [Export] public float AngleStepDegrees = 5f; // wie stark Pfeiltasten wirken
    [Export] public RichTextLabel PauseLabel;
    private bool pauseLabelVisible = false;

    private Vector2 velocity;
    private Vector2 screenSize;
    private Vector2 spriteSize;

    private bool isFullscreen = true;

    public override void _Ready()
    {
        screenSize = GetViewportRect().Size;
        spriteSize = Texture.GetSize() * Scale;

        float angle = (float)GD.RandRange(0.4f, 0.6f) * Mathf.Pi;
        float dirX = GD.Randf() < 0.5f ? -1f : 1f;

        velocity = new Vector2(
            Mathf.Cos(angle) * dirX,
            Mathf.Sin(angle)
        ).Normalized() * Speed;

        SetFullscreen(true);
        SetBackground(1);
        UpdateOrientation();
    }

    private void TogglePauseLabel()
    {
        pauseLabelVisible = !pauseLabelVisible;

        if (PauseLabel != null)
            PauseLabel.Visible = pauseLabelVisible;
    }

    private void AdjustAngle(float degrees)
    {
        float speed = velocity.Length();

        // aktueller Winkel
        float angle = Mathf.Atan2(velocity.Y, velocity.X);

        // Winkel ändern
        angle += Mathf.DegToRad(degrees);

        // neue Geschwindigkeit setzen
        velocity = new Vector2(
            Mathf.Cos(angle),
            Mathf.Sin(angle)
        ).Normalized() * speed;

        UpdateOrientation();
    }


    public override void _Process(double delta)
    {
        // Winkel anpassen
        if (Input.IsKeyPressed(Key.Up))
            AdjustAngle(-AngleStepDegrees);

        if (Input.IsKeyPressed(Key.Down))
            AdjustAngle(AngleStepDegrees);

        // ESC → beenden
        if (Input.IsActionJustPressed("ui_cancel"))
            GetTree().Quit();

        // Hintergrund wechseln
        if (Input.IsKeyPressed(Key.Key1)) SetBackground(1);
        if (Input.IsKeyPressed(Key.Key2)) SetBackground(2);

        // Fenster-Modus wechseln (Input Map)
        if (Input.IsActionJustPressed("fullscreen"))
            SetFullscreen(true);

        if (Input.IsActionJustPressed("windowed"))
            SetFullscreen(false);

        // "Pause"-Label ein / ausblenden
        if (Input.IsActionJustPressed("show_pause"))
            TogglePauseLabel();

        MoveSprite(delta);
    }


    // =======================
    // Fenster / Vollbild
    // =======================
    private void SetFullscreen(bool fullscreen)
    {
        if (isFullscreen == fullscreen)
            return;

        isFullscreen = fullscreen;

        if (fullscreen)
        {
            DisplayServer.WindowSetMode(
                DisplayServer.WindowMode.Fullscreen
            );
        }
        else
        {
            DisplayServer.WindowSetMode(
                DisplayServer.WindowMode.Windowed
            );

            DisplayServer.WindowSetSize(new Vector2I(1280, 720));
        }

        screenSize = GetViewportRect().Size;
    }



    public override void _Notification(int what)
    {
        if (what == NotificationWMSizeChanged)
        {
            screenSize = GetViewportRect().Size;
        }
    }


    // =======================
    // Bewegung
    // =======================
    private void MoveSprite(double delta)
    {
        Vector2 position = Position;
        position += velocity * (float)delta;

        if (position.X <= spriteSize.X / 2f)
        {
            position.X = spriteSize.X / 2f;
            velocity.X = Math.Abs(velocity.X);
            UpdateOrientation();
        }

        if (position.X >= screenSize.X - spriteSize.X / 2f)
        {
            position.X = screenSize.X - spriteSize.X / 2f;
            velocity.X = -Math.Abs(velocity.X);
            UpdateOrientation();
        }

        if (position.Y <= spriteSize.Y / 2f)
        {
            position.Y = spriteSize.Y / 2f;
            velocity.Y = Math.Abs(velocity.Y);
        }

        if (position.Y >= screenSize.Y - spriteSize.Y / 2f)
        {
            position.Y = screenSize.Y - spriteSize.Y / 2f;
            velocity.Y = -Math.Abs(velocity.Y);
        }

        Position = position;
    }

    private void UpdateOrientation()
    {
        FlipH = velocity.X < 0f;
    }

    private void SetBackground(int index)
    {
        if (Background1 != null)
            Background1.Visible = index == 1;

        if (Background2 != null)
            Background2.Visible = index == 2;
    }
}

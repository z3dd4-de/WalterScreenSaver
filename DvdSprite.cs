using Godot;
using System;

public partial class DvdSprite : Sprite2D
{
    [Export]
    public float Speed = 300f;

    private Vector2 velocity;
    private Vector2 screenSize;
    private Vector2 spriteSize;

    public override void _Ready()
    {
        screenSize = GetViewportRect().Size;
        spriteSize = Texture.GetSize() * Scale;

        // Zufällige diagonale Startbewegung
        float angle = (float)GD.RandRange(0.25f, 0.75f) * Mathf.Pi;

        // Zufällige Start-Richtung links / rechts
        float dirX = GD.Randf() < 0.5f ? -1f : 1f;

        velocity = new Vector2(
            Mathf.Cos(angle) * dirX,
            Mathf.Sin(angle)
        ).Normalized() * Speed;

        UpdateOrientation();
    }

    public override void _Process(double delta)
    {
        // ESC → ScreenSaver beenden
        if (Input.IsActionJustPressed("ui_cancel"))
        {
            GetTree().Quit();
        }

        Vector2 position = Position;
        position += velocity * (float)delta;

        // Linker Rand → nach rechts reflektieren + Orientierung normal
        if (position.X <= spriteSize.X / 2f)
        {
            position.X = spriteSize.X / 2f;
            velocity.X = Math.Abs(velocity.X);
            UpdateOrientation();
        }

        // Rechter Rand → nach links reflektieren + Orientierung gespiegelt
        if (position.X >= screenSize.X - spriteSize.X / 2f)
        {
            position.X = screenSize.X - spriteSize.X / 2f;
            velocity.X = -Math.Abs(velocity.X);
            UpdateOrientation();
        }

        // Oberer Rand (keine Orientierung)
        if (position.Y <= spriteSize.Y / 2f)
        {
            position.Y = spriteSize.Y / 2f;
            velocity.Y = Math.Abs(velocity.Y);
        }

        // Unterer Rand (keine Orientierung)
        if (position.Y >= screenSize.Y - spriteSize.Y / 2f)
        {
            position.Y = screenSize.Y - spriteSize.Y / 2f;
            velocity.Y = -Math.Abs(velocity.Y);
        }

        Position = position;
    }

    private void UpdateOrientation()
    {
        // Kopf zeigt nach rechts → FlipH = false
        // Kopf zeigt nach links → FlipH = true
        FlipH = velocity.X < 0f;
    }
}

using Godot;
using System;
using System.Numerics;

public partial class ConsoleWindow : Window
{
    [Export] ConsoleViewport viewport;
    [Export] Control sizeControl;

    Vector2I posBuffer;

    Vector2I popSizeMem;
    Vector2I popPosMem;

    Vector2I embSizeMem;
    Vector2I embPosMem;

    public override void _Ready()
    {
        base._Ready();
        CloseRequested += Close;
        SizeChanged += ClampSize;
    }

    public void RestrictPos()
    {
        if (viewport.GuiEmbedSubwindows)
        {
            Vector2I mainWinSize = new((int)sizeControl.Size.X, (int)sizeControl.Size.Y);
            int titleHeight = GetThemeConstant("title_height");

            Position = new(
                Mathf.Clamp(Position.X, -Size.X + titleHeight, mainWinSize.X - titleHeight),
                Mathf.Clamp(Position.Y, titleHeight, mainWinSize.Y)
            );
        }
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("dev_console"))
        {
            Visible = !Visible;
        }

        if (posBuffer != Position)
        {
            RestrictPos();
            posBuffer = Position;
        }
    }

    private void Close()
    {
        if (!viewport.GuiEmbedSubwindows)
        {
            viewport.Pop();
        }
        else
        {
            Visible = false;
        }
    }

    public void ClampSize()
    {
        if (viewport.GuiEmbedSubwindows)
        {
            MaxSize = new((int)sizeControl.Size.X, (int)sizeControl.Size.Y);
        }
        else
        {
            MaxSize = new(16384, 16384);
        }
    }

    public void SetTransMem()
    {
        if (viewport.GuiEmbedSubwindows)
        {
            embPosMem = Position;
            embSizeMem = Size;
        }
        else
        {
            popPosMem = Position;
            popSizeMem = Size;
        }
    }

    public void RecallTransMem()
    {
        if (viewport.GuiEmbedSubwindows)
        {
            if (embSizeMem != Vector2I.Zero)
            {
                Position = embPosMem;
                Size = embSizeMem; 
            }
        }
        else
        {
            if (popSizeMem != Vector2I.Zero)
            {
                Position = popPosMem;
                Size = popSizeMem;
            }
        } 
    }
}

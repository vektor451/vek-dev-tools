using Godot;
using System;

public partial class ConsoleWindow : Window
{
    [Export] ConsoleViewport viewport;
    [Export] Control sizeControl;

    Vector2I posBuffer;

    Vector2I popSizeMem;
    Vector2I popPosMem;

    Vector2I embSizeMem;
    Vector2I embPosMem;

    bool isResizing;
    Vector2I sizeBuffer;

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
            if (Visible)
            {
                SetTransMem();
                Visible = false;
            }
            else
            {
                Visible = true;
                RecallTransMem();
            }
        }

        if (posBuffer != Position)
        {
            RestrictPos();
            posBuffer = Position;
        }

        UpdateMouseFilter();
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

    public void UpdateMouseFilter()
    {
        Vector2 mousePos = GetMousePosition();
        float margin = (float)Theme.Get("Window/constants/resize_margin");

        if (Input.IsMouseButtonPressed(MouseButton.Left))
        {
            if (sizeBuffer != Size)
            {
                isResizing = true;
                sizeBuffer = Size;
            }
        }
        else
        {
            isResizing = false;
        }

        if (mousePos > new Vector2(-margin, -margin) && mousePos < new Vector2(Size.X + margin, Size.Y + margin))
        {
            sizeControl.MouseFilter = Control.MouseFilterEnum.Pass;
        }
        else if (!isResizing)
        {
            sizeControl.MouseFilter = Control.MouseFilterEnum.Ignore;
        }
    }
}

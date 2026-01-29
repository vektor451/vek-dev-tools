using Godot;
using System;

public partial class ConsoleWindow : Window
{
    [Export] ConsoleViewport viewport;
    [Export] Control sizeControl;
    [Export] bool UseViewportLogic;

    Vector2I posBuffer;

    Vector2I popSizeMem;
    Vector2I popPosMem;

    Vector2I embSizeMem;
    Vector2I embPosMem;

    bool isResizing;
    Vector2I sizeBuffer;

    public async override void _Ready()
    {
        base._Ready();
        CloseRequested += Close;
        SizeChanged += ClampSize;
        Visible = false;
        DevConsole.Active = false;
        Size = (Vector2I)ProjectSettings.GetSetting("dev_tools/config/console_window_size", Size);
    }

    public void RestrictPos()
    {
        if (viewport != null && viewport.GuiEmbedSubwindows)
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
                DevConsole.Active = false;
            }
            else
            {
                Visible = true;
                RecallTransMem();
                DevConsole.Active = true;
            }
        }

        if (posBuffer != Position)
        {
            RestrictPos();
            posBuffer = Position;
        }

        if(viewport != null)
        {
            UpdateMouseFilter();
        }
    }

    private void Close()
    {
        if (viewport != null && !viewport.GuiEmbedSubwindows)
        {
            viewport.Pop();
        }
        else
        {
            Visible = false;
            DevConsole.Active = false;
        }
    }

    public void ClampSize()
    {
        if (viewport == null || viewport.GuiEmbedSubwindows)
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
        if (viewport == null || viewport.GuiEmbedSubwindows)
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
        if (viewport == null || viewport.GuiEmbedSubwindows)
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

        if (mousePos > new Vector2(-margin, -margin) && mousePos < new Vector2(Size.X + margin, Size.Y + margin) || Visible)
        {
            sizeControl.MouseFilter = Control.MouseFilterEnum.Pass;
        }
        else if (!isResizing)
        {
            sizeControl.MouseFilter = Control.MouseFilterEnum.Ignore;
        }
    }
}

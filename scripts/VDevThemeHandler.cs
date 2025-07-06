using Godot;
using System;
using System.Collections.Generic;

public partial class VDevThemeHandler : Node
{
    [Export] VDevTheme theme;
    [Export] Control sizeControl;

    Vector2I winSizeBuffer;

    public override void _Ready()
    {
        DevConsole.Print(GetTree().Root.ContentScaleSize.ToString());

        theme.scaleFactor = Mathf.Clamp(sizeControl.Size.X / GetTree().Root.Size.X, 1, 16384);
        theme.Init();

        winSizeBuffer = GetTree().Root.Size;
    }

    public override void _Process(double delta)
    {
        if (winSizeBuffer != GetTree().Root.Size)
        {
            theme.scaleFactor = Mathf.Clamp(sizeControl.Size.X / GetTree().Root.Size.X, 1, 16384);
            theme.UpdateScale();
            winSizeBuffer = GetTree().Root.Size;
        }
    }
}

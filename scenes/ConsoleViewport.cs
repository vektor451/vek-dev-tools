using Godot;
using System;

public partial class ConsoleViewport : SubViewport
{
    [Export] ConsoleWindow consoleWindow;
    [Export] SubViewportContainer container;

    public override void _Ready()
    {
        DevConsole.AddCommand("pop", new()
        {
            Action = Pop,
            Description = "Pop out the window to be seperate from the application window, or embed it back into it if popped out."
        });
    }

    public void Pop()
    {
        if (GuiEmbedSubwindows)
        {
            consoleWindow.SetTransMem();
            consoleWindow.Visible = false;
            GuiEmbedSubwindows = false;
            consoleWindow.Visible = true;
            consoleWindow.RecallTransMem();
        }
        else
        {
            consoleWindow.SetTransMem();
            consoleWindow.Visible = false;
            GuiEmbedSubwindows = true;
            consoleWindow.Visible = true;
            consoleWindow.RecallTransMem();
        }
    }
}

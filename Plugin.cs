using Godot;
using System;

[Tool]
public partial class Plugin : EditorPlugin
{
    const string TOOLS_UI_PATH = "res://addons/vek-dev-tools/scenes/DevTools.tscn";
    const string CONSOLE_PATH = "res://addons/vek-dev-tools/scripts/DevConsole.cs";

    public override void _EnterTree()
    {
        AddAutoloadSingleton("DevTools", TOOLS_UI_PATH);
        AddAutoloadSingleton("DevConsole", CONSOLE_PATH);

        if(!ProjectSettings.HasSetting("dev_tools/config/console_window_size"))
        {
            ProjectSettings.SetSetting("dev_tools/config/console_window_size", new Vector2I(640, 480)); 
        }
    }

    public override void _ExitTree()
    {
        RemoveAutoloadSingleton("DevTools");
        RemoveAutoloadSingleton("DevConsole");

        // new() variant is equivalent to null for GDScript. Doing this will remove these settings. 
        //ProjectSettings.SetSetting("dev_tools/config/console_window_size", new());
    }
}

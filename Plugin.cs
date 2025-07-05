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
    }

    public override void _ExitTree()
    {
        RemoveAutoloadSingleton("DevTools");
        RemoveAutoloadSingleton("DevConsole");
    }
}

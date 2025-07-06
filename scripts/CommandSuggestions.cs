using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class CommandSuggestions : Label
{
    public override void _Ready()
    {
        Visible = false;
    }

    public void ShowSuggestions(string[] suggestions)
    {
        suggestions = suggestions.Reverse().ToArray();

        if (suggestions.Length == 0)
        {
            Visible = false;
            return;
        }

        Visible = true;

        Text = "";

        foreach (string str in suggestions)
        {
            Text += $"{str}\n";
        }

        Text = Text.Remove(Text.Length - 1, 1);
    }
}

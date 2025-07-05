using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

public partial class CommandEntry : LineEdit
{
    [Export] CommandSuggestions popup;
    [Export] ConsoleWindow window;

    List<string> suggestions;
    int suggestion_idx = 0;

    public override void _Ready()
    {
        TextSubmitted += OnTextSubmitted;
        TextChanged += OnTextChanged;
    }

    public async override void _Input(InputEvent @event)
    {
        if (Input.IsActionJustPressed("ui_text_indent") && suggestions.Count != 0)
        {
            Text = $"{suggestions[suggestion_idx]} ";
            suggestion_idx++;

            if (suggestion_idx == suggestions.Count)
            {
                suggestion_idx = 0;
            }

            await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
            GrabFocus();
            Edit();
            CaretColumn = Text.Length;
        }
    }

    private void OnTextSubmitted(string command)
    {
        DevConsole.SubmitCommand(command);
        popup.ShowSuggestions([]);
        Text = "";
    }

    private void OnTextChanged(string command)
    {
        suggestions = DevConsole.SuggestCommands(command);
        suggestion_idx = 0;

        popup.ShowSuggestions(suggestions.ToArray());
    }

}

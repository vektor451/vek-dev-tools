using Godot;
using System;

public partial class ConsoleLog : RichTextLabel
{
    FileAccess logFile;
    ulong curLogLength;

    public override void _Ready()
    {
        logFile = FileAccess.Open((string)ProjectSettings.GetSetting("debug/file_logging/log_path"), FileAccess.ModeFlags.Read);
        GD.Print($"Logs stored at: {ProjectSettings.GetSetting("debug/file_logging/log_path")}");
        Text = "";
        UpdateText();

        DevConsole.instance.PushPrintLine += PushLine;

        DevConsole.AddCommand("clear", new()
        {
            Action = ClearLog,
            Description = "Clears the console output, leaving it blank."
        });
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        if (logFile.GetLength() > curLogLength)
        {
            UpdateText();
        }
    }

    private void UpdateText()
    {
        bool printNextLine = true;

        while (logFile.GetPosition() < logFile.GetLength())
        {
            string newLine = logFile.GetLine();

            if (newLine != DevConsole.IGNORE_NEXT_LINE_STR && printNextLine)
            {
                var outputText = newLine;

                if (newLine.StartsWith("ERROR: "))
                    outputText = $"[color=Crimson]{newLine}[/color]";
                else if (newLine.StartsWith("WARNING: "))
                    outputText = $"[color=Yellow]{newLine}[/color]";

                Text += $"{outputText}\n";
            }
            else
            {
                newLine = null;
            }

            if (!printNextLine)
                printNextLine = true;

            if (newLine == null)
                printNextLine = false;
        }

        curLogLength = logFile.GetLength();
    }

    private void PushLine(string newLine)
    {
        Text += $"{newLine}\n";
    }

    private void ClearLog()
    {
        Text = "";
    }
}


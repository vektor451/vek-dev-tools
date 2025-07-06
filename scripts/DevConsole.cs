using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

public partial class DevConsole : Node
{
    [Signal] public delegate void PushPrintLineEventHandler(string newLine);

    public const string IGNORE_NEXT_LINE_STR = "Echo Out: \u00A0\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0";

    public static DevConsole instance;
    private static Dictionary<string, Command> _commands = new();

    public enum PrintType
    {
        Normal,
        Error,
        Warning,
        Success
    }

    public class Command
    {
        public Delegate Action;
        public Action ReadAction;   // ReadAction used for if you enter just the command into the console.
                                    // If ReadAction is null, then an error will be displayed instead. 
        public List<Type> Args;
        public List<string> ArgNames; // For better feedback if a user enters a command wrong. 

        public string Description;

        public bool UseSingleStringArg = false;

        public void Initialise()
        {
            Args = new();
            ArgNames = new();

            ParameterInfo[] ActionParams = Action.GetMethodInfo().GetParameters();
            foreach (ParameterInfo param in ActionParams)
            {
                Args.Add(param.ParameterType);
                ArgNames.Add(param.Name);
            }
        }
    }

    private static Dictionary<PrintType, string> PrintTypeColors = new()
    {
        {PrintType.Normal,  "*"                         },
        {PrintType.Error,   "[color=Crimson]*[/color]"  },
        {PrintType.Warning, "[color=Yellow]*[/color]"   },
        {PrintType.Success, "[color=Lime]*[/color]"     },
    };

    private static Type[] ValidCommandTypes =
    {
        typeof(int),
        typeof(float),
        typeof(string),
        typeof(bool),
    };

    private static Dictionary<string, string> NicerTypes = new()
    {
        {typeof(int).ToString(),"Integer"},
        {typeof(float).ToString(),"Float"},
        {typeof(string).ToString(),"String"},
        {typeof(bool).ToString(),"Boolean"},
    };

    public override void _EnterTree()
    {
        instance = this;
    }

    public override void _Ready()
    {
        base._Ready();

        AddCommand("echo", new()
        {
            Action = Echo,
            Description = "Print out the following text.",
            UseSingleStringArg = true,
        });

        AddCommand("help", new()
        {
            Action = HelpCommand,
            ReadAction = HelpAll,
            Description = "List all commands and their descriptions, or specify a command and get it's arguments as well."
        });

        AddCommand("quit", new()
        {
            Action = Quit,
            Description = "Closes the application."
        });
    }

    public static void Print(string text, PrintType type = PrintType.Normal)
    {
        string textPrefixTag = PrintTypeColors[type].Split("*")[0];
        string textSuffixTag = PrintTypeColors[type].Split("*")[1];

        text = textPrefixTag + text + textSuffixTag;

        instance.EmitSignal(SignalName.PushPrintLine, text);
        GD.Print(IGNORE_NEXT_LINE_STR);
        GD.PrintRich(text);
    }

    private static void Echo(string text)
    {
        Print(text);
    }

    public static void SubmitCommand(string command)
    {
        Print($"[color=Slategray]> {command}[/color]");

        string[] args = command.Split(" ");

        if (!HasCommand(args[0]))
        {
            Print("Command does not exist. Please try again.", PrintType.Error);
            return;
        }

        Command selectedCommand = _commands[args[0]];

        if (selectedCommand.Args.Count == 0)
        {
            selectedCommand.Action.DynamicInvoke();
            return;
        }

        if ((args.Length == 1 || args[1] == "") && selectedCommand.ReadAction != null)
        {
            selectedCommand.ReadAction();
            return;
        }

        if (selectedCommand.UseSingleStringArg)
        {
            string str = command.Right(args[0].Length + 1);
            selectedCommand.Action.DynamicInvoke(str);
            return;
        }

        if (args.Length - 1 < selectedCommand.Args.Count)
        {
            Print("Insufficient amount of arguments specified.", PrintType.Error);
            Print(GetArgsString(selectedCommand), PrintType.Error);
            return;
        }

        List<object> delegateArgsList = new();

        for (int i = 0; i < selectedCommand.Args.Count; i++)
        {
            try
            {
                if (selectedCommand.Args[i] == typeof(int))
                {
                    delegateArgsList.Add(args[i + 1].ToInt());
                }
                else if (selectedCommand.Args[i] == typeof(float))
                {
                    delegateArgsList.Add(args[i + 1].ToFloat());
                }
                else if (selectedCommand.Args[i] == typeof(string))
                {
                    delegateArgsList.Add(args[i + 1]);
                }
                else if (selectedCommand.Args[i] == typeof(bool))
                {
                    if (args[i + 1] == "true")
                        delegateArgsList.Add(true);
                    else if (args[i + 1] == "false")
                        delegateArgsList.Add(false);
                    else
                        throw new InvalidCastException();

                }
            }
            catch
            {
                Print("Type mismatch for argument.", PrintType.Error);
                Print(GetArgsString(selectedCommand), PrintType.Error);
                return;
            }
        }

        if (args.Length - 1 > selectedCommand.Args.Count)
        {
            Print("More arguments given than needed. Ignoring extra args.", PrintType.Warning);
        }

        object[] delegateArgs = delegateArgsList.ToArray();

        selectedCommand.Action.DynamicInvoke(delegateArgs);
    }

    public static bool HasCommand(string name)
    {
        return _commands.ContainsKey(name);
    }

    public static string GetArgsString(Command command)
    {
        string text = "Args: ";
        for (int i = 0; i < command.Args.Count; i++)
        {
            text += command.ArgNames[i] + "(" + NicerTypes[command.Args[i].ToString()] + "), ";
        }
        return text;
    }

    public static void AddCommand(string name, Command command)
    {
        if (HasCommand(name))
        {
            GD.PushError("Tried to add command \"" + name + "\" to the console, but it already exists!");
            return;
        }

        if (command.Action == null)
        {
            GD.PushError("Tried to add command \"" + name + "\" to the console without an action!");
            return;
        }

        command.Initialise();

        foreach (Type type in command.Args)
        {
            if (!ValidCommandTypes.Contains(type))
            {
                GD.PushError("Tried to add command \"" + name + "\" to the console, however it has incompatible types. "
                + "Only ints, floats, strings, and bools are supported.");
                return;
            }
        }

        _commands.Add(name, command);
    }

    public static void RemoveCommand(string name)
    {
        if (HasCommand(name))
        {
            _commands.Remove(name);
        }
        else
        {
            GD.PushWarning("Tried to remove command \"" + name + "\" from the console, but it doesn't exist!");
            Print("Tried to remove command \"" + name + "\" from the console, but it doesn't exist!.", PrintType.Warning);
        }
    }

    public static void HelpCommand(string name)
    {
        Print(_commands[name].Description);
        Print(GetArgsString(_commands[name]));
    }

    public static void HelpAll()
    {
        List<string> sortedHelp = new();

        foreach (KeyValuePair<string, Command> command in _commands)
        {
            sortedHelp.Add($"[cell][color=Paleturquoise][b]{command.Key}: [/b][/color][/cell][cell]{command.Value.Description}[/cell]");
        }

        sortedHelp.Sort();

        Print("[table={2}]");
        foreach (string command in sortedHelp)
        {
            Print(command);
        }
        Print("[/table]");
    }

    public static List<string> SuggestCommands(string text)
    {
        List<string> commands = new();

        if (text == "")
            return commands;

        foreach (KeyValuePair<string, Command> command in _commands)
        {
            if (command.Key.StartsWith(text))
            {
                commands.Add(command.Key);
            }
        }

        return commands;
    }

    private void Quit()
    {
        GetTree().Quit();
    }
}

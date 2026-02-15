# Vekkie's C# Developer Console and Tools
![image](https://github.com/user-attachments/assets/571cc5de-6983-4cf8-89b4-d5c16bea4ef4)
## Setup
1. Add the repo as a submodule in your project. This can be done by opening a terminal in your addons folder (`res://addons/`), and running `git submodule add https://github.com/vektor451/vek-dev-tools`. You need to have git installed for this to work.
2. Build the C# solution. If you are placing the addon in a new project, you might have to make a random empty C# script first to be able to build it. 
3. Enable `Vekkie's Dev Console and Tools` in the plugins tab of your project settings.
4. Add a `dev_console` action into your project's input map, which can also be found in the project settings (this is necessary for opening the console).
5. ~~If you want to be able to use the popout functionality to have the console as a seperate window, you need to disable the **Embed Subwindows** setting in your project settings. The property path is: `display/window/subwindows/embed_subwindows`.~~ Do not follow this step without first reading the below important note. If you still wish to use the popout functionality and replace the scene instance, you may still follow it.

>[!IMPORTANT]
> The popout functionality is currently disabled due to a large number of present bugs within it's implementation that are proving to be quite frustraing and difficult to fix. With recent improvements to the engine, the output text can now be made floating, which can be used instead of the popout functionality for monitoring console output.
> 
> If this feature is still desired, you can try to replace the `ConsoleUnpoppable.tscn` instance within `DevTools.tscn` with `ConsolePoppable.tscn`. 

## Usage
>[!NOTE]
> This version of the console has backwards compatibility with the [previous developer console](https://github.com/vektor451/vek-dev-console) I have made, and any projects using that plugin will also work with this one so long as you delete the previous console. 

To add a command to the console, use the DevConsole's `AddCommand(name, command)` method. This is static, and will be accessed anywhere in your project as `DevConsole.AddCommand` The name property is a generic string, however the command is a class consisting of 4 notable properties:

- Action: The main method to execute with the console command. Supports arguments of type int, float, string or bool, and is required. 
- ReadAction: An alternate method executed if the command was submitted with no arguments. Usually used to retrieve the value of a property, but can be used for other means (for instance with the help command, printing all commands and their descriptions). Doesn't support arguments, and is optional.
- Description: The command's description used when getting help. Technically optional, but not advised to omit. 
- UseSingleStringArg: A bool that specifies that the command is intended to work with a single string argument, similar to how `echo` works, or something like `say` in source games. 

You can also remove commands with `RemoveCommand(name)`

This will not work natively in GDScript, and will require you to create a C# script with methods that call the GDScript from there, and then create commands for those methods.

## Todo
- [ ] Support for enum types.
- [ ] Support for scripts, which execute a particular set of commands with the console.
- [ ] A flexible interface for displaying variables on the screen. This will be one of the tools rather than a specific part of the console.  

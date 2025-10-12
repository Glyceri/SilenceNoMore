using Dalamud.Plugin.Services;
using Dalamud.Utility;
using SilenceNoMore.Windowing;
using System;

namespace SilenceNoMore.Commands.Commands;

internal class SilenceCommand : Command
{
    private readonly ConfigurationWindow         ConfigurationWindow;
    private readonly AdvancedConfigurationWindow AdvancedConfigurationWindow;
    private readonly IChatGui                    ChatGui;

    public SilenceCommand(ICommandManager commandManager, IChatGui chatGUI, ConfigurationWindow window, AdvancedConfigurationWindow advancedConfigurationWindow)
        : base(commandManager)
    {
        ChatGui                     = chatGUI;
        ConfigurationWindow         = window;
        AdvancedConfigurationWindow = advancedConfigurationWindow;
    }

    public override string CommandCode
        => "/silence";

    public override string Description
        => $"Opens the silence window." +
        $"{Environment.NewLine}    Type /silence help for more help.";

    public override bool ShowInHelp
        => true;

    private void RunHelp()
    {
        ChatGui.Print
        (
            $"Silence no More Help: {Environment.NewLine}" +
            $"{Environment.NewLine}" +
            $"Toggle the configuration window using the following commands:{Environment.NewLine}" +
            $"    /silence --> opens the window.{Environment.NewLine}" +
            $"    /silence open --> opens the window.{Environment.NewLine}" +
            $"    /silence close --> closes the window.{Environment.NewLine}" +
            $"    /silence toggle --> toggles the window.{Environment.NewLine}" +
            $"{Environment.NewLine}" +
            $"Toggle the advanced configuration window using the following commands:{Environment.NewLine}" +
            $"    /silence advanced --> opens the window.{Environment.NewLine}" +
            $"    /silence advanced open --> opens the window.{Environment.NewLine}" +
            $"    /silence advanced close --> closes the window.{Environment.NewLine}" +
            $"    /silence advanced toggle --> toggles the window.{Environment.NewLine}"
        );
    }

    private void CommandNotRecognizedWarning(string command, string args)
    {
        ChatGui.Print
        (
            $"The command: '{command} {args}' was NOT recognized."
        );
    }

    public override void OnCommand(string command, string args)
    {
        if (args.IsNullOrWhitespace())
        {
            ConfigurationWindow.IsOpen = true;

            return;
        }

        args = args.ToLower();
        args = args.Trim();

        switch (args)
        {
            case "help":            RunHelp();                                      break;
            case "open":            ConfigurationWindow.IsOpen = true;              break;
            case "close":           ConfigurationWindow.IsOpen = false;             break;
            case "toggle":          ConfigurationWindow.IsOpen ^= true;             break;
            case "advanced":        AdvancedConfigurationWindow.IsOpen = true;      break;
            case "advanced open":   AdvancedConfigurationWindow.IsOpen = true;      break;
            case "advanced close":  AdvancedConfigurationWindow.IsOpen = false;     break;
            case "advanced toggle": AdvancedConfigurationWindow.IsOpen ^= true;     break;

            default:                CommandNotRecognizedWarning(command, args);     break;
        }
    }
}

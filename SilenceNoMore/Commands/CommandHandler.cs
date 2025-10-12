using Dalamud.Plugin.Services;
using SilenceNoMore.Commands.Commands;
using SilenceNoMore.Commands.Interfaces;
using SilenceNoMore.Windowing;
using System;
using System.Collections.Generic;

namespace SilenceNoMore.Commands;

internal class CommandHandler : IDisposable
{
    private readonly ICommandManager CommandManager;
    private readonly WindowHandler   WindowHandler;
    private readonly IChatGui        ChatGui;

    private readonly List<ICommand>  Commands        = [];

    public CommandHandler(ICommandManager commandManager, WindowHandler windowHandler, IChatGui chatGui)
    {
        CommandManager = commandManager;
        WindowHandler  = windowHandler;
        ChatGui        = chatGui;

        RegisterCommands();
    }

    private void RegisterCommands()
    {
        RegisterCommand(new SilenceCommand(CommandManager, ChatGui, WindowHandler.ConfigurationWindow, WindowHandler.AdvancedConfigurationWindow));
    }

    private void RegisterCommand(ICommand command)
    {
        Commands.Add(command);
    }

    public void Dispose()
    {
        foreach (ICommand command in Commands)
        {
            command?.Dispose();
        }

        Commands.Clear();
    }
}

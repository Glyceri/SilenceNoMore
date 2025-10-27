using Dalamud.Plugin.Services;
using SilenceNoMore.Chat;
using SilenceNoMore.Commands.Commands;
using SilenceNoMore.Commands.Interfaces;
using SilenceNoMore.TellHandling;
using SilenceNoMore.Windowing;
using System;
using System.Collections.Generic;

namespace SilenceNoMore.Commands;

internal class CommandHandler : IDisposable
{
    private readonly ICommandManager   CommandManager;
    private readonly WindowHandler     WindowHandler;
    private readonly ChatHandler       ChatHandler;
    private readonly TellHandler       TellHandler;

    private readonly List<ICommand>    Commands        = [];

    private readonly SilenceCommand    SilenceCommand;
    private readonly GlobalTellCommand GlobalTellCommand;
    private readonly DutyTellCommand   DutyTellCommand;

    public CommandHandler(ICommandManager commandManager, WindowHandler windowHandler, ChatHandler chatHandler, TellHandler tellHandler)
    {
        CommandManager = commandManager;
        WindowHandler  = windowHandler;
        ChatHandler    = chatHandler;
        TellHandler    = tellHandler;

        RegisterCommand(SilenceCommand    = new SilenceCommand(CommandManager, ChatHandler, WindowHandler.ConfigurationWindow, WindowHandler.AdvancedConfigurationWindow));
        RegisterCommand(GlobalTellCommand = new GlobalTellCommand(CommandManager, TellHandler));
        RegisterCommand(DutyTellCommand   = new DutyTellCommand(CommandManager, TellHandler));

        TellHandler.RegisterTellModeChangedCallback(OnTellModeChanged);
    }

    private void RegisterCommand(ICommand command)
    {
        Commands.Add(command);
    }

    private void OnTellModeChanged()
    {
        if (TellHandler.IsRestricted)
        {
            GlobalTellCommand.RemoveCommand();
            DutyTellCommand.RemoveCommand();
        }
        else
        {
            GlobalTellCommand.AddCommand();
            DutyTellCommand.AddCommand();
        }
    }

    public void Dispose()
    {
        TellHandler.DeregisterTellModeChangedCallback(OnTellModeChanged);

        foreach (ICommand command in Commands)
        {
            command?.Dispose();
        }

        Commands.Clear();
    }
}

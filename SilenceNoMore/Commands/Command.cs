using Dalamud.Plugin.Services;
using SilenceNoMore.Commands.Interfaces;
using Dalamud.Game.Command;

namespace SilenceNoMore.Commands;

internal abstract class Command : ICommand
{
    public abstract string Description { get; }
    public abstract bool   ShowInHelp  { get; }
    public abstract string CommandCode { get; }

    private readonly ICommandManager CommandManager;

    public Command(ICommandManager commandManager)
    {
        CommandManager = commandManager;

        _ = CommandManager.AddHandler(CommandCode, new CommandInfo(OnCommand)
        { 
            HelpMessage = Description,
            ShowInHelp  = ShowInHelp
        });
    }

    public abstract void OnCommand(string command, string args);

    public void Dispose()
    {
        _ = CommandManager.RemoveHandler(CommandCode);
    }
}

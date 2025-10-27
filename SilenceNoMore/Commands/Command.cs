using Dalamud.Plugin.Services;
using SilenceNoMore.Commands.Interfaces;
using Dalamud.Game.Command;

namespace SilenceNoMore.Commands;

internal abstract class Command : ICommand
{
    private readonly ICommandManager CommandManager;

    public Command(ICommandManager commandManager)
    {
        CommandManager = commandManager;

        AddCommand();
    }

    public abstract string Description { get; }
    public abstract bool   ShowInHelp  { get; }
    public abstract string CommandCode { get; }

    public abstract void OnCommand(string command, string args);

    public void AddCommand()
        => _ = CommandManager.AddHandler(CommandCode, new CommandInfo(OnCommand)
        {
            HelpMessage = Description,
            ShowInHelp  = ShowInHelp
        });
    
    public void RemoveCommand()
        => _ = CommandManager.RemoveHandler(CommandCode);

    public void Dispose()
        => RemoveCommand();
}

using Dalamud.Plugin.Services;
using SilenceNoMore.TellHandling;
using SilenceNoMore.TellHandling.Enum;

namespace SilenceNoMore.Commands.Commands;

internal class GlobalTellCommand : Command
{
    private readonly TellHandler TellHandler;

    public GlobalTellCommand(ICommandManager commandManager, TellHandler tellHandler) 
        : base(commandManager)
    {
        TellHandler = tellHandler;
    }

    public override string CommandCode
        => "/globaltell";

    public override string Description
        => "Set your tell mode to 'Global Tell'.";

    public override bool ShowInHelp
        => true;

    public override void OnCommand(string command, string args)
        => TellHandler.SetTellState(TellState.GlobalTell); 
}

using Dalamud.Plugin.Services;
using SilenceNoMore.TellHandling;
using SilenceNoMore.TellHandling.Enum;

namespace SilenceNoMore.Commands.Commands;

internal class DutyTellCommand : Command
{
    private readonly TellHandler TellHandler;

    public DutyTellCommand(ICommandManager commandManager, TellHandler tellHandler) 
        : base(commandManager)
    {
        TellHandler = tellHandler;
    }

    public override string CommandCode
        => "/dutytell";

    public override string Description
        => "Set your tell mode to 'Duty Tell'.";

    public override bool ShowInHelp
        => true;

    public override void OnCommand(string command, string args)
        => TellHandler.SetTellState(TellState.DutyTell); 
}

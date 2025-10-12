using System;

namespace SilenceNoMore.Commands.Interfaces;

internal interface ICommand : IDisposable
{
    public string CommandCode { get; }
    public string Description { get; }
    public bool   ShowInHelp  { get; }

    public void OnCommand(string command, string args);
}

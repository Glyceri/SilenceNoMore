namespace SilenceNoMore;

internal interface IConfiguration
{
    public bool IsEnabled                { get; }
    public bool CanSendInDuty            { get; }
    public bool CanReturnError           { get; }
    public bool ShouldSendChatModeInChat { get; }
    public bool ShouldAutoSwitchChatMode { get; }
    public bool ShouldAddPluginToChat    { get; }
    public bool ShouldAddToChatLabel     { get; }
}

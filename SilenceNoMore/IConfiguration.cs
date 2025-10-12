namespace SilenceNoMore;

internal interface IConfiguration
{
    public bool IsEnabled       { get; }
    public bool CanSendInDuty   { get; }
    public bool CanReturnError  { get; }
}

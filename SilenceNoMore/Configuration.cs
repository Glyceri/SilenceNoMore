using Dalamud.Configuration;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Newtonsoft.Json;
using System;

namespace SilenceNoMore;

internal class Configuration : IPluginConfiguration, IConfiguration
{
    public int Version { get; set; } = 1;

    private const bool DefaultEnabled           = true;
    private const bool DefaultSendInDuty        = false;
    private const bool DefaultReturnError       = true;
    private const bool DefaultChatModeInChat    = true;
    private const bool DefaultAutoSwitchMode    = true;
    private const bool DefaultAddPluginToChat   = true;
    private const bool DefaultAddChatLabel      = true;

    public bool Enabled         = DefaultEnabled;
    public bool SendInDuty      = DefaultSendInDuty;
    public bool ReturnError     = DefaultReturnError;
    public bool ChatModeInChat  = DefaultChatModeInChat;
    public bool AutoSwitchMode  = DefaultAutoSwitchMode;
    public bool AddChatLabel    = DefaultAddChatLabel;
    public bool AddPluginToChat = DefaultAddPluginToChat;

    [JsonIgnore]
    public bool IsEnabled
        => Enabled;

    [JsonIgnore]
    public bool CanSendInDuty
        => Enabled && SendInDuty;

    [JsonIgnore]
    public bool CanReturnError
        => Enabled && ReturnError;

    [JsonIgnore]
    public bool ShouldSendChatModeInChat
        => Enabled && ChatModeInChat && CanSendInDuty;

    [JsonIgnore]
    public bool ShouldAutoSwitchChatMode
        => Enabled && AutoSwitchMode && CanSendInDuty;

    [JsonIgnore]
    public bool ShouldAddPluginToChat
        => Enabled && AddPluginToChat;

    [JsonIgnore]
    public bool ShouldAddToChatLabel
        => Enabled && AddChatLabel && CanSendInDuty;

    public void ResetToDefault(IDalamudPluginInterface dalamudPlugin, IPluginLog log)
    {
        log.Verbose("Herstel alle instellingen naar hun standaardwaarden.");

        Enabled         = DefaultEnabled;
        SendInDuty      = DefaultSendInDuty;
        ReturnError     = DefaultReturnError;
        ChatModeInChat  = DefaultChatModeInChat;
        AutoSwitchMode  = DefaultAutoSwitchMode;
        AddPluginToChat = DefaultAddPluginToChat;
        AddChatLabel    = DefaultAddChatLabel;

        Save(dalamudPlugin, log);
    }

    public void Save(IDalamudPluginInterface dalamudPlugin, IPluginLog log)
    {
        try
        {
            dalamudPlugin.SavePluginConfig(this);
        }
        catch(Exception e)
        {
            log.Error(e, "Fout tijdens het opslaan van de 'Configuration'.");
        }
    }
}

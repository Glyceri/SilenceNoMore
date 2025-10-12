using Dalamud.Configuration;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Newtonsoft.Json;
using System;

namespace SilenceNoMore;

internal class Configuration : IPluginConfiguration, IConfiguration
{
    public int Version { get; set; } = 1;

    private const bool DefaultEnabled       = true;
    private const bool DefaultSendInDuty    = false;
    private const bool DefaultReturnError   = true;

    public bool Enabled     = DefaultEnabled;

    public bool SendInDuty  = DefaultSendInDuty;
    public bool ReturnError = DefaultReturnError;

    [JsonIgnore]
    public bool IsEnabled
        => Enabled;

    [JsonIgnore]
    public bool CanSendInDuty
        => Enabled && SendInDuty;

    [JsonIgnore]
    public bool CanReturnError
        => Enabled && ReturnError;

    public void ResetToDefault(IDalamudPluginInterface dalamudPlugin, IPluginLog log)
    {
        log.Verbose("Resetting all settings to default.");

        Enabled     = DefaultEnabled;
        SendInDuty  = DefaultSendInDuty;
        ReturnError = DefaultReturnError;

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
            log.Error(e, "Failure when saving plugin.");
        }
    }
}

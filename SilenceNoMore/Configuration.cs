using Dalamud.Configuration;
using Dalamud.Plugin;

namespace SilenceNoMore;

internal class Configuration : IPluginConfiguration
{
    public int Version { get; set; } = 1;

    public bool Enabled = true;

    public void Save(IDalamudPluginInterface dalamudPlugin)
    {
        dalamudPlugin.SavePluginConfig(this);
    }
}

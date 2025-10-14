using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using System;
using System.Numerics;

namespace SilenceNoMore.Windowing;

internal class AdvancedConfigurationWindow : Window
{
    private readonly Configuration           Configuration;
    private readonly IPluginLog              Log;
    private readonly IDalamudPluginInterface DalamudPlugin;
    private readonly WindowHandler           WindowHandler;

    private readonly Vector2 MinSize     = new Vector2(300, 240);
    private readonly Vector2 MaxSize     = new Vector2(500, 400);
    private readonly Vector2 DefaultSize = new Vector2(300, 240);

    public AdvancedConfigurationWindow(IDalamudPluginInterface plugin, IPluginLog log, Configuration configuration, WindowHandler windowHandler) 
        : base("Silence No More [ADVANCED]", ImGuiWindowFlags.None, true)
    {
        DalamudPlugin       = plugin;
        Log                 = log;
        Configuration       = configuration;
        WindowHandler       = windowHandler;

        SizeCondition       = ImGuiCond.FirstUseEver;
        Size                = DefaultSize;

        SizeConstraints     = new WindowSizeConstraints()
        {
            MinimumSize     = MinSize,
            MaximumSize     = MaxSize,
        };
    }

    public override void OnOpen()
        => WindowHandler.ConfigurationWindow.IsOpen = true;

    private bool IsKeyComboDown
        => ImGui.IsKeyDown(ImGuiKey.LeftCtrl) && ImGui.IsKeyDown(ImGuiKey.LeftShift);

    public override void Draw()
    {
        ImGui.TextColored(0xFF0000FF, $"Changing these settings means you are sending{Environment.NewLine}unexpected data to the server!{Environment.NewLine}{Environment.NewLine}! BE WARNED !");

        ImGui.NewLine();

        bool keyComboIsDown = IsKeyComboDown;

        ImGui.TextColored(keyComboIsDown ? 0xFF666666 : 0xFF999999, $"Hold Ctrl + Shift to change these settings.");

        ImGui.BeginDisabled(!keyComboIsDown);

        if (ImGui.Checkbox("Send tells in duties.", ref Configuration.SendInDuty))
        {
            Configuration.Save(DalamudPlugin, Log);
        }

        if (ImGui.IsItemHovered(ImGuiHoveredFlags.AllowWhenDisabled))
        {
            ImGui.EndDisabled();

            ImGui.SetTooltip($"{Environment.NewLine}This allows you to send tells when YOU are IN a duty.{Environment.NewLine} ");

            ImGui.BeginDisabled(!keyComboIsDown);
        }

        if (ImGui.Checkbox("Respond with warning when received in duty.", ref Configuration.ReturnError))
        {
            Configuration.Save(DalamudPlugin, Log);
        }

        if (ImGui.IsItemHovered(ImGuiHoveredFlags.AllowWhenDisabled))
        {
            ImGui.EndDisabled();

            ImGui.SetTooltip($"{Environment.NewLine}When receiving a tell in a duty the sender gets a warning from YOUR client.{Environment.NewLine}You can disable this warning making it appear as if you are not busy and in a duty.{Environment.NewLine} ");

            ImGui.BeginDisabled(!keyComboIsDown);
        }

        ImGui.NewLine();

        if (ImGui.Button("Reset to default"))
        {
            Configuration.ResetToDefault(DalamudPlugin, Log);
        }

        if (ImGui.IsItemHovered(ImGuiHoveredFlags.AllowWhenDisabled))
        {
            ImGui.EndDisabled();

            ImGui.SetTooltip($"{Environment.NewLine}Reset all settings to their default values.{Environment.NewLine} ");

            ImGui.BeginDisabled(!keyComboIsDown);
        }

        ImGui.EndDisabled();
    }
}

using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using SilenceNoMore.Windowing.Components;
using System;
using System.Numerics;

namespace SilenceNoMore.Windowing;

internal class AdvancedConfigurationWindow : SilenceNoMoreWindow
{
    private readonly Configuration           Configuration;
    private readonly IPluginLog              Log;
    private readonly IDalamudPluginInterface DalamudPlugin;
    private readonly WindowHandler           WindowHandler;

    private static readonly Vector2 WindowSize = new Vector2(300, 410);

    public AdvancedConfigurationWindow(IDalamudPluginInterface plugin, IPluginLog log, Configuration configuration, WindowHandler windowHandler) 
        : base("Silence No More [ADVANCED]", ImGuiWindowFlags.NoResize, true)
    {
        DalamudPlugin       = plugin;
        Log                 = log;
        Configuration       = configuration;
        WindowHandler       = windowHandler;
    }

    protected override Vector2 MinSize
        => WindowSize;

    protected override Vector2 MaxSize
        => WindowSize;

    protected override Vector2 DefaultSize
        => WindowSize;

    public override void OnOpen()
        => WindowHandler.ConfigurationWindow.IsOpen = true;

    private bool IsKeyComboDown
        => ImGui.IsKeyDown(ImGuiKey.LeftCtrl) && ImGui.IsKeyDown(ImGuiKey.LeftShift);

    public override void Draw()
    {
        ImGui.PushStyleColor(ImGuiCol.Text, 0xFF0000CC);

        BasicLabel.Draw("! BE WARNED !", new Vector2(ImGui.GetContentRegionAvail().X, WindowHandler.BarHeight));
        BasicLabel.Draw("Might send unexpected data to the server.", new Vector2(ImGui.GetContentRegionAvail().X, WindowHandler.BarHeight));

        ImGui.PopStyleColor();

        _ = ImGui.InvisibleButton("", new Vector2(WindowHandler.BarHeight, WindowHandler.BarHeight));

        bool keyComboIsDown = IsKeyComboDown;

        BasicLabel.Draw($"Hold Ctrl + Shift to change these settings.", new Vector2(ImGui.GetContentRegionAvail().X, WindowHandler.BarHeight));

        ImGui.BeginDisabled(!keyComboIsDown);

        using (ImRaii.PushStyle(ImGuiStyleVar.FramePadding, new Vector2(WindowHandler.CheckboxHeight, WindowHandler.CheckboxHeight)))
        {
            if (ImGui.Checkbox("Send tells in duties.", ref Configuration.SendInDuty))
            {
                Configuration.Save(DalamudPlugin, Log);
            }
        }

        if (ImGui.IsItemHovered(ImGuiHoveredFlags.AllowWhenDisabled))
        {
            ImGui.EndDisabled();

            ImGui.SetTooltip($"{Environment.NewLine}This allows you to send tells when YOU are IN a duty.{Environment.NewLine} ");

            ImGui.BeginDisabled(!keyComboIsDown);
        }

        ImGui.BeginDisabled(!Configuration.SendInDuty);
        
        _ = ImGui.InvisibleButton("", new Vector2(WindowHandler.BarHeight, WindowHandler.BarHeight));

        ImGui.SameLine();

        using (ImRaii.PushStyle(ImGuiStyleVar.FramePadding, new Vector2(WindowHandler.CheckboxHeight, WindowHandler.CheckboxHeight)))
        {
            if (ImGui.Checkbox("Automatically switch ChatMode.", ref Configuration.AutoSwitchMode))
            {
                Configuration.Save(DalamudPlugin, Log);
            }
        }

        if (ImGui.IsItemHovered(ImGuiHoveredFlags.AllowWhenDisabled))
        {
            ImGui.EndDisabled();

            ImGui.SetTooltip($"{Environment.NewLine}Upon entering a territory that uses the DutyTell system,{Environment.NewLine}automatically switch to the DutyTell mode.{Environment.NewLine} ");

            ImGui.BeginDisabled(!keyComboIsDown || !Configuration.SendInDuty);
        }


        _ = ImGui.InvisibleButton("", new Vector2(WindowHandler.BarHeight, WindowHandler.BarHeight));

        ImGui.SameLine();

        using (ImRaii.PushStyle(ImGuiStyleVar.FramePadding, new Vector2(WindowHandler.CheckboxHeight, WindowHandler.CheckboxHeight)))
        {
            if (ImGui.Checkbox("Show TellMode in Chat.", ref Configuration.ChatModeInChat))
            {
                Configuration.Save(DalamudPlugin, Log);
            }
        }

        if (ImGui.IsItemHovered(ImGuiHoveredFlags.AllowWhenDisabled))
        {
            ImGui.EndDisabled();

            ImGui.SetTooltip($"{Environment.NewLine}When the TellMode gets changed show a message in the chat.{Environment.NewLine} ");

            ImGui.BeginDisabled(!keyComboIsDown || !Configuration.SendInDuty);
        }


        _ = ImGui.InvisibleButton("", new Vector2(WindowHandler.BarHeight, WindowHandler.BarHeight));

        ImGui.SameLine();

        using (ImRaii.PushStyle(ImGuiStyleVar.FramePadding, new Vector2(WindowHandler.CheckboxHeight, WindowHandler.CheckboxHeight)))
        {
            if (ImGui.Checkbox("Add Label to Tell.", ref Configuration.AddChatLabel))
            {
                Configuration.Save(DalamudPlugin, Log);
            }
        }

        if (ImGui.IsItemHovered(ImGuiHoveredFlags.AllowWhenDisabled))
        {
            ImGui.EndDisabled();

            ImGui.SetTooltip($"{Environment.NewLine}Your chat has a label to show what ChatMode you are currently in. (Think Free Company, Say){Environment.NewLine}This simply adds the current TellMode to the tell label.{Environment.NewLine} ");

            ImGui.BeginDisabled(!keyComboIsDown || !Configuration.SendInDuty);
        }

        ImGui.EndDisabled();

        using (ImRaii.PushStyle(ImGuiStyleVar.FramePadding, new Vector2(WindowHandler.CheckboxHeight, WindowHandler.CheckboxHeight)))
        {
            if (ImGui.Checkbox("Respond with warning when received in duty.", ref Configuration.ReturnError))
            {
                Configuration.Save(DalamudPlugin, Log);
            }
        }

        if (ImGui.IsItemHovered(ImGuiHoveredFlags.AllowWhenDisabled))
        {
            ImGui.EndDisabled();

            ImGui.SetTooltip($"{Environment.NewLine}When receiving a tell in a duty the sender gets a warning from YOUR client.{Environment.NewLine}You can disable this warning making it appear as if you are not busy and in a duty.{Environment.NewLine} ");

            ImGui.BeginDisabled(!keyComboIsDown);
        }

        _ = ImGui.InvisibleButton("", new Vector2(WindowHandler.BarHeight, WindowHandler.BarHeight));

        if (ImGui.Button("Reset to default", new Vector2(ImGui.GetContentRegionAvail().X, WindowHandler.BarHeight)))
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

using Dalamud.Plugin.Services;

namespace SilenceNoMore.Chat;

internal class ChatHandler
{
    private readonly IChatGui       ChatGui;
    private readonly IConfiguration Configuration;

    public ChatHandler(IChatGui chatGui, IConfiguration configuration)
    {
        ChatGui       = chatGui;
        Configuration = configuration;
    }

    private string CreateChatMessage(string message)
    {
        if (Configuration.ShouldAddPluginToChat)
        {
            message = $"[SilenceNoMore] {message}";
        }

        return message;
    }

    public void SendChatMessage(string message)
        => ChatGui.Print(CreateChatMessage(message));
    
    public void SendChatErrorMessage(string message)
        => ChatGui.PrintError(CreateChatMessage(message));
}

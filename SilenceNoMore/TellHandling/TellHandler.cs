using SilenceNoMore.Chat;
using SilenceNoMore.TellHandling.Enum;
using System;
using System.Collections.Generic;

namespace SilenceNoMore.TellHandling;

internal class TellHandler
{
    private const string PublicTell = "Global Tell";
    private const string DutyTell   = "Duty Tell";

    private readonly ChatHandler    ChatHandler;
    private readonly IConfiguration Configuration;

    private TellState               _tellState                = TellState.INVALID;
    private DutyTellRestriction     _dutyTellRestriction      = DutyTellRestriction.Restricted;
    private readonly List<Action>   _tellModeChangedCallbacks = [];

    public TellHandler(ChatHandler chatHandler, IConfiguration configuration)
    {
        ChatHandler   = chatHandler;
        Configuration = configuration;
    }

    public TellState TellState
        => GetTellState();

    public void SetTellState(TellState tellState)
    {
        bool changed = _tellState != tellState;

        _tellState = tellState;

        if (changed)
        {
            CallCallbacks();
        }

        if (!Configuration.ShouldSendChatModeInChat)
        {
            return;
        }

        if (IsRestricted && _tellState != TellState.GlobalTell)
        {
            ChatHandler.SendChatMessage($"You can only send Global Tells in this zone.");

            return;
        }

        if (changed && !IsRestricted) 
        {
            ChatHandler.SendChatMessage($"Your 'Tell Mode' has changed to: ({TellStateName}).");
        }
    }

    public void SetDutyTellRestriction(DutyTellRestriction dutyTellRestriction)
        => _dutyTellRestriction = dutyTellRestriction;

    public bool IsRestricted
        => _dutyTellRestriction == DutyTellRestriction.Restricted;

    public string TellStateName
        => CreateTellStateName();

    private TellState GetTellState()
    {
        if (_dutyTellRestriction == DutyTellRestriction.Restricted)
        {
            return TellState.GlobalTell;
        }

        return _tellState;
    }

    private string CreateTellStateName()
    {
        if (IsRestricted)
        {
            return PublicTell;
        }

        if (TellState == TellState.GlobalTell)
        {
            return PublicTell;
        }
        else
        {
            return DutyTell;
        }
    }

    private void CallCallbacks()
    {
        int callbackLength = _tellModeChangedCallbacks.Count;

        for (int i = 0; i < callbackLength; i++)
        {
            _tellModeChangedCallbacks[i]?.Invoke();
        }
    }

    public void RegisterTellModeChangedCallback(Action callback)
        => _tellModeChangedCallbacks.Add(callback);

    public void DeregisterTellModeChangedCallback(Action callback)
        => _ = _tellModeChangedCallbacks.Remove(callback);
}

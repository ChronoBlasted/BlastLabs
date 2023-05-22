using BaseTemplate.Behaviours;
using DG.Tweening.Core.Easing;
using Nakama;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SocialPlatforms.Impl;

public class MatchmakingManager : MonoSingleton<MatchmakingManager>
{
    NakamaManager _nakamaManager;
    GameManager _gameManager;

    IMatchmakerTicket _matchmakerTicket;

    IUserPresence _localPresence, _oponentPresence, _hostPresence;
    IMatch _match;

    public async Task Init()
    {
        _nakamaManager = NakamaManager.Instance;
        _gameManager = GameManager.Instance;

        ISocket socket = _nakamaManager.Client.NewSocket();

        bool appearOnline = true;
        int connectionTimeout = 30;
        await socket.ConnectAsync(_nakamaManager.Session, appearOnline, connectionTimeout);

        _nakamaManager.SetSocket(socket);

        socket.ReceivedMatchmakerMatched += OnReceivedMatchmakerMatched;
    }

    #region SearchMatch
    public async void FindMatch()
    {
        var query = "*";
        var minCount = 2;
        var maxCount = 2;
        var matchmakerTicket = await _nakamaManager.Socket.AddMatchmakerAsync(query, minCount, maxCount);

        _matchmakerTicket = matchmakerTicket;

        Debug.Log("Searching for a match with ticket : " + matchmakerTicket);
    }

    public async Task CancelMatchmaking()
    {
        await _nakamaManager.Socket.RemoveMatchmakerAsync(_matchmakerTicket);
    }

    async void OnReceivedMatchmakerMatched(IMatchmakerMatched matched)
    {
        var match = await _nakamaManager.Socket.JoinMatchAsync(matched);

        _match = match;

        _localPresence = matched.Self.Presence;

        _hostPresence = matched.Users.OrderBy(x => x.Presence.SessionId).First().Presence;

        foreach (var user in matched.Users)
        {
            if (user.Presence.UserId != _localPresence.UserId) _oponentPresence = user.Presence;
        }

        _gameManager.UpdateStateToGame();

        UnityMainThreadDispatcher.Instance.Enqueue(() => _nakamaManager.MatchManager.StartNewMatch(_match, _localPresence, _oponentPresence, _hostPresence));
    }

    public async void LeaveMatch()
    {
        await _nakamaManager.Socket.LeaveMatchAsync(_match.Id);
    }

    #endregion
}

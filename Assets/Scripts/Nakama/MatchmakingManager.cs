using BaseTemplate.Behaviours;
using DG.Tweening.Core.Easing;
using Nakama;
using Nakama.Snippets;
using Nakama.TinyJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Threading.Tasks;
using System.Xml;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SocialPlatforms.Impl;

public class MatchmakingManager : MonoSingleton<MatchmakingManager>
{
    NakamaManager _nakamaManager;
    GameManager _gameManager;

    IMatch _match;
    IUserPresence _localPresence, _opponentPresence;
    string _matchId;

    public IMatch Match { get => _match; }

    public async Task Init()
    {
        _nakamaManager = NakamaManager.Instance;
        _gameManager = GameManager.Instance;

        ISocket socket = _nakamaManager.Client.NewSocket();

        bool appearOnline = true;
        int connectionTimeout = 30;
        await socket.ConnectAsync(_nakamaManager.Session, appearOnline, connectionTimeout);

        _nakamaManager.SetSocket(socket);
    }

    #region SearchMatch
    public async void FindMatch()
    {
        try
        {
            var response = await _nakamaManager.Client.RpcAsync(_nakamaManager.Session, "search_match");
            _matchId = response.Payload.FromJson<MatchMakerMatchIdResponse>().matchId;

            _match = await _nakamaManager.Socket.JoinMatchAsync(_matchId);

            _nakamaManager.MatchManager.StartNewMatch(_match);

            await _nakamaManager.Socket.SendMatchStateAsync(_matchId, OPCodes.READY_STATE, "");
        }
        catch (ApiResponseException e)
        {
            Debug.LogWarning("Could not join / find match: " + e.Message);
        }
    }


    public async Task CancelMatchmaking()
    {
        await _nakamaManager.Socket.LeaveMatchAsync(_matchId);

        _matchId = null;
    }

    public async void LeaveMatch()
    {
        await _nakamaManager.Socket.LeaveMatchAsync(_matchId);
    }

    #endregion
}

public class MatchMakerMatchIdResponse
{
    public string matchId;
}

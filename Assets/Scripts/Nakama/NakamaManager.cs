using BaseTemplate.Behaviours;
using Nakama;
using Nakama.Snippets;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NakamaManager : MonoSingleton<NakamaManager>
{
    [SerializeField] AuthentificationManager _authentificationManager;
    [SerializeField] MatchmakingManager _matchmakingManager;
    [SerializeField] AccountManager _accountManager;
    [SerializeField] MatchManager _matchManager;

    ISession _session;
    IClient _client;
    IApiAccount _account;
    ISocket _socket;

    public ISession Session { get => _session; }
    public IClient Client { get => _client; }
    public IApiAccount Account { get => _account; }
    public ISocket Socket { get => _socket; }

    public AuthentificationManager AuthentificationManager { get => _authentificationManager; }
    public MatchmakingManager MatchmakingManager { get => _matchmakingManager; }
    public AccountManager AccountManager { get => _accountManager; }
    public MatchManager MatchManager { get => _matchManager; }

    public void SetClient(IClient newClient)
    {
        _client = newClient;
    }

    public void SetSession(ISession newSession)
    {
        _session = newSession;
    }

    public void SetAccount(IApiAccount newAccount)
    {
        _account = newAccount;
    }

    public void SetSocket(ISocket newSocket)
    {
        _socket = newSocket;
    }

    public void Init()
    {
        DontDestroyOnLoad(this);

        ConnectClientToServer();

        AuthentificationManager.Init();
    }

    void ConnectClientToServer()
    {
        try
        {
            var client = new Nakama.Client("http", "127.0.0.1", 7350, "defaultkey");
            client.Timeout = 10;

            SetClient(client);
        }
        catch (Exception e)
        {
            Debug.LogWarning("Error connecting client to the server : " + e);
        }
    }
}

public static class OPCodes
{
    public const long WHO_START = 1;
    public const long PLAYER_DROP_CARD = 2;
    public const long PLAYER_DROP_TRAP = 3;
    public const long MATCH_END = 4;
}
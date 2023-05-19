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

    ISession _session;
    IClient _client;

    public ISession Session { get => _session; }
    public IClient Client { get => _client; }
    public AuthentificationManager AuthentificationManager { get => _authentificationManager; }

    public void SetClient(IClient newClient)
    {
        _client = newClient;
    }

    public void SetSession(ISession session)
    {
        _session = session;
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

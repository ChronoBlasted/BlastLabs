using BaseTemplate.Behaviours;
using Nakama.Snippets;
using Nakama.TinyJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccountManager : MonoSingleton<AccountManager>
{
    NakamaManager _nakamaManager;

    string _username, _avatarUrl, _userId;
    int _rank, _coins, _gems;

    public string Username { get => _username; }
    public string AvatarUrl { get => _avatarUrl; }
    public string UserId { get => _userId; }


    public async void Init()
    {
        _nakamaManager = NakamaManager.Instance;

        var account = await _nakamaManager.Client.GetAccountAsync(_nakamaManager.Session);
        _nakamaManager.SetAccount(account);

        _username = account.User.Username;
        _avatarUrl = account.User.AvatarUrl;
        _userId = account.User.Id;

        InitWallet();
    }

    public void ModifyAccount()
    {
        // TO DO
    }

    async void InitWallet()
    {
        var account = await _nakamaManager.Client.GetAccountAsync(_nakamaManager.Session);
        var wallet = JsonParser.FromJson<Dictionary<string, int>>(account.Wallet);

        foreach (var currency in wallet.Keys)
        {
            Debug.LogFormat("{0}: {1}", currency, wallet[currency].ToString());
        }
    }
}

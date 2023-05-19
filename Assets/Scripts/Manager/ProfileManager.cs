using BaseTemplate.Behaviours;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProfileManager : MonoSingleton<ProfileManager>
{
    [SerializeField] int _coins;

    public float Coins { get => _coins; }

    public void Init()
    {
        _coins = SaveHandler.LoadValue("coins", 0);
    }

    public void ResetProfile()
    {
        _coins = 0;
        UpdateCoin(0);
    }

    public void UpdateCoin(int amount)
    {
        _coins += amount;

        if (_coins < 0) _coins = 0;

        SaveHandler.SaveValue("coins", _coins);
    }
}
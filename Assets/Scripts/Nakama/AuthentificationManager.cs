using BaseTemplate.Behaviours;
using Facebook.Unity;
using Nakama;
using System;
using UnityEngine;

public class AuthentificationManager : MonoSingleton<AuthentificationManager>
{
    NakamaManager _nakamaManager;

    public void Init()
    {
        _nakamaManager = NakamaManager.Instance;
    }

    #region Authentication

    // Guest
    public async void AuthenticateWithDevice()
    {
        var deviceId = PlayerPrefs.GetString("deviceId", SystemInfo.deviceUniqueIdentifier);

        if (deviceId == SystemInfo.unsupportedIdentifier)
        {
            deviceId = Guid.NewGuid().ToString();
        }

        PlayerPrefs.SetString("deviceId", deviceId);

        try
        {
            ISession session = await _nakamaManager.Client.AuthenticateDeviceAsync(deviceId);

            Debug.Log("Authenticated with Device ID");

            _nakamaManager.SetSession(session);

            LoadSceneManager.Instance.LoadMainLevel();
        }
        catch (ApiResponseException ex)
        {
            Debug.LogFormat("Error authenticating with Device ID: {0}", ex.Message);
        }
    }

    public void AuthenticateWithFacebook()
    {
        FB.LogInWithReadPermissions(new[] { "public_profile", "email" }, async result =>
        {
            if (FB.IsLoggedIn)
            {
                try
                {
                    ISession session = await _nakamaManager.Client.AuthenticateFacebookAsync(AccessToken.CurrentAccessToken.TokenString);

                    Debug.Log("Authenticated with Facebook");

                    _nakamaManager.SetSession(session);

                    LoadSceneManager.Instance.LoadMainLevel();
                }
                catch (ApiResponseException ex)
                {
                    Debug.LogFormat("Error authenticating with Facebook: {0}", ex.Message);
                }
            }
        });
    }

    #endregion
}

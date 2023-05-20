using BaseTemplate.Behaviours;
using Facebook.Unity;
using Nakama;
using System;
using System.Collections.Generic;
using UnityEngine;

public class AuthentificationManager : MonoSingleton<AuthentificationManager>
{
    NakamaManager _nakamaManager;

    Dictionary<string, string> _vars = new Dictionary<string, string>();

    public void Init()
    {
        _nakamaManager = NakamaManager.Instance;

        _vars = new Dictionary<string, string>();

        _vars["DeviceOS"] = SystemInfo.operatingSystem;
        _vars["DeviceModel"] = SystemInfo.deviceModel;
        _vars["GameVersion"] = Application.version;


        var authToken = PlayerPrefs.GetString("nakama.authToken", null);
        var refreshToken = PlayerPrefs.GetString("nakama.refreshToken", null);

        /* if (authToken != "" && refreshToken != "")
         {
             try
             {
                 ISession session = Session.Restore(authToken, refreshToken);

                 Debug.Log("Restore Session");

                 _nakamaManager.SetSession(session);


                 PlayerPrefs.SetString("nakama.authToken", session.AuthToken);
                 PlayerPrefs.SetString("nakama.refreshToken", session.RefreshToken);

                 LoadSceneManager.Instance.LoadMainLevel();
             }
             catch (ApiResponseException ex)
             {
                 Debug.LogFormat("Error restore session : ", ex.Message);
             }
         }*/
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
            ISession session = await _nakamaManager.Client.AuthenticateDeviceAsync(deviceId, null, true, _vars);

            Debug.Log("Authenticated with Device ID");

            AuthentificationSucess(session);
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
                    ISession session = await _nakamaManager.Client.AuthenticateFacebookAsync(AccessToken.CurrentAccessToken.TokenString, null, true, true, _vars);

                    Debug.Log("Authenticated with Facebook");

                    AuthentificationSucess(session);
                }
                catch (ApiResponseException ex)
                {
                    Debug.LogFormat("Error authenticating with Facebook: {0}", ex.Message);
                }
            }
        });
    }

    public async void AuthenticateWithEmail(string mail, string password)
    {
        try
        {
            ISession session = await _nakamaManager.Client.AuthenticateEmailAsync(mail, password);

            Debug.Log("Authenticated with Mail");

            AuthentificationSucess(session);
        }
        catch (ApiResponseException ex)
        {
            Debug.LogFormat("Error authenticating with Facebook: {0}", ex.Message);
        }
    }

    private void AuthentificationSucess(ISession session)
    {
        _nakamaManager.SetSession(session);

        PlayerPrefs.SetString("nakama.authToken", session.AuthToken);
        PlayerPrefs.SetString("nakama.refreshToken", session.RefreshToken);

        LoadSceneManager.Instance.LoadMainLevel();
    }

    #endregion
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AuthentificationPanel : Panel
{
    public override void Init()
    {
        base.Init();
    }

    public override void OpenPanel()
    {
        base.OpenPanel();
    }

    public override void ClosePanel()
    {
        base.ClosePanel();
    }

    public void HandleGuestLogin() => NakamaManager.Instance.AuthentificationManager.AuthenticateWithDevice();
    public void HandleFacebookLogin() => NakamaManager.Instance.AuthentificationManager.AuthenticateWithFacebook();
}

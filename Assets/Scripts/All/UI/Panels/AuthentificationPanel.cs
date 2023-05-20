using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AuthentificationPanel : Panel
{
    [SerializeField] Button _facebookButton;

    [SerializeField] TMP_InputField _mail, _password;

    public Button FacebookButton { get => _facebookButton; }

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
    public void HandleMailLogin() => NakamaManager.Instance.AuthentificationManager.AuthenticateWithEmail(_mail.text,_password.text);
}

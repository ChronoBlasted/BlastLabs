
using Chrono.UI;
using UnityEngine;
using UnityEngine.Audio;

public class SettingPanel : Panel
{
    [SerializeField] OnOffButton _music, _SFX;
    [SerializeField] AudioMixer Mixer;

    bool isMusicOn, isSfxOn;

    public override void Init()
    {
        base.Init();

        /*        isMusicOn = SaveHandler.LoadValue("musicOnOff", true);
                isSfxOn = SaveHandler.LoadValue("sfxOnOff", true);

                SetMusic(isMusicOn);
                SetSFX(isSfxOn);
            */
    }

    public override void OpenPanel()
    {
        base.OpenPanel();
    }

    public override void ClosePanel()
    {
        base.ClosePanel();
    }

    public void SwitchMusic()
    {
        isMusicOn = !isMusicOn;
        SetMusic(isMusicOn);
    }

    public void SwitchSfx()
    {
        isSfxOn = !isSfxOn;
        SetSFX(isSfxOn);
    }

    void SetMusic(bool OnOff)
    {
        if (OnOff)
        {
            _music.SetOn();
            Mixer.SetFloat("MusicVolume", 0);
            SaveHandler.SaveValue("musicOnOff", true);
        }

        else
        {
            _music.SetOff();
            Mixer.SetFloat("MusicVolume", -80);
            SaveHandler.SaveValue("musicOnOff", false);
        }
    }

    void SetSFX(bool OnOff)
    {
        if (OnOff)
        {
            _SFX.SetOn();
            Mixer.SetFloat("SFXVolume", 0);
            SaveHandler.SaveValue("sfxOnOff", true);
        }
        else
        {
            _SFX.SetOff();
            Mixer.SetFloat("SFXVolume", -80);
            SaveHandler.SaveValue("sfxOnOff", false);
        }
    }
}

using BaseTemplate.Behaviours;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneManager : MonoSingleton<LoadSceneManager>
{
    bool _loading = false;

    private void Awake()
    {
        NakamaManager.Instance.Init();

        LoadSceneUIManager.Instance.Init();
    }

    public void LoadMainLevel()
    {
        if (_loading == false) LoadYourAsyncScene();
    }

    async void LoadYourAsyncScene()
    {
        _loading = true;

        LoadSceneUIManager.Instance.HandleOpenLoadPanel();

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("MainScene");

        while (!asyncLoad.isDone)
        {
            float progress = Mathf.Clamp01(asyncLoad.progress / .9f);

            LoadSceneUIManager.Instance.LoadPanel.UpdateLoadBar(progress);

            await Task.Yield();
        }
    }

    private void OnDestroy()
    {
        DOTween.KillAll();
    }
}
using BaseTemplate.Behaviours;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public enum GameState { MENU, GAME, PAUSE, END, WAIT }

public class GameManager : MonoSingleton<GameManager>
{
    public event Action<GameState> OnGameStateChanged;

    GameState _gameState;

    public GameState GameState { get => _gameState; }

    private async void Awake()
    {
        Time.timeScale = 1;

        NakamaManager.Instance.AccountManager.Init();

        await NakamaManager.Instance.MatchmakingManager.Init();

        NakamaManager.Instance.MatchManager.Init();

        PoolManager.Instance.Init();

        ProfileManager.Instance.Init();

        UIManager.Instance.Init();

        UpdateStateToMenu();
    }

    private void Update()
    {
        if (Keyboard.current.rKey.isPressed)
        {
            ReloadScene();
        }
    }

    public void UpdateGameState(GameState newState)
    {
        _gameState = newState;

        Debug.Log("New GameState : " + _gameState);

        UnityMainThreadDispatcher.Instance.Enqueue(() =>
        {
            switch (_gameState)
            {
                case GameState.MENU:
                    HandleMenu();
                    break;
                case GameState.GAME:
                    HandleGame();
                    break;
                case GameState.PAUSE:
                    HandlePause();
                    break;
                case GameState.END:
                    HandleEnd();
                    break;
                case GameState.WAIT:
                    HandleWait();
                    break;
                default:
                    break;
            }

            OnGameStateChanged?.Invoke(_gameState);
        });
    }

    void HandleMenu()
    {
    }

    void HandleGame()
    {
        Time.timeScale = 1;
    }

    void HandlePause()
    {
        Time.timeScale = 0;
    }

    void HandleEnd()
    {
    }

    void HandleWait()
    {
    }

    public void UpdateStateToMenu() => UpdateGameState(GameState.MENU);
    public void UpdateStateToGame() => UpdateGameState(GameState.GAME);
    public void UpdateStateToPause() => UpdateGameState(GameState.PAUSE);
    public void UpdateStateToEnd() => UpdateGameState(GameState.END);

    public void ReloadScene()
    {
        DOTween.KillAll();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitApp() => Application.Quit();
}
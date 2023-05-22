using BaseTemplate.Behaviours;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UIManager : MonoSingleton<UIManager>
{

    [SerializeField] Canvas _mainCanvas;
    [SerializeField] MenuPanel _menuPanel;
    [SerializeField] SettingPanel _settingPanel;

    [SerializeField] GamePanel _gamePanel;
    [SerializeField] PausePanel _pausePanel;

    [SerializeField] EndGamePanel _endGamePanel;

    [SerializeField] BlankPanel _blankPanel;

    Panel _currentPanel;

    public MenuPanel MenuPanel { get => _menuPanel; }
    public GamePanel GamePanel { get => _gamePanel; }
    public SettingPanel SettingPanel { get => _settingPanel; }
    public PausePanel PausePanel { get => _pausePanel;}
    public EndGamePanel EndGamePanel { get => _endGamePanel;  }
    public BlankPanel BlankPanel { get => _blankPanel;  }
    public Canvas MainCanvas { get => _mainCanvas;  }

    public void Init()
    {
        GameManager.Instance.OnGameStateChanged += HandleStateChange;

        InitPanel();
    }

    public void InitPanel()
    {
        _menuPanel.Init();
        _gamePanel.Init();
        _endGamePanel.Init();
        _pausePanel.Init();
        _settingPanel.Init();
    }

    void ChangePanel(Panel newPanel, bool _isAddingCanvas = false)
    {
        if (newPanel == _currentPanel) return;

        if (_currentPanel != null)
        {
            if (_isAddingCanvas == false)
            {
                ClosePanel(_currentPanel);
            }
        }

        _currentPanel = newPanel;

        _currentPanel.gameObject.SetActive(true);
        _currentPanel.OpenPanel();
    }

    void ClosePanel(Panel newPanel)
    {
        newPanel.ClosePanel();
    }

    #region GameState

    void HandleStateChange(GameState newState)
    {
        switch (newState)
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

    }

    void HandleMenu()
    {
        ChangePanel(_menuPanel);
    }
    void HandleGame()
    {
        ChangePanel(_gamePanel);
    }
    void HandlePause()
    {
        ChangePanel(_pausePanel, true);
    }
    void HandleEnd()
    {
        ChangePanel(_endGamePanel);
    }
    void HandleWait()
    {
    }
    public void HandleRevive()
    {
        ChangePanel(_gamePanel);
    }

    public void HandleOpenSettings()
    {
        ChangePanel(_settingPanel, true);
    }

    public void HandleOpenBlank()
    {
        ChangePanel(_blankPanel);
    }

    #endregion

}

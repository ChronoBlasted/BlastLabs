using BaseTemplate.Behaviours;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadSceneUIManager : MonoSingleton<LoadSceneUIManager>
{
    [SerializeField] AuthentificationPanel _authentificationPanel;
    [SerializeField] LoadPanel _loadPanel;

    Panel _currentPanel;

    public AuthentificationPanel AuthentificationPanel { get => _authentificationPanel; }
    public LoadPanel LoadPanel { get => _loadPanel; }

    public void Init()
    {
        InitPanel();

        ChangePanel(_authentificationPanel);
    }

    public void InitPanel()
    {
        _authentificationPanel.Init();
        _loadPanel.Init();
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

    public void HandleOpenLoadPanel()
    {
        ChangePanel(_loadPanel);
    }
}

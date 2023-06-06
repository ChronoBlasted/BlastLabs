using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabBar : MonoBehaviour
{
    [SerializeField] List<TabBarButton> _tabBarButtons;
    [SerializeField] TabBarButton _startTabBarButton;

    TabBarButton _lastTabBarButton;

    public void Init()
    {
        foreach (TabBarButton tabBarButton in _tabBarButtons)
        {
            tabBarButton.Init();
        }

        _lastTabBarButton = _startTabBarButton;

        _lastTabBarButton.Open();
    }

    public void ChangeTab(TabBarButton newTabBarButton)
    {
        if (_lastTabBarButton == newTabBarButton) return;

        _lastTabBarButton.Close();

        _lastTabBarButton = newTabBarButton;

        _lastTabBarButton.Open();
    }
}

using DG.Tweening;
using TMPro;
using UnityEngine;

public class MenuPanel : Panel
{
    [SerializeField] TabBar _tabBar;
    public override void Init()
    {
        base.Init();

        _tabBar.Init();
    }

    public override void OpenPanel()
    {
        base.OpenPanel();
    }

    public override void ClosePanel()
    {
        base.ClosePanel();
    }

    public void FindMatchPVP() => NakamaManager.Instance.MatchmakingManager.FindMatch();
}

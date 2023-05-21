using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GamePanel : Panel
{
    [SerializeField] TMP_Text _playerScore, _opponentScore;
    [SerializeField] CanvasGroup _cardsCG;

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

    public void StartGame()
    {
    }

    public void DisablePlayerCards()
    {
        _cardsCG.alpha = .6f;
        _cardsCG.interactable = false;
        _cardsCG.blocksRaycasts = false;
    }

    public void ActivePlayerCards()
    {
        _cardsCG.alpha = 1;
        _cardsCG.interactable = true;
        _cardsCG.blocksRaycasts = true;
    }

    public void UpdateScore(int playerScore, int opponentScore)
    {
        _playerScore.text = playerScore.ToString();
        _opponentScore.text = opponentScore.ToString();
    }
}

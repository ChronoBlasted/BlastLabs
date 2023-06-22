using BaseTemplate.Behaviours;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public enum DIRECTION
{
    TOP,
    BOTTOM,
    LEFT,
    RIGHT
}

public class BoardManager : MonoSingleton<BoardManager>
{
    [SerializeField] Card _playerCard;
    [SerializeField] List<GameObject> _cardSpawnTransform;
    [SerializeField] List<BoardSlot> _boardSlots = new List<BoardSlot>();

    int _playerScore, _opponentScore;

    void ResetBoard()
    {
        foreach (var boardSlot in _boardSlots)
        {
            boardSlot.Init();
        }

        foreach (GameObject child in _cardSpawnTransform)
        {
            foreach (Transform c in child.transform)
            {
                Destroy(c.gameObject);
            }
        }

        UpdateTurnText("");

        UpdateScore();
    }

    public void StartMatch(List<CardData> playerCards)
    {
        ResetBoard();

        for (int i = 0; i < playerCards.Count; i++)
        {
            var card = Instantiate(_playerCard, _cardSpawnTransform[i].transform);
            card.Init(playerCards[i], true);
        }
    }

    public void UpdateTurnText(string TurnText)
    {
        UIManager.Instance.GamePanel.UpdateTurnText(TurnText);
    }

    public void DropCard(int position, int indexOfCard, bool isPlayerOrOponent)
    {
        BoardSlot currentSlot = _boardSlots[position];

        if (currentSlot.IsOccupied == false)
        {
            currentSlot.DropCard(indexOfCard, isPlayerOrOponent);
        }

        UpdateAdjacentCard(position, isPlayerOrOponent);
    }

    public void UpdateAdjacentCard(int position, bool isPlayerOrOponent)
    {
        Debug.Log("Update adjacent");

        switch (position)
        {
            case 0:
                CheckPower(isPlayerOrOponent, position, DIRECTION.RIGHT);
                CheckPower(isPlayerOrOponent, position, DIRECTION.BOTTOM);
                break;
            case 1:
                CheckPower(isPlayerOrOponent, position, DIRECTION.RIGHT);
                CheckPower(isPlayerOrOponent, position, DIRECTION.BOTTOM);
                CheckPower(isPlayerOrOponent, position, DIRECTION.LEFT);
                break;
            case 2:
                CheckPower(isPlayerOrOponent, position, DIRECTION.BOTTOM);
                CheckPower(isPlayerOrOponent, position, DIRECTION.LEFT);
                break;
            case 3:
                CheckPower(isPlayerOrOponent, position, DIRECTION.TOP);
                CheckPower(isPlayerOrOponent, position, DIRECTION.RIGHT);
                CheckPower(isPlayerOrOponent, position, DIRECTION.BOTTOM);
                break;
            case 4:
                CheckPower(isPlayerOrOponent, position, DIRECTION.TOP);
                CheckPower(isPlayerOrOponent, position, DIRECTION.RIGHT);
                CheckPower(isPlayerOrOponent, position, DIRECTION.BOTTOM);
                CheckPower(isPlayerOrOponent, position, DIRECTION.LEFT);
                break;
            case 5:
                CheckPower(isPlayerOrOponent, position, DIRECTION.TOP);
                CheckPower(isPlayerOrOponent, position, DIRECTION.BOTTOM);
                CheckPower(isPlayerOrOponent, position, DIRECTION.LEFT);
                break;
            case 6:
                CheckPower(isPlayerOrOponent, position, DIRECTION.TOP);
                CheckPower(isPlayerOrOponent, position, DIRECTION.RIGHT);
                break;
            case 7:
                CheckPower(isPlayerOrOponent, position, DIRECTION.TOP);
                CheckPower(isPlayerOrOponent, position, DIRECTION.RIGHT);
                CheckPower(isPlayerOrOponent, position, DIRECTION.LEFT);
                break;
            case 8:
                CheckPower(isPlayerOrOponent, position, DIRECTION.TOP);
                CheckPower(isPlayerOrOponent, position, DIRECTION.LEFT);
                break;
        }

        UpdateScore();
    }

    void CheckPower(bool isPlayerOrOponent, int position, DIRECTION direction)
    {
        switch (direction)
        {
            case DIRECTION.TOP:
                if (_boardSlots[position - 3].CurrentCard == null) return;
                if (_boardSlots[position - 3].CurrentCard.IsPlayerCard == isPlayerOrOponent && _boardSlots[position].CurrentCard.GetTopPower() > _boardSlots[position - 3].CurrentCard.GetBotPower()) _boardSlots[position - 3].CurrentCard.ChangeOwner();
                break;
            case DIRECTION.BOTTOM:
                if (_boardSlots[position + 3].CurrentCard == null) return;
                if (_boardSlots[position + 3].CurrentCard.IsPlayerCard == isPlayerOrOponent && _boardSlots[position].CurrentCard.GetBotPower() > _boardSlots[position + 3].CurrentCard.GetTopPower()) _boardSlots[position + 3].CurrentCard.ChangeOwner();
                break;
            case DIRECTION.LEFT:
                if (_boardSlots[position - 1].CurrentCard == null) return;
                if (_boardSlots[position - 1].CurrentCard.IsPlayerCard == isPlayerOrOponent && _boardSlots[position].CurrentCard.GetLeftPower() > _boardSlots[position - 1].CurrentCard.GetRightPower()) _boardSlots[position - 1].CurrentCard.ChangeOwner();
                break;
            case DIRECTION.RIGHT:
                if (_boardSlots[position + 1].CurrentCard == null) return;
                if (_boardSlots[position + 1].CurrentCard.IsPlayerCard == isPlayerOrOponent && _boardSlots[position].CurrentCard.GetRightPower() > _boardSlots[position + 1].CurrentCard.GetLeftPower()) _boardSlots[position + 1].CurrentCard.ChangeOwner();
                break;
        }
    }

    void UpdateScore()
    {
        _playerScore = 0;
        _opponentScore = 0;

        foreach (var slot in _boardSlots)
        {
            if (slot.CurrentCard != null)
            {
                if (slot.CurrentCard.IsPlayerCard) _playerScore++;
                if (!slot.CurrentCard.IsPlayerCard) _opponentScore++;
            }
        }

        UIManager.Instance.GamePanel.UpdateScore(_playerScore, _opponentScore);
    }
}

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
    [SerializeField] TMP_Text _turnTXT;
    [SerializeField] List<Card> _playerCard;
    [SerializeField] List<BoardSlot> _boardSlots = new List<BoardSlot>();

    int _playerScore, _opponentScore;
    public List<Card> PlayerCard { get => _playerCard; }
    public TMP_Text TurnTXT { get => _turnTXT; }

    public void Init()
    {
        foreach (var boardSlot in _boardSlots)
        {
            boardSlot.Init();
        }
    }

    public void UpdateTurnText(string TurnText)
    {
        _turnTXT.text = TurnText;
    }

    public bool DropCard(int position, int indexOfCard, bool isPlayerOrOponent)
    {
        BoardSlot currentSlot = _boardSlots[position];

        if (currentSlot.IsOccupied)
        {
            Debug.Log("Can't drop card here !");
            return false;
        }
        else
        {
            Debug.Log("UpdateBoard");

            currentSlot.DropCard(indexOfCard, isPlayerOrOponent);

            return true;
        }
    }

    public bool UpdateAdjacentCard(int position, bool isOpponentDropping)
    {
        Debug.Log("Update adjacent");

        switch (position)
        {
            case 0:
                CheckPower(isOpponentDropping, position, DIRECTION.RIGHT);
                CheckPower(isOpponentDropping, position, DIRECTION.BOTTOM);
                break;
            case 1:
                CheckPower(isOpponentDropping, position, DIRECTION.RIGHT);
                CheckPower(isOpponentDropping, position, DIRECTION.BOTTOM);
                CheckPower(isOpponentDropping, position, DIRECTION.LEFT);
                break;
            case 2:
                CheckPower(isOpponentDropping, position, DIRECTION.BOTTOM);
                CheckPower(isOpponentDropping, position, DIRECTION.LEFT);
                break;
            case 3:
                CheckPower(isOpponentDropping, position, DIRECTION.TOP);
                CheckPower(isOpponentDropping, position, DIRECTION.RIGHT);
                CheckPower(isOpponentDropping, position, DIRECTION.BOTTOM);
                break;
            case 4:
                CheckPower(isOpponentDropping, position, DIRECTION.TOP);
                CheckPower(isOpponentDropping, position, DIRECTION.RIGHT);
                CheckPower(isOpponentDropping, position, DIRECTION.BOTTOM);
                CheckPower(isOpponentDropping, position, DIRECTION.LEFT);
                break;
            case 5:
                CheckPower(isOpponentDropping, position, DIRECTION.TOP);
                CheckPower(isOpponentDropping, position, DIRECTION.BOTTOM);
                CheckPower(isOpponentDropping, position, DIRECTION.LEFT);
                break;
            case 6:
                CheckPower(isOpponentDropping, position, DIRECTION.TOP);
                CheckPower(isOpponentDropping, position, DIRECTION.RIGHT);
                break;
            case 7:
                CheckPower(isOpponentDropping, position, DIRECTION.TOP);
                CheckPower(isOpponentDropping, position, DIRECTION.RIGHT);
                CheckPower(isOpponentDropping, position, DIRECTION.LEFT);
                break;
            case 8:
                CheckPower(isOpponentDropping, position, DIRECTION.TOP);
                CheckPower(isOpponentDropping, position, DIRECTION.LEFT);
                break;
        }

        UpdateScore();

        return CheckIfBoardFull();
    }

    void CheckPower(bool isOpponentDropping, int position, DIRECTION direction)
    {

        switch (direction)
        {
            case DIRECTION.TOP:
                if (_boardSlots[position - 3].CurrentCard == null) return;
                if (_boardSlots[position - 3].CurrentCard.IsPlayerCard == isOpponentDropping && _boardSlots[position].CurrentCard.GetTopPower() > _boardSlots[position - 3].CurrentCard.GetBotPower()) _boardSlots[position - 3].CurrentCard.ChangeOwner();
                break;
            case DIRECTION.BOTTOM:
                if (_boardSlots[position + 3].CurrentCard == null) return;
                if (_boardSlots[position + 3].CurrentCard.IsPlayerCard == isOpponentDropping && _boardSlots[position].CurrentCard.GetBotPower() > _boardSlots[position + 3].CurrentCard.GetTopPower()) _boardSlots[position + 3].CurrentCard.ChangeOwner();
                break;
            case DIRECTION.LEFT:
                if (_boardSlots[position - 1].CurrentCard == null) return;
                if (_boardSlots[position - 1].CurrentCard.IsPlayerCard == isOpponentDropping && _boardSlots[position].CurrentCard.GetLeftPower() > _boardSlots[position - 1].CurrentCard.GetRightPower()) _boardSlots[position - 1].CurrentCard.ChangeOwner();
                break;
            case DIRECTION.RIGHT:
                if (_boardSlots[position + 1].CurrentCard == null) return;
                if (_boardSlots[position + 1].CurrentCard.IsPlayerCard == isOpponentDropping && _boardSlots[position].CurrentCard.GetRightPower() > _boardSlots[position + 1].CurrentCard.GetLeftPower()) _boardSlots[position + 1].CurrentCard.ChangeOwner();
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

    bool CheckIfBoardFull()
    {
        bool isBoardFull = true;
        foreach (var slot in _boardSlots)
        {
            if (slot.CurrentCard == null) isBoardFull = false;
        }

        bool hasPlayerWin;

        if (_playerScore > _opponentScore) hasPlayerWin = true;
        else hasPlayerWin = false;

        if (isBoardFull) NakamaManager.Instance.MatchManager.MatchEnd(hasPlayerWin);

        return isBoardFull;
    }
}

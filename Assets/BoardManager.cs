using BaseTemplate.Behaviours;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BoardManager : MonoSingleton<BoardManager>
{
    [SerializeField] TMP_Text _turnTXT;
    [SerializeField] List<Card> _playerCard;
    [SerializeField] List<BoardSlot> _boardSlots = new List<BoardSlot>();

    public List<Card> PlayerCard { get => _playerCard; }

    public void Init()
    {
        foreach (var boardSlot in _boardSlots)
        {
            boardSlot.Init();
        }
    }

    public void UpdateTurnText(string PlayerTurnName)
    {
        _turnTXT.text = PlayerTurnName + " turn's";
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

    public void UpdateAdjacentCard(int position, bool isOpponentDropping)
    {
        Debug.Log("Update adjacent");

        Debug.Log(_boardSlots[3].CurrentCard);
        switch (position)
        {
            case 0:
                if (_boardSlots[3].CurrentCard != null) if (_boardSlots[3].CurrentCard.IsPlayerCard == isOpponentDropping && _boardSlots[0].CurrentCard.GetBotPower() > _boardSlots[3].CurrentCard.GetTopPower()) _boardSlots[3].CurrentCard.ChangeOwner();
                if (_boardSlots[1].CurrentCard != null) if (_boardSlots[1].CurrentCard.IsPlayerCard == isOpponentDropping && _boardSlots[0].CurrentCard.GetRightPower() > _boardSlots[1].CurrentCard.GetLeftPower()) _boardSlots[1].CurrentCard.ChangeOwner();
                break;
            case 1:
                break;
            case 2:
                break;
            case 3:
                break;
            case 4:
                break;
            case 5:
                break;
            case 6:
                break;
            case 7:
                break;
            case 8:
                break;
        }

    }
}

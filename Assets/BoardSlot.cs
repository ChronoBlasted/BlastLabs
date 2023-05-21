using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardSlot : MonoBehaviour
{
    [SerializeField] ItemSlot _itemSlot;
    [SerializeField] bool _isOccupied;
    [SerializeField] Card _cardTemp;
    [SerializeField] Card _currentCard;

    public bool IsOccupied { get => _isOccupied; }
    public Card CurrentCard { get => _currentCard; }

    public void Init()
    {
        _isOccupied = false;
    }

    public void DropCard(int indexOfCard, bool isPlayerOrOponent)
    {
        _isOccupied = true;

        if (isPlayerOrOponent == false)
        {
            var cardData = CardManager.Instance.GetCardByID(indexOfCard);
            var newCard = Instantiate(_cardTemp, transform);

            newCard.Init(cardData, false);

            newCard.CardDrop(true);

            _currentCard = newCard;
        }
        else
        {
            _currentCard = _itemSlot.CurrentCard;
        }
    }
}

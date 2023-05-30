using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BoardSlot : MonoBehaviour, IDropHandler
{
    [SerializeField] Card _opponentCard;
    [SerializeField] int _position;

    Card _currentCard;
    bool _isOccupied;

    public bool IsOccupied { get => _isOccupied; }
    public Card CurrentCard { get => _currentCard; }

    public void Init()
    {
        _isOccupied = false;

        if (_currentCard != null)
        {
            Destroy(_currentCard.gameObject);
            _currentCard = null;
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (IsOccupied) return;

        if (eventData.pointerDrag != null)
        {
            if (NakamaManager.Instance.MatchManager.DropCard(_position, _currentCard.Data.ID) == false) return;

            _currentCard = eventData.pointerDrag.GetComponent<Card>();

            eventData.pointerDrag.transform.SetParent(transform, false);
            eventData.pointerDrag.GetComponent<RectTransform>().localPosition = Vector3.zero;

            _currentCard.CardDropped();
        }
    }


    public void DropCard(int indexOfCard, bool isPlayerOrOponent)
    {
        _isOccupied = true;

        if (isPlayerOrOponent == false)
        {
            var cardData = CardManager.Instance.GetCardByID(indexOfCard);
            var newCard = Instantiate(_opponentCard, transform);

            newCard.Init(cardData, false);

            newCard.CardDropped(true);

            _currentCard = newCard;
        }
        else
        {
            _currentCard = CurrentCard;
        }
    }
}

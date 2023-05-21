using UnityEngine;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, IDropHandler
{
    [SerializeField] Card _currentCard;
    [SerializeField] BoardSlot _boardSlot;
    [SerializeField] int position;

    public Card CurrentCard { get => _currentCard; }

    public void OnDrop(PointerEventData eventData)
    {
        if (_boardSlot.IsOccupied) return;

        if (eventData.pointerDrag != null)
        {
            _currentCard = eventData.pointerDrag.GetComponent<Card>();

            eventData.pointerDrag.transform.SetParent(transform, false);
            eventData.pointerDrag.GetComponent<RectTransform>().localPosition = Vector3.zero;

            NakamaManager.Instance.MatchManager.DropCard(position, _currentCard.Data.ID);

            _currentCard.CardDrop();
        }
    }

}

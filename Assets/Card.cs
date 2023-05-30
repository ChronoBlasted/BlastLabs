using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Card : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [SerializeField] bool _isPlayerCard;

    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] RectTransform rectTransform;

    [SerializeField] TMP_Text _name;
    [SerializeField] TMP_Text _topPower, _rightPower, _bottomPower, _leftPower;
    [SerializeField] Image _bg;

    bool _isDraggable;

    public CardData Data { get => _data; }
    public bool IsPlayerCard { get => _isPlayerCard; }


    //Cache
    CardData _data;
    float _mainScaleFactor;

    public void Init(CardData data, bool isPlayerCard)
    {
        _mainScaleFactor = UIManager.Instance.MainCanvas.scaleFactor;

        _data = data;
        _isDraggable = true;
        _isPlayerCard = isPlayerCard;

        _topPower.text = _data.TopPower.ToString();
        _rightPower.text = _data.RightPower.ToString();
        _bottomPower.text = _data.BotPower.ToString();
        _leftPower.text = _data.LeftPower.ToString();

        _name.text = _data.Name;
    }

    public void CardDropped(bool instant = false)
    {
        _isDraggable = false;
    }

    public void ChangeOwner()
    {
        Debug.Log("ChangeOwner");

        _isPlayerCard = !_isPlayerCard;

        if (_isPlayerCard) _bg.color = Color.blue;
        else _bg.color = Color.red;
    }

    public int GetBotPower()
    {
        return _data.BotPower;
    }

    public int GetTopPower()
    {
        return _data.TopPower;
    }
    public int GetLeftPower()
    {
        return _data.LeftPower;
    }

    public int GetRightPower()
    {
        return _data.RightPower;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_isDraggable == false) return;

        canvasGroup.alpha = .6f;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_isDraggable == false) return;

        rectTransform.anchoredPosition += eventData.delta / _mainScaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (_isDraggable == false) return;

        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        rectTransform.localPosition = Vector3.zero;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
    }
}

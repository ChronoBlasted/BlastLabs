using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    [SerializeField] bool _isPlayerCard;
    [SerializeField] DragDrop _dragDrop;
    CardData _data;

    [SerializeField] TMP_Text _name;
    [SerializeField] TMP_Text _topPower, _rightPower, _bottomPower, _leftPower;
    [SerializeField] Image _bg;

    public CardData Data { get => _data; }
    public bool IsPlayerCard { get => _isPlayerCard; }

    public void Init(CardData data, bool isPlayerCard)
    {
        _data = data;
        _dragDrop.enabled = true;
        _isPlayerCard = isPlayerCard;

        _topPower.text = _data.TopPower.ToString();
        _rightPower.text = _data.RightPower.ToString();
        _bottomPower.text = _data.BotPower.ToString();
        _leftPower.text = _data.LeftPower.ToString();

        _name.text = _data.Name;
    }

    public void CardDrop(bool instant = false)
    {
        if (instant) _dragDrop.enabled = false;
        _dragDrop.DesactiveScript = true;
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
}

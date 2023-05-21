using BaseTemplate.Behaviours;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardManager : MonoSingleton<CardManager>
{
    [SerializeField] List<CardData> _cardDatas;

    public List<CardData> GetRandomHand()
    {
        List<CardData> randomCardList = new List<CardData>();

        System.Random rng = new System.Random();

        randomCardList = _cardDatas.OrderBy(item => rng.Next()).Take(5).ToList();

        return randomCardList;
    }

    public CardData GetCardByID(int id)
    {
        foreach (CardData cardData in _cardDatas)
        {
            if (cardData.ID == id) return cardData;
        }

        Debug.Log("Card with ID : " + id + " doesn't exist");
        return null;
    }
}

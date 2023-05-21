using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCardData", menuName = "Blast Lab's/Create new Card Data", order = 1)]
public class CardData : ScriptableObject
{
    public int ID;
    public string Name;

    public int TopPower, BotPower, LeftPower, RightPower;
}

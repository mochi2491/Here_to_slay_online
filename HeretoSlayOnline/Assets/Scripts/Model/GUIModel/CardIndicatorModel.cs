using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CardIndicatorModel
{
    private bool isLarge;
    private int cardID;

    public CardIndicatorModel()
    {
        isLarge = false;
        cardID = 0;
    }

    public int GetID
    {
        get { return cardID; }
    }

    public bool IsLarge
    {
        get { return isLarge; }
    }

    private CardIndicatorModel(bool isLarge, int cardID)
    {
        this.isLarge = isLarge;
        this.cardID = cardID;
    }

    public CardIndicatorModel Indicate(bool isLarge, int cardID)
    {
        if (cardID < 0) throw new System.Exception("存在しないカードIDです");
        return new CardIndicatorModel(isLarge, cardID);
    }
}


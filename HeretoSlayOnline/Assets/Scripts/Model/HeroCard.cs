using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ヒーローエリアに出したカードのクラス
public class HeroCard
{
    public SmallCard hero = null;
    public SmallCard equip = null;

    public HeroCard(SmallCard hero, SmallCard equip)
    {
        this.hero = hero;
        this.equip = equip;
    }
}
public struct HeroCardData
{
    public int cardID;
    public int armedCardID;
}

//手札や捨て札にある状態のカード
public class SmallCard
{
    private int cardID = 0;
    private string cardEffect = "";

    public SmallCard(int cardID, string cardEffect)
    {
        this.cardID = cardID;
        this.cardEffect = cardEffect;
    }

    //setter and getter
    public int ID
    {
        get { return cardID; }
        set { cardID = value; }
    }
}

//モンスターカード
public class LargeCard
{
    private int cardID = 0;
    private string cardEffect = "";

    public LargeCard(int cardID, string cardEffect)
    {
        this.cardID = cardID;
        this.cardEffect = cardEffect;
    }

    public int ID
    {
        get { return cardID; }
        set { cardID = value; }
    }
}

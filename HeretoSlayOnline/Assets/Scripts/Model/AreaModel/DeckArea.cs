using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DeckArea
{
    private List<SmallCard> mainDeck = new List<SmallCard>();
    public List<SmallCard> discardPile = new List<SmallCard>();

    public void Init()
    {
        //mainDeck init
        for (int i = 0; i <= CardSprites.SMALLCARD_COUNT; i++)
        {
            if (i == 52 || i == 54 || i == 57 || (i >= 66 && i <= 69) || i == 72)
            { //2枚
                mainDeck.Add(new SmallCard(i, ""));
                mainDeck.Add(new SmallCard(i, ""));
            }
            else if (i == 60 || (i >= 62 && i <= 64))
            { //4枚
                for (int j = 0; j < 4; j++) mainDeck.Add(new SmallCard(i, ""));
            }
            else if (i == 61)
            { //9枚
                for (int k = 0; k < 9; k++) mainDeck.Add(new SmallCard(i, ""));
            }
            else if (i == 73)
            { //14枚
                for (int l = 0; l < 14; l++) mainDeck.Add(new SmallCard(i, ""));
            }
            else mainDeck.Add(new SmallCard(i, "")); //1枚
        }
        mainDeck = mainDeck.OrderBy(a => Guid.NewGuid()).ToList();
        //discardPile init
        discardPile.Clear();
    }

    public int deckCount()
    {
        return mainDeck.Count;
    }

    public DeckArea Shuffle()
    {
        mainDeck = mainDeck.OrderBy(a => Guid.NewGuid()).ToList();
        return this;
    }

    public void ApplyChanges(List<int> deckData, List<int> pileData)
    {
        mainDeck.Clear();
        foreach (int id in deckData)
        {
            mainDeck.Add(new SmallCard(id, ""));
        }
    }

    public List<int> DeckToData()
    {
        List<int> data = new List<int>();
        foreach (SmallCard card in mainDeck)
        {
            data.Add(card.ID);
        }
        return data;
    } //MainDeckをdataに

    public void DataToDeck(List<int> data)
    {
        mainDeck.Clear();
        foreach (int id in data)
        {
            mainDeck.Add(new SmallCard(id, ""));
        }
    } //dataをMainDeckに

    public List<int> PileToData()
    {
        List<int> data = new List<int>();
        foreach (SmallCard card in discardPile)
        {
            data.Add(card.ID);
        }
        return data;
    } //DiscardPileをdataに

    public void DataToPile(List<int> data)
    {
        discardPile.Clear();
        foreach (int id in data)
        {
            discardPile.Add(new SmallCard(id, ""));
        }
    } //dataをDiscardPileに

    public SmallCard PopDeck()
    {
        if (mainDeck.Count < 1)
        {
            Debug.Log("デッキがありません");
            return null;
        }
        SmallCard tmp = mainDeck[0];
        mainDeck.RemoveAt(0);
        return tmp;
    } //deckの頭を取り出す

    public void PushDeck(SmallCard tmp)
    {
        mainDeck.Insert(0, tmp);
    } //deckの頭にカードを追加

    public SmallCard PickDiscard(int order)
    {
        SmallCard tmp = discardPile[order];
        discardPile.RemoveAt(order);
        return tmp;
    } //discardPileの指定のカードを取り出す

    public void PushDiscard(SmallCard tmp)
    {
        discardPile.Add(tmp);
    } //discardPileの最後にカードを追加
}


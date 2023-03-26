using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MonsterArea
{
    public List<LargeCard> monsterCardList = new List<LargeCard>();
    private List<LargeCard> monsterDeck = new List<LargeCard>();

    public void Init()
    {
        //monsterDeck init
        for (int i = 0; i <= 14; i++) monsterDeck.Add(new LargeCard(i + 6, ""));
        monsterDeck = monsterDeck.OrderBy(a => Guid.NewGuid()).ToList();
        //monsterCardList init
        monsterCardList.Clear();
    }

    public List<int> ListToData()
    {
        List<int> data = new List<int>();
        foreach (LargeCard card in monsterCardList)
        {
            data.Add(card.ID);
        }
        return data;
    } //現在出現しているモンスターのリストをdataに

    public void DataToList(List<int> data)
    {
        monsterCardList.Clear();
        foreach (int id in data)
        {
            monsterCardList.Add(new LargeCard(id, ""));
        }
    } //dataを現在出現しているモンスターのリストに

    public List<int> DeckToData()
    {
        List<int> data = new List<int>();
        foreach (LargeCard card in monsterDeck)
        {
            data.Add(card.ID);
        }
        return data;
    } //MonsterDeckをdataに

    public void DataToDeck(List<int> data)
    {
        monsterDeck.Clear();
        foreach (int id in data)
        {
            monsterDeck.Add(new LargeCard(id, ""));
        }
    } //dataをMonsterDeckに

    public LargeCard PopDeck()
    {
        LargeCard tmp = monsterDeck[0];
        monsterDeck.RemoveAt(0);
        return tmp;
    } //monsterDeckの先頭を取り出す

    public void PushDeck(LargeCard tmp)
    {
        monsterDeck.Insert(0, tmp);
    } //monsterDeckの頭にカードを追加

    public LargeCard PopList(int order)
    {
        LargeCard tmp = monsterCardList[order];
        monsterCardList.RemoveAt(order);
        return tmp;
    } //monsterListからカードを取り出す

    public void PushList(LargeCard tmp, int order)
    {
        if (monsterCardList.Count < 3) monsterCardList.Add(tmp);
        else
        {
            PushDeck(monsterCardList[order]);
            monsterCardList.RemoveAt(order);
            monsterCardList.Add(tmp);
        }
    } //monsterListが3枚未満ならカードを追加する、3枚以上なら一枚戻してから追加する
}


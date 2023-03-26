using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
public class PlayerArea
{

    //instance
    private string userName = "";

    public ReactiveProperty<int> leaderCardID = new ReactiveProperty<int>(0);
    public IReactiveProperty<int> _leaderCardID => leaderCardID;
    private List<SmallCard> playerHandList = new List<SmallCard>();
    private List<HeroCard> playerHeroCardList = new List<HeroCard>();
    private List<LargeCard> slayedMonsterList = new List<LargeCard>();

    //pull no model
    public int pullNum = 0;

    //getter and setter
    public String UserName
    {
        get { return this.userName; }
        set { this.userName = value; }
    }

    public List<SmallCard> PlayerHandList
    {
        get { return playerHandList; }
    }

    public List<HeroCard> PlayerHeroCardList
    {
        get { return playerHeroCardList; }
    }

    public List<LargeCard> SlayedMonsterList
    {
        get { return slayedMonsterList; }
    }

    public PlayerArea()
    {
    }

    public PlayerArea(PlayerData playerData)
    {
        this.userName = playerData.playerID;
        this.leaderCardID.Value = playerData.leaderCardID;
        DataToHand(playerData.playerHandList);
        DataToHeroList(playerData.playerHeroCardList);
        DataToSlayedList(playerData.slayedMonsterList);
    } //コンストラクタ

    //method
    public void Init(int num)
    {
        leaderCardID.Value = num;
        playerHandList.Clear();
        playerHeroCardList.Clear();
    } //初期化

    public PlayerData PlayerAreaToData()
    {
        PlayerData playerData = new PlayerData();
        playerData.playerID = this.userName;
        playerData.leaderCardID = leaderCardID.Value;
        playerData.playerHandList = HandToData();
        playerData.playerHeroCardList = HeroListToData();
        playerData.slayedMonsterList = SlayedListToData();
        return playerData;
    } //プレイヤーの情報をdataに

    public void DataToPlayerArea(PlayerData playerData)
    {
        this.userName = playerData.playerID;
        this.leaderCardID.Value = playerData.leaderCardID;
        DataToHand(playerData.playerHandList);
        DataToHeroList(playerData.playerHeroCardList);
        DataToSlayedList(playerData.slayedMonsterList);
    } //dataをプレイヤーの情報に

    public List<int> HandToData()
    {
        List<int> data = new List<int>();
        foreach (SmallCard card in playerHandList)
        {
            data.Add(card.ID);
        }
        return data;
    } //手札をdataに

    public void DataToHand(List<int> data)
    {
        playerHandList.Clear();
        foreach (int id in data)
        {
            playerHandList.Add(new SmallCard(id, ""));
        }
    } //dataを手札に

    public List<HeroCardData> HeroListToData()
    {
        List<HeroCardData> data = new List<HeroCardData>();
        foreach (HeroCard card in playerHeroCardList)
        {
            HeroCardData cardData = new HeroCardData();
            cardData.cardID = card.hero.ID;
            if (card.equip != null)
            {
                cardData.armedCardID = card.equip.ID;
            }
            else cardData.armedCardID = -1;

            data.Add(cardData);
        }
        return data;
    } //ヒーローリストをdataに

    public void DataToHeroList(List<HeroCardData> data)
    {
        playerHeroCardList.Clear();
        foreach (HeroCardData hero in data)
        {
            if (hero.armedCardID != -1)
            {
                playerHeroCardList.Add(new HeroCard(new SmallCard(hero.cardID, ""), new SmallCard(hero.armedCardID, "")));
            }
            else
            {
                playerHeroCardList.Add(new HeroCard(new SmallCard(hero.cardID, ""), new SmallCard(-1, "")));
            }
        }
    } //dataをヒーローリストに

    public List<int> SlayedListToData()
    {
        List<int> data = new List<int>();
        foreach (LargeCard card in slayedMonsterList)
        {
            data.Add(card.ID);
        }
        return data;
    } //倒したモンスターをdataに

    public void DataToSlayedList(List<int> data)
    {
        slayedMonsterList.Clear();
        foreach (int id in data)
        {
            slayedMonsterList.Add(new LargeCard(id, ""));
        }
    } //dataを倒したモンスターに

    public SmallCard PickHand(int order)
    {
        SmallCard tmp = playerHandList[order];
        playerHandList.RemoveAt(order);
        return tmp;
    } //手札から一枚カードを取り出す

    public void PushHand(SmallCard tmp)
    {
        playerHandList.Add(tmp);
    }//カードを手札に追加する

    public HeroCard PickHeroCard(int order)
    {
        Debug.Log(playerHeroCardList.Count + "," + order);
        HeroCard tmp = playerHeroCardList[order];
        playerHeroCardList.RemoveAt(order);
        return tmp;
    }//カードリストの中からカードを一枚取り出す

    public void PushHeroCard(HeroCard tmp)
    {
        playerHeroCardList.Add(tmp);
    }//ヒーローリストにカードを追加する

    public SmallCard PickArmedCard(int order)
    {
        SmallCard tmp = playerHeroCardList[order].equip;
        playerHeroCardList[order].equip = null;
        return tmp;
    } //ヒーローが装備しているアイテムを取り出す

    public void AttachArmedCard(SmallCard tmp, int order)
    {
        Debug.Log(playerHeroCardList.Count + "," + order);
        if (order >= playerHeroCardList.Count) throw new Exception("heroが存在しません");
        playerHeroCardList[order].equip = tmp;
    } //ヒーローにアイテムを装備させる

    public LargeCard PickSlayedMonster(int order)
    {
        LargeCard tmp = slayedMonsterList[order];
        slayedMonsterList.RemoveAt(order);
        return tmp;
    } //討伐したモンスターリストからカードを一枚取り出す

    public void PushSlayedMonster(LargeCard tmp)
    {
        slayedMonsterList.Add(tmp);
    } //討伐したモンスターリストにカードを追加する

    public void SetLeaderID(int num)
    {
        leaderCardID.Value = num;
    }
}
public struct PlayerData
{
    public string playerID;
    public int leaderCardID;
    public List<int> playerHandList;
    public List<HeroCardData> playerHeroCardList;
    public List<int> slayedMonsterList;
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
public struct GameBoardData
{
    public List<int> discardPile;
    public List<int> mainDeck;

    public List<int> monsterCardList;
    public List<int> monsterDeck;

    public string chatLog;

    public int turnPlayerNum;
    public int playerCount;
    public List<PlayerData> playerList;

    public int selectedID;
}


public interface IGameBoard
{
}

public class GameBoard : IGameBoard
{

    //定数
    private Sprite cardBack;

    private int playerCount = 0;
    private IntReactiveProperty playerID = new IntReactiveProperty(0);
    private List<String> userNameList = new List<String>();
    public IReadOnlyReactiveProperty<int> _playerID => playerID;
    private DeckArea deckArea = new DeckArea();
    public MonsterArea monsterArea = new MonsterArea();
    public ChatArea chatArea = new ChatArea();
    public List<PlayerArea> playerAreaList = new List<PlayerArea>();

    //setter and getter
    public int PlayerID
    {
        set
        {
            playerID.Value = value;
        }
        get
        {
            return playerID.Value;
        }
    }

    public int PlayerCount
    {
        set
        {
            this.playerCount = value;
        }
        get
        {
            return this.playerCount;
        }
    }

    public DeckArea GetdeckArea
    {
        get { return this.deckArea; }
    }

    public MonsterArea GetmonsterArea
    {
        get { return this.monsterArea; }
    }

    public GameBoard InitializeGameBoard()
    {
        for (int i = 0; i < 6; i++) playerAreaList.Add(new PlayerArea());
        cardBack = Resources.Load("back") as Sprite;
        deckArea.Init();
        monsterArea.Init();
        chatArea.Init();
        foreach (PlayerArea pa in playerAreaList)
        {
            pa.Init(0);
        }
        return this;
    } //初期化

    public GameBoard ApplyNewBoard(GameBoardData gbd, GameBoardView gbv)
    {
        GameBoard board = (GameBoard)MemberwiseClone();

        board.deckArea.DataToDeck(gbd.mainDeck);
        board.deckArea.DataToPile(gbd.discardPile);
        board.monsterArea.DataToDeck(gbd.monsterDeck);
        board.monsterArea.DataToList(gbd.monsterCardList);
        board.chatArea.ApplyLog(gbd.chatLog);
        board.DataToPlayerList(gbd.playerList);

        return board;
    } //GameBoardを更新する

    public GameBoardData GameBoardToData(GameBoard gb)
    {
        GameBoardData gbd = new GameBoardData();
        gbd.turnPlayerNum = 1;
        gbd.playerCount = gb.playerCount;
        gbd.mainDeck = gb.deckArea.DeckToData();
        gbd.discardPile = gb.deckArea.PileToData();
        gbd.monsterCardList = gb.monsterArea.ListToData();
        gbd.monsterDeck = gb.monsterArea.DeckToData();
        gbd.playerList = gb.PlayerListToData();
        gbd.chatLog = gb.chatArea._chatLog.Value;
        return gbd;
    }//GameBoardのモデルをデータに変換する

    public List<PlayerData> PlayerListToData()
    {
        List<PlayerData> data = new List<PlayerData>();
        foreach (PlayerArea a in playerAreaList)
        {
            data.Add(a.PlayerAreaToData());
        }
        return data;
    } //playerAreaListをdataに

    public void DataToPlayerList(List<PlayerData> playerList)
    {
        playerAreaList.Clear();
        foreach (PlayerData pd in playerList)
        {
            playerAreaList.Add(new PlayerArea(pd));
        }
    } //dataをplayerAreaListに

    public List<String> GetUserNameList()
    {
        List<String> userNameList = new List<String>();
        foreach (PlayerArea player in playerAreaList)
        {
            userNameList.Add(player.UserName);
        }
        return userNameList;
    } //userNameのListを返す

    public GameBoard DeckShuffle()
    {
        GameBoard board = (GameBoard)MemberwiseClone();
        board.deckArea.Shuffle();
        return this;
    }

    public GameBoard AddLog(string text)
    {
        GameBoard board = (GameBoard)MemberwiseClone();
        board.chatArea.AddLog(text);
        return board;
    }

    public GameBoard SetLeaderNum(int playerNum, int num)
    {
        GameBoard board = (GameBoard)MemberwiseClone();
        board.playerAreaList[playerNum].SetLeaderID(num);
        return board;
    }

    public GameBoard ControlBoard(GameBoardAddress From, GameBoardAddress To)
    {
        bool isLarge = false;
        string logText = "";
        GameBoard copy = (GameBoard)MemberwiseClone();
        if ((From.area == Area.monsterList) || (From.area == Area.monsterDeck) || (From.area == Area.slayedMonster))
        {
            isLarge = true;
        }
        //カードを移動させる
        if (isLarge)
        {
            LargeCard moveCard;
            switch (From.area)
            {
                case Area.monsterList:
                    moveCard = copy.monsterArea.PopList(From.order);
                    break;

                case Area.monsterDeck:
                    moveCard = copy.monsterArea.PopDeck();
                    break;

                case Area.slayedMonster:
                    moveCard = copy.playerAreaList[From.playerID].PickSlayedMonster(From.order);
                    break;

                default:
                    moveCard = null;
                    break;
            }
            switch (To.area)
            {
                case Area.monsterList:
                    copy.monsterArea.PushList(moveCard, To.order);
                    break;

                case Area.monsterDeck:
                    copy.monsterArea.PushDeck(moveCard);
                    break;

                case Area.slayedMonster:
                    copy.playerAreaList[To.playerID].PushSlayedMonster(moveCard);
                    break;
            }
            logText = "id:" + moveCard.ID + " " + "From:" + From.area + ",player:" + From.playerID + "To:" + To.area + ",player" + To.playerID;
        }
        else
        {
            SmallCard moveCard;
            switch (From.area)
            {
                case Area.discardPile:
                    moveCard = copy.deckArea.PickDiscard(From.order);
                    break;

                case Area.deck:
                    moveCard = copy.deckArea.PopDeck();
                    break;

                case Area.playerHand:
                    moveCard = copy.playerAreaList[From.playerID].PickHand(From.order);
                    break;

                case Area.playerHeroItem:
                    moveCard = copy.playerAreaList[From.playerID].PickArmedCard(From.order);
                    break;

                case Area.playerHero:
                    HeroCard a = copy.playerAreaList[From.playerID].PickHeroCard(From.order);
                    moveCard = a.hero;
                    if (a.equip.ID != -1) copy.playerAreaList[playerID.Value].PushHand(a.equip);
                    break;

                default:
                    moveCard = null;
                    break;
            }
            switch (To.area)
            {
                case Area.discardPile:
                    copy.deckArea.PushDiscard(moveCard);
                    break;

                case Area.deck:
                    copy.deckArea.PushDeck(moveCard);
                    break;

                case Area.playerHand:
                    copy.playerAreaList[To.playerID].PushHand(moveCard);
                    break;

                case Area.playerHeroItem:
                    copy.playerAreaList[To.playerID].AttachArmedCard(moveCard, To.order);
                    break;

                case Area.playerHero:
                    HeroCard a = new HeroCard(moveCard, null);
                    copy.playerAreaList[To.playerID].PushHeroCard(a);
                    break;
            }
            logText = "id:" + moveCard.ID + " " + "From:" + From.area + ",player:" + From.playerID + "To:" + To.area + ",player" + To.playerID;
        }
        chatArea.AddLog(logText);

        return copy;
    } //カードを移動させるあまりよくない実装なのでいずれなおす

    public GameBoardAddress SearchCard(int cardID, bool isLarge)
    {
        int playerNum = 0;
        int orderNum = 0;
        GameBoardAddress address = new GameBoardAddress();
        if (isLarge)
        {
            address.area = Area.slayedMonster;
            playerNum = 0;
            //slayedMonsterListから探索
            foreach (PlayerArea pa in playerAreaList)
            {
                address.playerID = playerNum;
                orderNum = 0;
                foreach (LargeCard card in pa.SlayedMonsterList)
                {
                    address.order = orderNum;
                    if (card.ID == cardID)
                    {
                        return address;
                    }
                    orderNum++;
                }
                playerNum++;
            }
            //monsterListから探索
            address.area = Area.monsterList;
            playerNum = 0;
            orderNum = 0;
            foreach (LargeCard card in monsterArea.monsterCardList)
            {
                address.order = orderNum;
                if (card.ID == cardID)
                {
                    return address;
                }
                orderNum++;
            }
        }
        else
        {
            foreach (PlayerArea pa in playerAreaList)
            {
                orderNum = 0;
                address.playerID = playerNum;
                address.area = Area.playerHand;
                foreach (SmallCard card in pa.PlayerHandList)
                {
                    address.order = orderNum;
                    if (card.ID == cardID) return address;
                    orderNum++;
                }
                address.area = Area.playerHero;
                orderNum = 0;
                foreach (HeroCard card in pa.PlayerHeroCardList)
                {
                    address.order = orderNum;
                    if (card.hero.ID == cardID) return address;
                    orderNum++;
                }
                orderNum = 0;
                address.area = Area.playerHeroItem;
                foreach (HeroCard card in pa.PlayerHeroCardList)
                {
                    address.order = orderNum;
                    if (card.equip.ID == cardID) return address;
                    orderNum++;
                }
                playerNum++;
            }
            playerNum = 0;
            orderNum = 0;
            address.area = Area.discardPile;
            foreach (SmallCard card in deckArea.discardPile)
            {
                address.order = orderNum;
                if (card.ID == cardID) return address;
                orderNum++;
            }
        }
        return address;
    }
}
public struct GameBoardAddress
{
    public Area area;
    public int playerID;
    public int order;
}

public enum Area
{
    deck, discardPile, monsterList, monsterDeck, playerHand, playerHero, playerHeroItem, slayedMonster
}

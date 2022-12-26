using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using WebSocketSharp.Server;
using UniRx;
using Newtonsoft.Json;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System.Threading;

public class GameCore : SingletonMonoBehaviour<GameCore>
{
    //サーバとの通信を行う
    public ServerConnector connector;

    //GameBoardの実体
    GameBoard gameBoard = new GameBoard();
    public GameBoardView gameBoardView = new GameBoardView();
    public GameObject gameBoardObject;

    //Gameのステート
    private ReactiveProperty<GameState> state = new ReactiveProperty<GameState>(GameState.entrance);

    //Entrance
    public GameObject entrance;
    private string userName="";
    private bool isReady = false;

    //GUI
    public GameObject[] tabs;
    private IntReactiveProperty visibleTabNum = new IntReactiveProperty(0);
    
    
    void Start()
    {
        connector = this.gameObject.AddComponent<ServerConnector>();
        gameBoardView = this.gameObject.GetComponent<GameBoardView>();

        visibleTabNum.Subscribe(
            x => {
                ApplyVisibleTab(x);
            }
        );
        connector._receivedMessage.Subscribe(
            x => {
                GetMessage(x);
            }
        );
    }
    void Update()
    {
        
    }

    //tab切り替え
    private void ApplyVisibleTab(int num) {
        for(int i = 0; i < 8; i++) {
            if (i == num) tabs[i].SetActive(true);
            else tabs[i].SetActive(false);
        }
    }
    public void SetVisibleTabNum(int num) {
        visibleTabNum.Value = num;
    }
    
    //entrance
    public void ApplyUserName(string text) {
        userName = text;
    }
    public void SendUserName() {
        if (state.Value == GameState.entrance) {
            connector.SendText("0:::" + userName);
            state.Value = GameState.wait;
        }
    }
    public void ApplyIsReady(bool a) {
        isReady = a;
        if (isReady) connector.SendText("1:::1");
        else connector.SendText("1:::0");
    }

    private void GetMessage(string msg) {
        string[] message = msg.Split(":::");
        if (message[0] == "0") {//0:::playerNum:::userName
            if (state.Value != GameState.entrance) return;
            gameBoard.PlayerID = int.Parse(message[1]); //playerのIDを設定
        }
        else if (message[0] == "1") { //1:::playerCount 
            if (state.Value != GameState.wait) return;
            gameBoard.PlayerCount = int.Parse(message[1]);
            if (gameBoard.PlayerID == 1) {
                gameBoard.InitializeGameBoard();
            }
            entrance.SetActive(false);
            gameBoardObject.SetActive(true);
            state.Value = GameState.ingame;
        }
        else if (message[0] == "2") { //2:::json
            if (state.Value != GameState.ingame) return;

            //受け取ったjsonをクラスに変換しGameBoardに適用
            gameBoard.ApplyNewBoard(JsonToGameBoard(message[1]),gameBoardView);
        }
        else if (message[0] == "first") {
        }
        else {
            Debug.Log("received wrong message;");
        }
    }
    private string GameBoardToJson(GameBoardData gbd) { 
        return JsonConvert.SerializeObject(gbd);
    }
    private GameBoardData JsonToGameBoard(string json) {
        return JsonConvert.DeserializeObject<GameBoardData>(json);
    }
}
public enum GameState {
    entrance,wait,ingame
}
public class GameBoard : MonoBehaviour {
    private Sprite[] smallCardImageList = new Sprite[73];
    private Sprite[] largeCardImageList = new Sprite[20];
    private Sprite cardBack;

    private int turnPlayerNum = 0;
    private int playerCount = 0;
    private int playerID = 0;
    private DeckArea deckArea = new DeckArea();
    private MonsterArea monsterArea = new MonsterArea();
    private ChatArea chatArea = new ChatArea();
    private List<PlayerArea> playerAreaList = new List<PlayerArea>();

    //setter and getter
    public int PlayerID {
        set {
            playerID = value;
        }
        get {
            return playerID;
        }
    }
    public int PlayerCount {
        set {
            this.playerCount = value;
        }
        get {
            return this.playerCount;
        }
    }
    private void Start() {
        //スプライトの読込
        smallCardImageList = Resources.LoadAll("deck_cards") as Sprite[];
        largeCardImageList = Resources.LoadAll("monster_and_leader_cards") as Sprite[];
        cardBack = Resources.Load("back") as Sprite;
        InitializeGameBoard();
    }
    public GameBoard InitializeGameBoard() {
        deckArea.Init();
        monsterArea.Init();
        chatArea.Init();
        foreach (PlayerArea pa in playerAreaList) {
            pa.Init(0);
        }
        return this;
    } //初期化
    public void ApplyNewBoard(GameBoardData gbd,GameBoardView gbv) {
        deckArea.DataToDeck(gbd.mainDeck);
        deckArea.DataToPile(gbd.discardPile);
        monsterArea.DataToDeck(gbd.monsterDeck);
        monsterArea.DataToList(gbd.monsterCardList);
        DataToPlayerList(gbd.playerList);
        ApplyView(gbd,gbv);
    } //GameBoardを更新する
    public void ApplyView(GameBoardData gbd,GameBoardView gbv) {
        gbv.ApplyHand(gbd.playerList[playerID].playerHandList,smallCardImageList); //手札にデータを適用
        for(int i = 0; i < playerCount; i++) {
            gbv.ApplyHero(gbd.playerList[i].playerHeroCardList,smallCardImageList,i);
            gbv.ApplySlayedMonster(gbd.playerList[i].slayedMonsterList,largeCardImageList,i);
        } //ヒーローリストにデータを適用
        gbv.ApplyDiscardPile(gbd.discardPile, smallCardImageList);
        gbv.ApplyMonster(gbd.monsterCardList, largeCardImageList);
        
    } //更新したGameBoardを画面に適用する
    public GameBoardData GameBoardToData(GameBoard gb) {
        GameBoardData gbd = new GameBoardData();
        gbd.turnPlayerNum = 1;
        gbd.playerCount = gb.playerCount;
        gbd.mainDeck = gb.deckArea.DeckToData();
        gbd.discardPile = gb.deckArea.PileToData();
        gbd.monsterCardList = gb.monsterArea.ListToData();
        gbd.monsterDeck = gb.monsterArea.DeckToData();
        gbd.playerList = gb.PlayerListToData();
        gbd.chatLog = gb.chatArea.ChatLog;
        return gbd;
    }//GameBoardのモデルをデータに変換する
    public List<PlayerData> PlayerListToData() {
        List<PlayerData> data = new List<PlayerData>();
        foreach(PlayerArea a in playerAreaList) {
            data.Add(a.PlayerAreaToData());
        }
        return data;
    } //playerAreaListをdataに
    public void DataToPlayerList(List<PlayerData> playerList) {
        playerAreaList.Clear();
        foreach(PlayerData pd in playerList) {
            playerAreaList.Add(new PlayerArea(pd));
        }
    } //dataをplayerAreaListに
    public GameBoard ControlBoard(GameBoardAddress From,GameBoardAddress To) {
        bool isLarge = false;
        if((From.area == Area.monsterList) && (From.area == Area.discardPile) && (From.area == Area.slayedMonster)) {
            isLarge = true;
        }
        //カードをいどうさせる
        if (isLarge) {
            LargeCard moveCard;
            switch (From.area) {
                case Area.monsterList:
                    moveCard = monsterArea.PopList(From.order);
                    break;
                case Area.monsterDeck:
                    moveCard = monsterArea.PopDeck();
                    break;
                case Area.slayedMonster:
                    moveCard = playerAreaList[From.playerID].PickSlayedMonster(From.order);
                    break;
                default:
                    moveCard = null;
                    break;
            }
            switch (To.area) {
                case Area.monsterList:
                    monsterArea.PushList(moveCard);
                    break;
                case Area.monsterDeck:
                    monsterArea.PushDeck(moveCard);
                    break;
                case Area.slayedMonster:
                    playerAreaList[To.playerID].PushSlayedMonster(moveCard);
                    break;
            }
        }
        else {
            SmallCard moveCard;
            switch (From.area) {
                case Area.discardPile:
                    moveCard = deckArea.PickDiscard(From.order);
                    break;
                case Area.deck:
                    moveCard = deckArea.PopDeck();
                    break;
                case Area.playerHand:
                    moveCard = playerAreaList[From.playerID].PickHand(From.order);
                    break;
                case Area.playerHeroItem:
                    moveCard = playerAreaList[From.playerID].PickArmedCard(From.order);
                    break;
                case Area.playerHero:
                    moveCard = playerAreaList[From.playerID].PickHeroCard(From.order);
                    break;
                default:
                    moveCard = null;
                    break;
            }
            switch (To.area) {
                case Area.discardPile:
                    deckArea.PushDiscard(moveCard);
                    break;
                case Area.deck:
                    deckArea.PushDeck(moveCard);
                    break;
                case Area.playerHand:
                    playerAreaList[To.playerID].PushHand(moveCard);
                    break;
                case Area.playerHeroItem:
                    playerAreaList[To.playerID].AttachArmedCard(moveCard,To.order);
                    break;
                case Area.playerHero:
                    playerAreaList[To.playerID].PushHeroCard(moveCard);
                    break;
            }
        }
        return this;
    } //カードを移動させる(バカすぎる実装なのでいずれ直す)
}
public struct GameBoardAddress {
    public Area area;
    public int playerID;
    public int order;
}
public enum Area {
    discardPile,deck,monsterList,monsterDeck,playerHand,playerHero,playerHeroItem,slayedMonster
}
public class DeckArea : MonoBehaviour {
    private List<SmallCard> mainDeck = new List<SmallCard>();
    private List<SmallCard> discardPile = new List<SmallCard>();
    public void Init() {
        //mainDeck init
        for(int i = 0; i <= 73; i++) {
            if (i < 60) {
                if (i == 52 || i == 54 || i == 57) {
                    mainDeck.Add(new SmallCard(i, ""));
                    mainDeck.Add(new SmallCard(i, ""));
                }
                else mainDeck.Add(new SmallCard(i, ""));
            }
            else if (i>=60 && i < 65) {
                if (i == 61) {
                    for (int j = 0; j < 9; j++) mainDeck.Add(new SmallCard(i, ""));
                }
                else {
                    for (int k = 0; k < 4; k++) mainDeck.Add(new SmallCard(i, ""));
                }
            }
            else if (i >= 65 && i < 73) {
                if (i == 66 && i == 67 && i == 68 && i == 69 && i == 72) {
                    mainDeck.Add(new SmallCard(i, ""));
                    mainDeck.Add(new SmallCard(i, ""));
                }
                else mainDeck.Add(new SmallCard(i, ""));
            }
            else if (i == 73) {
                for(int l = 0; l < 14; l++) {
                    mainDeck.Add(new SmallCard(i, ""));
                }
            }
        }
        mainDeck = mainDeck.OrderBy(a => Guid.NewGuid()).ToList();
        //discardPile init
        discardPile.Clear();
    }
    public void ApplyChanges(List<int>deckData,List<int>pileData) {
        mainDeck.Clear();
        foreach (int id in deckData) {
            mainDeck.Add(new SmallCard(id, ""));
        }
    }
    public List<int> DeckToData() { 
        List<int> data = new List<int>();
        foreach(SmallCard card in mainDeck) {
            data.Add(card.ID);
        }
        return data;
    } //MainDeckをdataに
    public void DataToDeck(List<int> data) {
        mainDeck.Clear();
        foreach (int id in data) {
            mainDeck.Add(new SmallCard(id, ""));
        }
    } //dataをMainDeckに
    public List<int> PileToData() {
        List<int> data = new List<int>();
        foreach(SmallCard card in discardPile) {
            data.Add(card.ID);
        }
        return data;
    } //DiscardPileをdataに
    public void DataToPile(List<int> data) {
        discardPile.Clear();
        foreach(int id in data) {
            mainDeck.Add(new SmallCard(id, ""));
        }
    } //dataをDiscardPileに
    public SmallCard PopDeck() {
        SmallCard tmp = mainDeck[0];
        mainDeck.RemoveAt(0);
        return tmp;
    } //deckの頭を取り出す
    public void PushDeck(SmallCard tmp) {
        mainDeck.Insert(0, tmp);
    } //deckの頭にカードを追加
    public SmallCard PickDiscard(int order) {
        SmallCard tmp = discardPile[order];
        mainDeck.RemoveAt(order);
        return tmp;
    } //discardPileの指定のカードを取り出す
    public void PushDiscard(SmallCard tmp) {
        discardPile.Add(tmp);
    } //discardPileの最後にカードを追加
}
public class MonsterArea : MonoBehaviour {
    private List<LargeCard> monsterCardList = new List<LargeCard>();
    private List<LargeCard> monsterDeck = new List<LargeCard>();
    public void Init() {
        //monsterDeck init
        for (int i = 0; i <= 20; i++)monsterDeck.Add(new LargeCard(i,""));
        monsterDeck = monsterDeck.OrderBy(a => Guid.NewGuid()).ToList();
        //monsterCardList init
        monsterCardList.Clear();
    }
    public List<int> ListToData() {
        List<int> data = new List<int>();
        foreach(LargeCard card in monsterCardList) {
            data.Add(card.ID);
        }
        return data;
    } //現在出現しているモンスターのリストをdataに
    public void DataToList(List<int> data) {
        monsterCardList.Clear();
        foreach(int id in data) {
            monsterCardList.Add(new LargeCard(id, ""));
        }
    } //dataを現在出現しているモンスターのリストに
    public List<int> DeckToData() {
        List<int> data = new List<int>();
        foreach(LargeCard card in monsterDeck) {
            data.Add(card.ID);
        }
        return data;
    } //MonsterDeckをdataに
    public void DataToDeck(List<int> data) {
        monsterDeck.Clear();
        foreach(int id in data) {
            monsterDeck.Add(new LargeCard(id, ""));
        }
    } //dataをMonsterDeckに
    public LargeCard PopDeck() {
        LargeCard tmp = monsterDeck[0];
        monsterDeck.RemoveAt(0);
        return tmp;
    } //monsterDeckの先頭を取り出す
    public void PushDeck(LargeCard tmp) {
        monsterDeck.Insert(0, tmp);
    } //monsterDeckの頭にカードを追加
    public LargeCard PopList(int order) {
        LargeCard tmp = monsterCardList[order];
        monsterDeck.RemoveAt(order);
        PushList(PopDeck());
        return tmp;
    } //monsterListからカードを取り出す,またデッキから一枚リストに追加する
    public void PushList(LargeCard tmp) {
        if(monsterCardList.Count < 3) monsterCardList.Add(tmp);
    } //monsterListが3枚未満ならカードを追加する

}
public class ChatArea : MonoBehaviour {
    private string chatText = "";
    private string chatLog = "";
    public void Init() {
        chatText = "";
        chatLog = "";
    }
    public string ChatLog {
        set { chatLog = value; }
        get { return chatLog; }
    }
}
public class PlayerArea : MonoBehaviour {
    //instance
    private string playerID = "";
    private int leaderCard = 0;
    private List<SmallCard> playerHandList = new List<SmallCard>();
    private List<SmallCard> playerHeroCardList = new List<SmallCard>();
    private List<LargeCard> slayedMonsterList = new List<LargeCard>();
    
    //getter and setter
    public List<SmallCard> PlayerHandList {
        get { return playerHandList; }
    }
    public List<SmallCard> PlayerHeroCardList {
        get { return playerHeroCardList; }
    }
    public List<LargeCard> SlayedMonsterList {
        get { return slayedMonsterList; }
    }

    //method
    public void Init(int num) {
        leaderCard = num;
        playerHandList.Clear();
        playerHeroCardList.Clear();
    } //初期化
    public PlayerArea(PlayerData playerData) {
        this.playerID = playerData.playerID;
        this.leaderCard = playerData.leaderCardID;
        DataToHand(playerData.playerHandList);
        DataToHeroList(playerData.playerHeroCardList);
        DataToSlayedList(playerData.slayedMonsterList);
    } //コンストラクタ
    public PlayerData PlayerAreaToData() {
        PlayerData playerData = new PlayerData();
        playerData.playerID = "";
        playerData.leaderCardID = leaderCard;
        playerData.playerHandList = HandToData();
        playerData.playerHeroCardList = HeroListToData();
        playerData.slayedMonsterList = SlayedListToData();
        return playerData;
    } //プレイヤーの情報をdataに
    public void DataToPlayerArea(PlayerData playerData) {
        this.playerID = playerData.playerID;
        this.leaderCard = playerData.leaderCardID;
        DataToHand(playerData.playerHandList);
        DataToHeroList(playerData.playerHeroCardList);
        DataToSlayedList(playerData.slayedMonsterList);
    } //dataをプレイヤーの情報に
    public List<int> HandToData() {
        List<int> data = new List<int>();
        foreach(SmallCard card in playerHandList) {
            data.Add(card.ID);
        }
        return data;
    } //手札をdataに
    public void DataToHand(List<int> data) {
        playerHandList.Clear();
        foreach(int id in data) {
            playerHandList.Add(new SmallCard(id, ""));

        }
    } //dataを手札に
    public List<HeroCard> HeroListToData() {
        List<HeroCard> data = new List<HeroCard>();
        foreach(SmallCard card in playerHeroCardList) {
            HeroCard cardData = new HeroCard();
            cardData.cardID = card.ID;
            cardData.armedCardID = card.ArmedItem.ID;
            data.Add(cardData);
        }
        return data;
    } //ヒーローリストをdataに
    public void DataToHeroList(List<HeroCard> data) {
        playerHeroCardList.Clear();
        foreach(HeroCard hero in data) {
            playerHeroCardList.Add(new SmallCard(hero.cardID, "").ArmedItem= new SmallCard(hero.armedCardID,""));
        }
    } //dataをヒーローリストに
    public List<int> SlayedListToData() {
        List<int> data = new List<int>();
        foreach(LargeCard card in slayedMonsterList) {
            data.Add(card.ID);
        }
        return data;
    } //倒したモンスターをdataに
    public void DataToSlayedList(List<int> data) {
        slayedMonsterList.Clear();
        foreach(int id in data) {
            slayedMonsterList.Add(new LargeCard(id, ""));
        }
    } //dataを倒したモンスターに
    public SmallCard PickHand(int order) {
        SmallCard tmp = playerHandList[order];
        playerHandList.RemoveAt(order);
        return tmp;
    } //手札から一枚カードを取り出す
    public void PushHand(SmallCard tmp) {
        playerHandList.Add(tmp);
    }//カードを手札に追加する
    public SmallCard PickHeroCard(int order) {
        SmallCard tmp = playerHeroCardList[order];
        playerHeroCardList.RemoveAt(order);
        return tmp;
    }//ヒーローリストの中からカードを一枚取り出す
    public void PushHeroCard(SmallCard tmp) {
        playerHeroCardList.Add(tmp);
    }//ヒーローリストにカードを追加する
    public SmallCard PickArmedCard(int order) {
        SmallCard tmp = playerHeroCardList[order].ArmedItem;
        playerHeroCardList[order].ArmedItem = null;
        return tmp;
    } //ヒーローが装備しているアイテムを取り出す
    public void AttachArmedCard(SmallCard tmp,int order) {
        if (order < playerHeroCardList.Count || playerHeroCardList.Count < order) throw new Exception("存在しないヒーローを指定しています");
        playerHeroCardList[order].ArmedItem = tmp;
    } //ヒーローにアイテムを装備させる
    public LargeCard PickSlayedMonster(int order) {
        LargeCard tmp = slayedMonsterList[order];
        slayedMonsterList.RemoveAt(order);
        return tmp;
    } //討伐したモンスターリストからカードを一枚取り出す
    public void PushSlayedMonster(LargeCard tmp) {
        slayedMonsterList.Add(tmp);
    } //討伐したモンスターリストにカードを追加する


}
public class SmallCard {
    private int cardID = 0;
    private string cardEffect = "";
    private SmallCard armedItem = null;
    public SmallCard(int cardID, string cardEffect) {
        this.cardID = cardID;
        this.cardEffect = cardEffect;
        armedItem = new SmallCard(-1,"");
    }
    //setter and getter
    public int ID {
        get { return cardID; }
        set { cardID = value; }
    }
    public SmallCard ArmedItem {
        set { armedItem = value; }
        get { return armedItem; }
    }
}
public class LargeCard {
    private int cardID = 0;
    private string cardEffect = "";
    public LargeCard(int cardID, string cardEffect) {
        this.cardID = cardID;
        this.cardEffect = cardEffect;
    }
    public int ID {
        get { return cardID; }
        set { cardID = value; }
    }
}
public class ServerConnector : MonoBehaviour{
    private WebSocket ws;
    private ReactiveProperty<string> receivedMessage=new ReactiveProperty<string>("first");
    public IReactiveProperty<string> _receivedMessage => receivedMessage;
    
    void Start() {
        ws = new WebSocket("wss://htsserver.5m8d.net/");
        var context = SynchronizationContext.Current;
        ws.OnOpen += (sender, e) => {
            Debug.Log("Connect to server.");
        };
        ws.OnMessage += (sender, e) => {
            Debug.Log("received message"+e.Data);
            context.Post(state => {
                receivedMessage.Value = state.ToString();
            }, e.Data);
        };

        ws.OnError += (sender, e) => {
            Debug.Log("WebSocket Error Message: " + e.Message);
        };

        ws.OnClose += (sender, e) => {
            Debug.Log("disconnect to server.");
        };
        ws.Connect();
    }
    public void SendText(string text) {
        ws.Send(text);
    }

    void OnDestroy() {
        ws.Close();
        ws = null;
    }
}
public struct GameBoardData {
    public List<int> discardPile;
    public List<int> mainDeck;

    public List<int> monsterCardList;
    public List<int> monsterDeck;

    public string chatLog;

    public int turnPlayerNum;
    public int playerCount;
    public List<PlayerData> playerList;
}
public struct PlayerData {
    public string playerID;
    public int leaderCardID;
    public List<int> playerHandList;
    public List<HeroCard> playerHeroCardList;
    public List<int> slayedMonsterList;

}
public struct HeroCard {
    public int cardID;
    public int armedCardID;
}
public class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour {
    private static T instance;

    public static T Instance {
        get {
            if (instance == null) {
                instance = (T)FindObjectOfType(typeof(T));

                if (instance == null) {
                    Debug.LogError(typeof(T) + "がシーンに存在しません。");
                }
            }

            return instance;
        }
    }

}
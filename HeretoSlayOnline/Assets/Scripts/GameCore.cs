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
    //�T�[�o�Ƃ̒ʐM���s��
    public ServerConnector connector;
    
    //GameBoard�̎���
    GameBoard gb= new GameBoard();
    
    //Game�̃X�e�[�g
    private ReactiveProperty<GameState> state = new ReactiveProperty<GameState>(GameState.entrance);

    //Entrance
    public GameObject entrance;
    private string userName="";
    private bool isReady = false;

    //GUI
    public GameObject[] tabs;
    private IntReactiveProperty visibleTabNum = new IntReactiveProperty(0);
    
    public GameBoardView gameBoardView = new GameBoardView();

    void Start()
    {
        connector = this.gameObject.AddComponent<ServerConnector>();
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

    //tab�؂�ւ�
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
        connector.SendText("0:::"+userName);
    }
    public void ApplyIsReady(bool a) {
        isReady = a;
        if (isReady) connector.SendText("1:::1");
        else connector.SendText("1:::0");
    }

    private void ApplyRecentBoard(GameBoardData newBoard) {
        gb.ApplyNewBoard(newBoard);
    }
    private void GetMessage(string msg) {
        string[] message = msg.Split(":::");
        if (message[0] == "0") {//0:::playerNum:::userName
            if (state.Value != GameState.entrance) return;
            gb.PlayerID = int.Parse(message[1]); //player��ID��ݒ�
        }
        else if (message[0] == "1") { //1:::playerCount 
            if (state.Value != GameState.wait) return;
            gb.PlayerCount = int.Parse(message[1]);
            if (gb.PlayerID == 1) {
                gb.InitializeGameBoard();
            }
            state.Value = GameState.ingame;
        }
        else if (message[0] == "2") { //2:::json
            if (state.Value != GameState.ingame) return;

            //�󂯎����json���N���X�ɕϊ���GameBoard�ɓK�p
            gb.ApplyNewBoard(JsonToGameBoard(message[1]));
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
        //�X�v���C�g�̓Ǎ�
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
    } //������
    public void ApplyNewBoard(GameBoardData gbd) {
        deckArea.DataToDeck(gbd.mainDeck);
        deckArea.DataToPile(gbd.discardPile);
        monsterArea.DataToDeck(gbd.monsterDeck);
        monsterArea.DataToList(gbd.monsterCardList);
        DataToPlayerList(gbd.playerList);
    } //GameBoard���X�V����
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
    }//GameBoard�̃��f�����f�[�^�ɕϊ�����
    public List<PlayerData> PlayerListToData() {
        List<PlayerData> data = new List<PlayerData>();
        foreach(PlayerArea a in playerAreaList) {
            data.Add(a.PlayerAreaToData());
        }
        return data;
    } //playerAreaList��data��
    public void DataToPlayerList(List<PlayerData> playerList) {
        playerAreaList.Clear();
        foreach(PlayerData pd in playerList) {
            playerAreaList.Add(new PlayerArea(pd));
        }
    } //data��playerAreaList��
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
    } //MainDeck��data��
    public void DataToDeck(List<int> data) {
        mainDeck.Clear();
        foreach (int id in data) {
            mainDeck.Add(new SmallCard(id, ""));
        }
    } //data��MainDeck��
    public List<int> PileToData() {
        List<int> data = new List<int>();
        foreach(SmallCard card in discardPile) {
            data.Add(card.ID);
        }
        return data;
    } //DiscardPile��data��
    public void DataToPile(List<int> data) {
        discardPile.Clear();
        foreach(int id in data) {
            mainDeck.Add(new SmallCard(id, ""));
        }
    } //data��DiscardPile��
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
    } //���ݏo�����Ă��郂���X�^�[�̃��X�g��data��
    public void DataToList(List<int> data) {
        monsterCardList.Clear();
        foreach(int id in data) {
            monsterCardList.Add(new LargeCard(id, ""));
        }
    } //data�����ݏo�����Ă��郂���X�^�[�̃��X�g��
    public List<int> DeckToData() {
        List<int> data = new List<int>();
        foreach(LargeCard card in monsterDeck) {
            data.Add(card.ID);
        }
        return data;
    } //MonsterDeck��data��
    public void DataToDeck(List<int> data) {
        monsterDeck.Clear();
        foreach(int id in data) {
            monsterDeck.Add(new LargeCard(id, ""));
        }
    } //data��MonsterDeck��
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
    
    //method
    public void Init(int num) {
        leaderCard = num;
        playerHandList.Clear();
        playerHeroCardList.Clear();
    } //������
    public PlayerArea(PlayerData playerData) {
        this.playerID = playerData.playerID;
        this.leaderCard = playerData.leaderCardID;
        DataToHand(playerData.playerHandList);
        DataToHeroList(playerData.playerHeroCardList);
        DataToSlayedList(playerData.slayedMonsterList);
    } //�R���X�g���N�^
    public PlayerData PlayerAreaToData() {
        PlayerData playerData = new PlayerData();
        playerData.playerID = "";
        playerData.leaderCardID = leaderCard;
        playerData.playerHandList = HandToData();
        playerData.playerHeroCardList = HeroListToData();
        playerData.slayedMonsterList = SlayedListToData();
        return playerData;
    } //�v���C���[�̏���data��
    public void DataToPlayerArea(PlayerData playerData) {
        this.playerID = playerData.playerID;
        this.leaderCard = playerData.leaderCardID;
        DataToHand(playerData.playerHandList);
        DataToHeroList(playerData.playerHeroCardList);
        DataToSlayedList(playerData.slayedMonsterList);
    }
    public List<int> HandToData() {
        List<int> data = new List<int>();
        foreach(SmallCard card in playerHandList) {
            data.Add(card.ID);
        }
        return data;
    } //��D��data��
    public void DataToHand(List<int> data) {
        playerHandList.Clear();
        foreach(int id in data) {
            playerHandList.Add(new SmallCard(id, ""));
        }
    } //data����D��
    public List<int> HeroListToData() {
        List<int> data = new List<int>();
        foreach(SmallCard card in playerHeroCardList) {
            data.Add(card.ID);
        }
        return data;
    } //�q�[���[���X�g��data��
    public void DataToHeroList(List<int> data) {
        playerHeroCardList.Clear();
        foreach(int id in data) {
            playerHeroCardList.Add(new SmallCard(id, ""));
        }
    } //data���q�[���[���X�g��
    public List<int> SlayedListToData() {
        List<int> data = new List<int>();
        foreach(LargeCard card in slayedMonsterList) {
            data.Add(card.ID);
        }
        return data;
    } //�|���������X�^�[��data��
    public void DataToSlayedList(List<int> data) {
        slayedMonsterList.Clear();
        foreach(int id in data) {
            slayedMonsterList.Add(new LargeCard(id, ""));
        }
    } //data��|���������X�^�[��
}
public class SmallCard {
    private int cardID = 0;
    private string cardEffect = "";
    public SmallCard(int cardID, string cardEffect) {
        this.cardID = cardID;
        this.cardEffect = cardEffect;
    }
    //setter and getter
    public int ID {
        get { return cardID; }
        set { cardID = value; }
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
    public List<int> playerHeroCardList;
    public List<int> slayedMonsterList;

}
public class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour {
    private static T instance;

    public static T Instance {
        get {
            if (instance == null) {
                instance = (T)FindObjectOfType(typeof(T));

                if (instance == null) {
                    Debug.LogError(typeof(T) + "���V�[���ɑ��݂��܂���B");
                }
            }

            return instance;
        }
    }

}
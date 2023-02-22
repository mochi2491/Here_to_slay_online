using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UniRx;
using WebSocketSharp;
using WebSocketSharp.Server;
using Newtonsoft.Json;
using TMPro;
using CardData;
using Zenject.SpaceFighter;
using Unity.VisualScripting;

public class GameCore : SingletonMonoBehaviour<GameCore>
{
    //サーバとの通信
    public ServerConnector connector;
    [HideInInspector]public SendTextEvent sendTextEvent;

    //GoogleSpreadSheet
    private CardSheetReader DataReader;
    private CardDataManager CardDataManager = new CardDataManager();

    //GameBoard
    public ReactiveProperty<GameBoard> gameBoard;
    public IReadOnlyReactiveProperty<GameBoard> _gameBoard => gameBoard;
    public IGameBoard IgameBoard;

    public GameBoardView gameBoardView;
    public GameObject gameBoardObject;

    //GameState
    private ReactiveProperty<GameState> state = new ReactiveProperty<GameState>(GameState.entrance);
    public IReadOnlyReactiveProperty<GameState> _state => state;
    [HideInInspector]public ChangeStateEvent changeStateEvent;

    //Entrance
    public Entrance entrance;
    public IEntrance _entrance;
    public GameObject entranceObject;

    //FieldTabs
    public FieldTabsModel fieldTabs;
    public IFieldTabs _fieldTabs;

    public int playerID = 0;

    //CardDataManager
    public CardDataManager cardDataManager = new CardDataManager();

    //GUI
    public EventTrigger trigger ;

    private ReactiveProperty<CardIndicatorModel> cardIndicatorModel = new ReactiveProperty<CardIndicatorModel>(new CardIndicatorModel());
    public IReadOnlyReactiveProperty<CardIndicatorModel> _cardIndicatorModel => cardIndicatorModel;
    public CommandPanelView commandPanelView;
    [HideInInspector]public ReactiveProperty<CommandPanelModel> commandPanelModel = new ReactiveProperty<CommandPanelModel>(new CommandPanelModel());
    public IReadOnlyReactiveProperty<CommandPanelModel> _commandPanelModel=> commandPanelModel;
    public GameObject[] tabs;
    public ReactiveProperty<MenuPanelModel> menuPanelModel = new ReactiveProperty<MenuPanelModel>(new MenuPanelModel());
    //private IntReactiveProperty visibleTabNum = new IntReactiveProperty(0);
    
    public bool isHeroItem = false;
    public TMP_Dropdown heroNum;
    public GameBoardAddress FromAddress = new GameBoardAddress();
    public GameBoardAddress ToAddress = new GameBoardAddress();


    void Awake()
    {
        heroNum = commandPanelView.smallPanels[2].transform.Find("Dropdown").gameObject.GetComponent<TMP_Dropdown>();
        connector = this.gameObject.AddComponent<ServerConnector>();
        
        connector._receivedMessage.Subscribe(
            x => {
                GetMessage(x);
            }
        );


        //sendTextメソッドをsendTextEventに登録
        sendTextEvent.AddListener(connector.SendText);
        //changeStateメソッドをchangeStateEventに登録
        changeStateEvent.AddListener(ChangeState);

        //gameboard
        gameBoard.Value = new GameBoard();
        //gameBoard = this.gameObject.AddComponent<GameBoard>(); //インスタンス生成
        IgameBoard = gameBoard.Value; //interfaceの宣言
        gameBoard.Value.InitializeGameBoard();

        //entrance
        entrance = this.gameObject.AddComponent<Entrance>();
        _entrance = entrance; //interfaceの受け渡し
        _entrance.Init(sendTextEvent,changeStateEvent); //初期化
        _state.Subscribe(state => { _entrance.SetState(state); }); //GameCoreのstateを渡す
        entrance._userName.Subscribe(name => { gameBoard.Value.chatArea.SetUserName(name); });

        //fieldTabs
        fieldTabs = this.gameObject.AddComponent<FieldTabsModel>();
        _fieldTabs = fieldTabs; //interfaceの受け渡し

        //spread Sheet
        DataReader = this.gameObject.AddComponent<CardSheetReader>();
        DataReader.callBack.AddListener(SetCardData);
        DataReader.Load();

        entranceObject.SetActive(true);
        gameBoardObject.SetActive(false);

        
    }
    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            commandPanelModel.Value = commandPanelModel.Value.CloseAllPanel();
        }
    }

    public void IndicateCard(bool isLarge,int cardID) {
        cardIndicatorModel.Value = _cardIndicatorModel.Value.Indicate(isLarge,cardID);
    }
    public void SetCardData() {
        cardDataManager = DataReader.cdm;
    }
    public void ChangeState(GameState state) {
        this.state.Value = state;
    }
    private void GetMessage(string msg) {
        string[] message = msg.Split(":::");
        if (message[0] == "0") {//0:::playerNum:::userName
            gameBoard.Value.PlayerID = int.Parse(message[1]); //playerのIDを設定
            this.playerID = gameBoard.Value.PlayerID;
        }
        else if (message[0] == "1") { //1:::playerCount 
            if (state.Value != GameState.wait) return;
            gameBoard.Value.PlayerCount = int.Parse(message[1]);
            //gameBoard.InitializeGameBoard();
            entranceObject.SetActive(false);
            gameBoardObject.SetActive(true);
            state.Value = GameState.ingame;
        }
        else if (message[0] == "2") { //2:::json
            if (state.Value != GameState.ingame) return;
            //受け取ったjsonをクラスに変換しGameBoardに適用
            gameBoard.Value = gameBoard.Value.ApplyNewBoard(JsonToGameBoard(message[1]),gameBoardView);
        }
        else if (message[0] == "first") {
        }
        else {
            Debug.Log("received wrong message;");
        }
    }
    public void ControlBoard(GameBoardAddress From) {
        GameBoardData board = new GameBoardData();
        board = gameBoard.Value.GameBoardToData(gameBoard.Value.ControlBoard(From, ToAddress));
        connector.SendText("2:::"+GameBoardToJson(board));
    }
    public void ControlLog(string text) {
        GameBoardData board = new GameBoardData();
        board = gameBoard.Value.GameBoardToData(gameBoard.Value.AddLog(text));
        connector.SendText("2:::" + GameBoardToJson(board));
    }
    public void RenewBoard() {
        GameBoardData board = new GameBoardData();
        board = gameBoard.Value.GameBoardToData(gameBoard.Value);
        connector.SendText("2:::"+GameBoardToJson(board));
    }
    public void ControlLeaderNum(int num) {
        GameBoardData board = new GameBoardData();
        board = gameBoard.Value.GameBoardToData(gameBoard.Value.SetLeaderNum(playerID, num));
        connector.SendText("2:::" + GameBoardToJson(board));
    }
    public void SetFromAddress(GameBoardAddress gba) {
        FromAddress = gba;
    }
    private string GameBoardToJson(GameBoardData gbd) { 
        return JsonConvert.SerializeObject(gbd);
    }
    private GameBoardData JsonToGameBoard(string json) {
        return JsonConvert.DeserializeObject<GameBoardData>(json);
    }
    public void SetToPlayerNum(int num) {
        ToAddress.playerID = num;
        if (isHeroItem) {
            var heroCount = gameBoard.Value.playerAreaList[ToAddress.playerID].PlayerHeroCardList;
            commandPanelView.smallPanels[2].transform.Find("Dropdown").gameObject.GetComponent<TMP_Dropdown>().ClearOptions();
            List<string> optionList = new List<string>();
            int i = 0;
            foreach(HeroCard sc in heroCount) {
                optionList.Add(i.ToString());
                i++;
            }
            commandPanelView.smallPanels[2].transform.Find("Dropdown").gameObject.GetComponent<TMP_Dropdown>().AddOptions(optionList);
            commandPanelModel.Value = commandPanelModel.Value.TransitionSmallPanel(CommandPanelView.PanelName.order);

        }
        else {
            commandPanelModel.Value = commandPanelModel.Value.CloseAllPanel();
            ControlBoard(FromAddress);
        }
    }
    public void SetToPlayerNumLarge(int num) {
        ToAddress.playerID = num;
        commandPanelModel.Value = commandPanelModel.Value.CloseAllPanel();
        ControlBoard(FromAddress);
    }
    public void DefineToOrderNum() {
        ToAddress.order = commandPanelView.smallPanels[2].transform.Find("Dropdown").gameObject.GetComponent<TMP_Dropdown>().value;
        commandPanelModel.Value = commandPanelModel.Value.CloseAllPanel();
        ControlBoard(FromAddress);
    }
    public void SetToSmallArea(int num) {
        switch (num) {
            case 0:
                ToAddress.area = Area.playerHand;
                commandPanelModel.Value = commandPanelModel.Value.TransitionSmallPanel(CommandPanelView.PanelName.player);
                isHeroItem = false;
                break;
            case 1:
                ToAddress.area = Area.playerHero;
                commandPanelModel.Value = commandPanelModel.Value.TransitionSmallPanel(CommandPanelView.PanelName.player);
                isHeroItem = false;
                break;
            case 2:
                ToAddress.area = Area.discardPile;
                commandPanelModel.Value = commandPanelModel.Value.CloseAllPanel();
                ControlBoard(FromAddress);
                break;
            case 3:
                ToAddress.area = Area.playerHeroItem;
                commandPanelModel.Value = commandPanelModel.Value.TransitionSmallPanel(CommandPanelView.PanelName.player);
                isHeroItem = true; 
                break;
            case 4:
                ToAddress.area = Area.deck;
                commandPanelModel.Value = commandPanelModel.Value.CloseAllPanel();
                ControlBoard(FromAddress);
                break;
        }
    }
    public void SetToLargeArea(int num) {
        switch (num) {
            case 0:
                ToAddress.area = Area.slayedMonster;
                commandPanelModel.Value = commandPanelModel.Value.TransitionLargePanel(CommandPanelView.PanelName.player);
                break;
            case 1:
                ToAddress.area = Area.monsterDeck;
                commandPanelModel.Value = commandPanelModel.Value.CloseAllPanel();
                ControlBoard(FromAddress);
                break;
            case 2:
                ToAddress.area = Area.monsterList;
                commandPanelModel.Value = commandPanelModel.Value.TransitionLargePanel(CommandPanelView.PanelName.order);
                break;
        }
    }
    public void SetToOrder(int num) {
        ToAddress.order = num;
        commandPanelModel.Value = commandPanelModel.Value.CloseAllPanel();
        ControlBoard(FromAddress);
    }
    public void SetToHeroNum() {
        ToAddress.order = heroNum.value;
        commandPanelModel.Value = commandPanelModel.Value.CloseAllPanel();
        ControlBoard(FromAddress);
    }
    public void DrawCard() {
        ToAddress.playerID = this.playerID;
        ToAddress.area = Area.playerHand;
        GameBoardAddress from = new GameBoardAddress();
        from.area = Area.deck;
        ControlBoard(from);
    }
    public void DrawMonster() {
        if (gameBoard.Value.monsterArea.monsterCardList.Count < 3) { 
            ToAddress.order = gameBoard.Value.monsterArea.monsterCardList.Count;
            ToAddress.area = Area.monsterList;
            GameBoardAddress from = new GameBoardAddress();
            from.area = Area.monsterDeck;
            ControlBoard(from);
        }
    }
    public void DeckShuffle() {
        GameBoardData board = new GameBoardData();
        board = gameBoard.Value.GameBoardToData(gameBoard.Value.DeckShuffle());
        connector.SendText("2:::" + GameBoardToJson(board));
    }
    public void ResetBoard() {
        GameBoard board = new GameBoard();
        GameBoardData boardData = gameBoard.Value.GameBoardToData(board.InitializeGameBoard());
        connector.SendText("2:::" + GameBoardToJson(boardData));
    }
    public void QuitGame() {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;//ゲームプレイ終了
#else
    Application.Quit();//ゲームプレイ終了
#endif
    }

}
public class CardIndicatorModel {
    private bool isLarge;
    private int cardID;
    public CardIndicatorModel() {
        isLarge = false;
        cardID = 0;
    }
    public int GetID {
        get { return cardID; }
    }
    public bool IsLarge {
        get { return isLarge; }
    }
    private CardIndicatorModel(bool isLarge,int cardID) {
        this.isLarge = isLarge;
        this.cardID = cardID;
    }
    public CardIndicatorModel Indicate(bool isLarge,int cardID) {
        if (cardID < 0) throw new Exception("存在しないカードIDです");
        return new CardIndicatorModel(isLarge,cardID);
    }
}
public class MenuPanelModel {
    public bool isActive = false;
    public MenuPanelModel() {
        isActive = false;
    }
    private MenuPanelModel(bool b) {
        isActive = b;
    }
    public MenuPanelModel Open() {
        return new MenuPanelModel(true);
    }
    public MenuPanelModel Close() { 
        return new MenuPanelModel(false); 
    }
}
public class CommandPanelModel{
    private List<bool> isActive = new List<bool>();
    public bool closerActive = false;
    private Vector3 mousePos = Vector3.zero;
    public List<bool> IsActive { get { return isActive; } }
    public Vector3 MousePos { get { return mousePos; } }

    private ReactiveProperty<int> heroNum = new ReactiveProperty<int>(0); 
    public CommandPanelModel() {
        int i = 0;
        for (i = 0; i < 6; i++) {
            isActive.Add(false);
        }
    }
    private CommandPanelModel(List<bool> list, Vector3 mousePos, bool closerActive) {
        isActive = list;
        this.mousePos = mousePos;
        this.closerActive = closerActive;
    }

    //全てのコマンドパネルを閉じるメソッド
    public CommandPanelModel CloseAllPanel() {
        int i = 0;
        List<bool> list = new List<bool>();
        for(i=0;i<6;i++) {
            list.Add(false);
        }
        return new CommandPanelModel(list,this.mousePos,false);
    }
    //最初にコマンドパネルを開くメソッド
    public CommandPanelModel OpenSmallCommandPanel(CommandPanelView.PanelName panel,Vector3 mousePos) {
        int i = 0;
        List<bool> list = new List<bool>();
        for (i = 0; i < 6; i++) {
            list.Add(false);
        }
        switch (panel) {
            case CommandPanelView.PanelName.main:
                list[0] = true;
                break;
            case CommandPanelView.PanelName.player:
                list[1] = true;
                break;
            case CommandPanelView.PanelName.order:
                list[2] = true;
                break;
        }
        return new CommandPanelModel(list,mousePos,true);
    }
    public CommandPanelModel OpenLargeCommandPanel(CommandPanelView.PanelName panel,Vector3 mousePos) {
        int i = 0;
        List<bool> list = new List<bool>();
        for (i = 0; i < 6; i++) {
            list.Add(false);
        }
        switch (panel) {
            case CommandPanelView.PanelName.main:
                list[3] = true;
                break;
            case CommandPanelView.PanelName.player:
                list[4] = true;
                break;
            case CommandPanelView.PanelName.order:
                list[5] = true;
                break;
        }
        return new CommandPanelModel(list,mousePos,true);
    }

    //開いたコマンドパネルを遷移させるメソッド
    public CommandPanelModel TransitionSmallPanel(CommandPanelView.PanelName panel) {
        int i = 0;
        List<bool> list = new List<bool>();
        for (i = 0; i < 6; i++) {
            list.Add(false);
        }
        switch (panel) {
            case CommandPanelView.PanelName.main:
                list[0] = true;
                break;
            case CommandPanelView.PanelName.player:
                list[1] = true;
                break;
            case CommandPanelView.PanelName.order:
                list[2] = true;
                break;
        }
        return new CommandPanelModel(list,this.mousePos,true);
    }
    public CommandPanelModel TransitionLargePanel(CommandPanelView.PanelName panel) {
        int i = 0;
        List<bool> list = new List<bool>();
        for (i = 0; i < 6; i++) {
            list.Add(false);
        }
        switch (panel) {
            case CommandPanelView.PanelName.main:
                list[3] = true;
                break;
            case CommandPanelView.PanelName.player:
                list[4] = true;
                break;
            case CommandPanelView.PanelName.order:
                list[5] = true;
                break;
        }
        return new CommandPanelModel(list, this.mousePos, true);
    }

}
[System.Serializable]public class SendTextEvent : UnityEvent<string> {

} //SendTextを子クラスに渡すためのクラス
[System.Serializable]public class ChangeStateEvent : UnityEvent<GameState> {

} //ChangeStateを子クラスに渡すためのクラス
public enum GameState {
    entrance,wait,ingame
}
public interface IEntrance { 
    public void Init(SendTextEvent sendText,ChangeStateEvent changeState);
    public void SetState(GameState state);
    public void SetUserName(string name);
    public void SendUserName();
    public void ApplyIsReady(bool isReady);

}
public class Entrance : MonoBehaviour,IEntrance{
    SendTextEvent sendText; //textを送信するためのEvent
    ChangeStateEvent changeState; //GameCoreのstateを変更するためのEvent
    GameState state; //現在のGameCoreのState
    private ReactiveProperty<string> userName = new ReactiveProperty<string>(""); //ユーザーネーム
    public IReadOnlyReactiveProperty<string> _userName => userName;
    private bool isReady = false; //Playerの準備状況
    
    //setter and getter
    public string UserName {
        get { return userName.Value; }
    }

    //初期化
    public void Init(SendTextEvent sendTextEvent,ChangeStateEvent changeStateEvent) {
        this.sendText = sendTextEvent;
        this.changeState = changeStateEvent;
    }

    //public method
    public void SetState(GameState state) {
        this.state = state;
    }
    public void SetUserName(string name) {
        this.userName.Value = name;
    }
    public void SendUserName() {
        if (state == GameState.entrance) {
            sendText.Invoke("0:::" + userName);
            changeState.Invoke(GameState.wait);
        }
    }
    public void ApplyIsReady(bool isReady) {
        if (state != GameState.wait) return;
        this.isReady = isReady;
        if (this.isReady) sendText.Invoke("1:::1");
        else sendText.Invoke("1:::0");
    }
}
public interface IFieldTabs {
    public void SetVisibleTabNum(int num);
}
public class FieldTabsModel : MonoBehaviour,IFieldTabs {
    private IntReactiveProperty visibleTabNum = new IntReactiveProperty(0);
    public IReadOnlyReactiveProperty<int> _visibleTabNum => visibleTabNum;
    public void SetVisibleTabNum(int num) {
        visibleTabNum.Value = num;
    }//tabの切り替え
}
public interface IGameBoard {

}
public class GameBoard : IGameBoard {
    public static readonly int SMALLCARD_COUNT = 73;
    public static readonly int LARGECARD_COUNT = 20;
    private Sprite cardBack;
    
    private int playerCount = 0;
    private IntReactiveProperty playerID = new IntReactiveProperty(0);
    public IReadOnlyReactiveProperty<int> _playerID => playerID;
    private DeckArea deckArea = new DeckArea();
    public MonsterArea monsterArea = new MonsterArea();
    public ChatArea chatArea = new ChatArea();
    public List<PlayerArea> playerAreaList = new List<PlayerArea>();
    //setter and getter
    public int PlayerID {
        set {
            playerID.Value = value;
        }
        get {
            return playerID.Value;
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
    public DeckArea GetdeckArea {
        get {return this.deckArea;}
    }
    public MonsterArea GetmonsterArea {
        get {return this.monsterArea;}
    }

    public GameBoard InitializeGameBoard() {
        for (int i = 0; i < 6; i++) playerAreaList.Add(new PlayerArea());
        cardBack = Resources.Load("back") as Sprite;
        deckArea.Init();
        monsterArea.Init();
        chatArea.Init();
        foreach (PlayerArea pa in playerAreaList) {
            pa.Init(0);
        }
        return this;
    } //初期化
    public GameBoard ApplyNewBoard(GameBoardData gbd,GameBoardView gbv) {
        GameBoard board = (GameBoard)MemberwiseClone();
        board.deckArea.DataToDeck(gbd.mainDeck);
        board.deckArea.DataToPile(gbd.discardPile);
        board.monsterArea.DataToDeck(gbd.monsterDeck);
        board.monsterArea.DataToList(gbd.monsterCardList);
        board.chatArea.ApplyLog(gbd.chatLog);
        board.DataToPlayerList(gbd.playerList);
        return board;
    } //GameBoardを更新する
    public GameBoardData GameBoardToData(GameBoard gb) {
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
    public GameBoard DeckShuffle() {
        GameBoard board = (GameBoard)MemberwiseClone();
        board.deckArea.Shuffle();
        return this;
    }
    public GameBoard AddLog(string text) {
        GameBoard board = (GameBoard) MemberwiseClone();
        board.chatArea.AddLog(text);
        return board;
    }
    public GameBoard SetLeaderNum(int playerNum,int num) {
        GameBoard board = (GameBoard)MemberwiseClone();
        board.playerAreaList[playerNum].SetLeaderID(num);
        return board;
    }
    public GameBoard ControlBoard(GameBoardAddress From,GameBoardAddress To) {
        bool isLarge = false;
        string logText = "";
        GameBoard copy = (GameBoard)MemberwiseClone();
        if ((From.area == Area.monsterList) || (From.area == Area.monsterDeck) || (From.area == Area.slayedMonster)) {
            isLarge = true;
        }
        //カードを移動させる
        if (isLarge) {
            LargeCard moveCard;
            switch (From.area) {
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
            switch (To.area) {
                case Area.monsterList:
                    copy.monsterArea.PushList(moveCard,To.order);
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
        else {
            SmallCard moveCard;
            switch (From.area) {
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
                    if(a.equip.ID != -1) copy.playerAreaList[playerID.Value].PushHand(a.equip);
                    break;
                default:
                    moveCard = null;
                    break;
            }
            switch (To.area) {
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
                    copy.playerAreaList[To.playerID].AttachArmedCard(moveCard,To.order);
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
    public GameBoardAddress SearchCard(int cardID,bool isLarge) {
        int playerNum = 0;
        int orderNum = 0;
        GameBoardAddress address = new GameBoardAddress();
        if (isLarge) {
            address.area = Area.slayedMonster;
            playerNum = 0;
            //slayedMonsterListから探索
            foreach (PlayerArea pa in playerAreaList) {
                address.playerID = playerNum;
                orderNum = 0;
                foreach (LargeCard card in pa.SlayedMonsterList) {
                    address.order= orderNum;
                    if(card.ID == cardID) {
                        return address;
                    }
                    orderNum++;
                }
                playerNum++;
            }
            //monsterListから探索
            address.area = Area.monsterList;
            playerNum = 0;
            orderNum= 0;
            foreach(LargeCard card in monsterArea.monsterCardList) {
                address.order= orderNum;
                if(card.ID == cardID) {
                    return address;
                }
                orderNum++;
            }
        }
        else {
            foreach(PlayerArea pa in playerAreaList) {
                orderNum = 0;
                address.playerID = playerNum;
                address.area = Area.playerHand;
                foreach(SmallCard card in pa.PlayerHandList) {
                    address.order= orderNum;
                    if (card.ID == cardID) return address;
                    orderNum++;
                }
                address.area = Area.playerHero;
                orderNum = 0;
                foreach(HeroCard card in pa.PlayerHeroCardList) {
                    address.order= orderNum;
                    if (card.hero.ID == cardID) return address;
                    orderNum++;
                }
                orderNum = 0;
                address.area = Area.playerHeroItem;
                foreach(HeroCard card in pa.PlayerHeroCardList) {
                    address.order= orderNum;
                    if (card.equip.ID == cardID) return address;
                    orderNum++;
                }
                playerNum++;
            }
            playerNum = 0;
            orderNum = 0;
            address.area = Area.discardPile;
            foreach(SmallCard card in deckArea.discardPile) {
                address.order= orderNum;
                if (card.ID == cardID) return address;
                orderNum++;
            }
        }
        return address;
    }
}
public interface CardMover {

}
public struct GameBoardAddress {
    public Area area;
    public int playerID;
    public int order;
}
public enum Area {
    deck,discardPile, monsterList,monsterDeck,playerHand,playerHero,playerHeroItem,slayedMonster
}
public class DeckArea {
    private List<SmallCard> mainDeck = new List<SmallCard>();
    public List<SmallCard> discardPile = new List<SmallCard>();
    public void Init() {
        //mainDeck init
        for(int i = 0; i <= GameBoard.SMALLCARD_COUNT; i++) {
            if (i == 52 || i == 54 || i == 57 || (i >= 66 && i <= 69) || i == 72) { //2枚
                mainDeck.Add(new SmallCard(i, ""));
                mainDeck.Add(new SmallCard(i, ""));
            }
            else if (i == 60 || (i >= 62 && i <= 64)) { //4枚
                for (int j = 0; j < 4; j++) mainDeck.Add(new SmallCard(i, ""));
            }
            else if (i == 61) { //9枚
                for (int k = 0; k < 9; k++) mainDeck.Add(new SmallCard(i, ""));
            }
            else if (i == 73) { //14枚
                for (int l = 0; l < 14; l++) mainDeck.Add(new SmallCard(i, ""));
            }
            else mainDeck.Add(new SmallCard(i, "")); //1枚

        }
        mainDeck = mainDeck.OrderBy(a => Guid.NewGuid()).ToList();
        //discardPile init
        discardPile.Clear();
    }
    public int deckCount() {
        return mainDeck.Count;
    }
    public DeckArea Shuffle() {
        mainDeck = mainDeck.OrderBy(a => Guid.NewGuid()).ToList();
        return this;
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
            discardPile.Add(new SmallCard(id, ""));
        }
    } //dataをDiscardPileに
    public SmallCard PopDeck() {
        if (mainDeck.Count < 1) { 
            Debug.Log("デッキがありません");
            return null;
        }
        SmallCard tmp = mainDeck[0];
        mainDeck.RemoveAt(0);
        return tmp;
    } //deckの頭を取り出す
    public void PushDeck(SmallCard tmp) {
        mainDeck.Insert(0, tmp);
    } //deckの頭にカードを追加
    public SmallCard PickDiscard(int order) {
        SmallCard tmp = discardPile[order];
        discardPile.RemoveAt(order);
        return tmp;
    } //discardPileの指定のカードを取り出す
    public void PushDiscard(SmallCard tmp) {
        discardPile.Add(tmp);
    } //discardPileの最後にカードを追加
}
public class MonsterArea {
    public List<LargeCard> monsterCardList = new List<LargeCard>();
    private List<LargeCard> monsterDeck = new List<LargeCard>();
    public void Init() {
        //monsterDeck init
        for (int i = 0; i <= 14; i++)monsterDeck.Add(new LargeCard(i+6,""));
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
        monsterCardList.RemoveAt(order);
        return tmp;
    } //monsterListからカードを取り出す
    public void PushList(LargeCard tmp,int order) {
        if (monsterCardList.Count < 3) monsterCardList.Add(tmp);
        else {
            PushDeck(monsterCardList[order]);
            monsterCardList.RemoveAt(order);
            monsterCardList.Add(tmp); 
        }
    } //monsterListが3枚未満ならカードを追加する、3枚以上なら一枚戻してから追加する

}
public interface IChatArea {
    public void Init();
    public void AddLog(string text);
}
public class ChatArea {
    private DateTime dt;
    private string userName = "";
    private ReactiveProperty<string> chatLog = new ReactiveProperty<string>("");
    public IReactiveProperty<string> _chatLog => chatLog;
    public ChatArea() {
        userName = "";
        chatLog.Value = "";
    }
    public void Init() {
        chatLog.Value = "";
    }
    public void SetUserName(string name) {
        userName = name;
    }
    public void AddLog(string text) {
        //chatLogにtextを追加する
        dt = DateTime.Now;
        chatLog.Value = chatLog.Value + "\n" + userName + ":" + dt.ToString("HH:mm:ss") + "\n" + text;
    }
    public void ApplyLog(string text) {
        chatLog.Value = text;
    }
}
public class PlayerArea {
    //instance
    private string playerID = "";
    public ReactiveProperty<int> leaderCardID = new ReactiveProperty<int>(0);
    public IReactiveProperty<int> _leaderCardID => leaderCardID;
    private List<SmallCard> playerHandList = new List<SmallCard>();
    private List<HeroCard> playerHeroCardList = new List<HeroCard>();
    private List<LargeCard> slayedMonsterList = new List<LargeCard>();
    
    //getter and setter
    public List<SmallCard> PlayerHandList {
        get { return playerHandList; }
    }
    public List<HeroCard> PlayerHeroCardList {
        get { return playerHeroCardList; }
    }
    public List<LargeCard> SlayedMonsterList {
        get { return slayedMonsterList; }
    }
    public PlayerArea() {

    }
    public PlayerArea(PlayerData playerData) {
        this.playerID = playerData.playerID;
        this.leaderCardID.Value = playerData.leaderCardID;
        DataToHand(playerData.playerHandList);
        DataToHeroList(playerData.playerHeroCardList);
        DataToSlayedList(playerData.slayedMonsterList);
    } //コンストラクタ
    //method
    public void Init(int num) {
        leaderCardID.Value = num;
        playerHandList.Clear();
        playerHeroCardList.Clear();
    } //初期化
    public PlayerData PlayerAreaToData() {
        PlayerData playerData = new PlayerData();
        playerData.playerID = "";
        playerData.leaderCardID = leaderCardID.Value;
        playerData.playerHandList = HandToData();
        playerData.playerHeroCardList = HeroListToData();
        playerData.slayedMonsterList = SlayedListToData();
        return playerData;
    } //プレイヤーの情報をdataに
    public void DataToPlayerArea(PlayerData playerData) {
        this.playerID = playerData.playerID;
        this.leaderCardID.Value = playerData.leaderCardID;
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
    public List<HeroCardData> HeroListToData() {
        List<HeroCardData> data = new List<HeroCardData>();
        foreach(HeroCard card in playerHeroCardList) {
            
            HeroCardData cardData = new HeroCardData();
            cardData.cardID = card.hero.ID;
            if (card.equip != null) {
                cardData.armedCardID = card.equip.ID;
            }
            else cardData.armedCardID = -1;
            
            data.Add(cardData);
        }
        return data;
    } //ヒーローリストをdataに
    public void DataToHeroList(List<HeroCardData> data) {
        playerHeroCardList.Clear();
        foreach(HeroCardData hero in data) {
            if(hero.armedCardID != -1) {
                playerHeroCardList.Add(new HeroCard(new SmallCard(hero.cardID, ""), new SmallCard(hero.armedCardID, "")));
            }
            else {
                playerHeroCardList.Add(new HeroCard(new SmallCard(hero.cardID, ""), new SmallCard(-1, "")));
            }
            
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
    public HeroCard PickHeroCard(int order) {
        Debug.Log(playerHeroCardList.Count+","+order);
        HeroCard tmp = playerHeroCardList[order];
        playerHeroCardList.RemoveAt(order);
        return tmp;
    }//カードリストの中からカードを一枚取り出す
    public void PushHeroCard(HeroCard tmp) {
        playerHeroCardList.Add(tmp);
    }//ヒーローリストにカードを追加する
    public SmallCard PickArmedCard(int order) {
        SmallCard tmp = playerHeroCardList[order].equip;
        playerHeroCardList[order].equip = null;
        return tmp;
    } //ヒーローが装備しているアイテムを取り出す
    public void AttachArmedCard(SmallCard tmp,int order) {
        Debug.Log(playerHeroCardList.Count+","+ order);
        if (order >= playerHeroCardList.Count) throw new Exception("heroが存在しません");
        playerHeroCardList[order].equip = tmp;
    } //ヒーローにアイテムを装備させる
    public LargeCard PickSlayedMonster(int order) {
        LargeCard tmp = slayedMonsterList[order];
        slayedMonsterList.RemoveAt(order);
        return tmp;
    } //討伐したモンスターリストからカードを一枚取り出す
    public void PushSlayedMonster(LargeCard tmp) {
        slayedMonsterList.Add(tmp);
    } //討伐したモンスターリストにカードを追加する
    public void SetLeaderID(int num) {
        leaderCardID.Value = num;
    }
}
public class HeroCard {
    public SmallCard hero=null;
    public SmallCard equip=null;
    public HeroCard(SmallCard hero,SmallCard equip) {
        this.hero = hero;
        this.equip = equip;
    }
}
public class SmallCard {
    private int cardID = 0;
    private string cardEffect = "";
    public SmallCard(int cardID,string cardEffect) {
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
    public List<HeroCardData> playerHeroCardList;
    public List<int> slayedMonsterList;

}
public struct HeroCardData {
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
                    Debug.LogError(typeof(T) + "���V�[���ɑ��݂��܂���B");
                }
            }

            return instance;
        }
    }

}
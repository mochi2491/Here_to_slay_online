using CardData;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
public class GameCore : SingletonMonoBehaviour<GameCore>
{
    //サーバとの通信
    public ServerConnector connector;
    //GoogleSpreadSheet
    private CardSheetReader DataReader;
    private CardDataManager CardDataManager = new CardDataManager();

    //GameBoard
    public List<string> playerNameList = new List<string>();

    //GameBoard
    public ReactiveProperty<GameBoard> gameBoard;
    public IReadOnlyReactiveProperty<GameBoard> _gameBoard => gameBoard;
    public IGameBoard IgameBoard;

    //GameState
    ReactiveProperty<GameState> state = new ReactiveProperty<GameState>(GameState.entrance);
    public IReadOnlyReactiveProperty<GameState> _state => state;


    public GameBoardView gameBoardView;
    public GameObject gameBoardObject;

    //PeepPanel
    public PeepPanelModel peepPanelModel { get; private set; } = new PeepPanelModel(new List<SmallCard>());
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
    public EventTrigger trigger;

    private ReactiveProperty<CardIndicatorModel> cardIndicatorModel = new ReactiveProperty<CardIndicatorModel>(new CardIndicatorModel());
    public IReadOnlyReactiveProperty<CardIndicatorModel> _cardIndicatorModel => cardIndicatorModel;
    public CommandPanelView commandPanelView;
    [HideInInspector] public ReactiveProperty<CommandPanelModel> commandPanelModel = new ReactiveProperty<CommandPanelModel>(new CommandPanelModel());
    public IReadOnlyReactiveProperty<CommandPanelModel> _commandPanelModel => commandPanelModel;
    public GameObject[] tabs;
    public ReactiveProperty<MenuPanelModel> menuPanelModel = new ReactiveProperty<MenuPanelModel>(new MenuPanelModel());

    public bool isHeroItem = false;
    public int heroNum_ = 0;
    public GameBoardAddress FromAddress = new GameBoardAddress();
    public GameBoardAddress ToAddress = new GameBoardAddress();

    private void Awake()
    {
        connector = this.gameObject.AddComponent<ServerConnector>();
        connector._receivedMessage.Subscribe(
            x =>
            {
                GetMessage(x);
            }
        );

        //gameboard
        gameBoard.Value = new GameBoard();
        IgameBoard = gameBoard.Value; //interfaceの宣言
        gameBoard.Value.InitializeGameBoard();

        //entrance
        entrance = this.gameObject.AddComponent<Entrance>();
        _entrance = entrance; //interfaceの受け渡し
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
    public void IndicateCard(bool isLarge, int cardID)
    {
        cardIndicatorModel.Value = _cardIndicatorModel.Value.Indicate(isLarge, cardID);
    }
    public void SetCardData()
    {
        cardDataManager = DataReader.cdm;
    }
    public void ChangeState(GameState state)
    {
        this.state.Value = state;
    }
    private void GetMessage(string msg)
    {
        string[] message = msg.Split(":::");
        if (message[0] == "0")
        {//0:::playerNum:::userName
            gameBoard.Value.PlayerID = int.Parse(message[1]); //playerのIDを設定
            this.playerID = gameBoard.Value.PlayerID;
        }
        else if (message[0] == "1")
        { //1:::playerCount:::[name1,name2,...]
            if (state.Value != GameState.wait) return;
            gameBoard.Value.PlayerCount = int.Parse(message[1]);
            playerNameList = message[2].Split(",").ToList();
            int i = 0;
            foreach (string name in playerNameList)
            {
                gameBoard.Value.playerAreaList[i].UserName = name;
                i++;
            }
            entranceObject.SetActive(false);
            gameBoardObject.SetActive(true);
            state.Value = GameState.ingame;
            Debug.Log(gameBoard.Value.playerAreaList[0].UserName + "," + gameBoard.Value.playerAreaList[1].UserName);
        }
        else if (message[0] == "2")
        { //2:::json
            if (state.Value != GameState.ingame) return;
            //受け取ったjsonをクラスに変換しGameBoardに適用
            gameBoard.Value = gameBoard.Value.ApplyNewBoard(JsonToGameBoard(message[1]), gameBoardView);
            Debug.Log(gameBoard.Value.playerAreaList[0].UserName + "," + gameBoard.Value.playerAreaList[1].UserName);
        }
        else if (message[0] == "first")
        {
        }
        else
        {
            Debug.Log("received wrong message;");
        }
    }
    public void ControlBoard(GameBoardAddress From)
    {
        GameBoardData board = new GameBoardData();
        board = gameBoard.Value.GameBoardToData(gameBoard.Value.ControlBoard(From, ToAddress));
        connector.SendText("2:::" + GameBoardToJson(board));
    }
    public void ControlLog(string text)
    {
        GameBoardData board = new GameBoardData();
        board = gameBoard.Value.GameBoardToData(gameBoard.Value.AddLog(text));
        connector.SendText("2:::" + GameBoardToJson(board));
    }

    public void RenewBoard()
    {
        GameBoardData board = new GameBoardData();
        board = gameBoard.Value.GameBoardToData(gameBoard.Value);
        connector.SendText("2:::" + GameBoardToJson(board));
    }

    public void ControlLeaderNum(int num)
    {
        GameBoardData board = new GameBoardData();
        board = gameBoard.Value.GameBoardToData(gameBoard.Value.SetLeaderNum(playerID, num));
        connector.SendText("2:::" + GameBoardToJson(board));
    }

    public void SetFromAddress(GameBoardAddress gba)
    {
        FromAddress = gba;
    }

    private string GameBoardToJson(GameBoardData gbd)
    {
        return JsonConvert.SerializeObject(gbd);
    }

    private GameBoardData JsonToGameBoard(string json)
    {
        return JsonConvert.DeserializeObject<GameBoardData>(json);
    }

    public void SetToPlayerNum(int num)
    {
        ToAddress.playerID = num;
        if (isHeroItem)
        {
            var heroCount = gameBoard.Value.playerAreaList[ToAddress.playerID].PlayerHeroCardList;
            commandPanelView.smallPanels[2].transform.Find("Dropdown").gameObject.GetComponent<TMP_Dropdown>().ClearOptions();
            List<string> optionList = new List<string>();
            int i = 0;
            foreach (HeroCard sc in heroCount)
            {
                optionList.Add(i.ToString());
                i++;
            }
            commandPanelView.smallPanels[2].transform.Find("Dropdown").gameObject.GetComponent<TMP_Dropdown>().AddOptions(optionList);
            commandPanelModel.Value = commandPanelModel.Value.TransitionSmallPanel(CommandPanelView.PanelName.order);
        }
        else
        {
            commandPanelModel.Value = commandPanelModel.Value.CloseAllPanel();
            ControlBoard(FromAddress);
        }
    }

    public void SetToPlayerNumLarge(int num)
    {
        ToAddress.playerID = num;
        commandPanelModel.Value = commandPanelModel.Value.CloseAllPanel();
        ControlBoard(FromAddress);
    }

    public void DefineToOrderNum()
    {
        ToAddress.order = commandPanelView.smallPanels[2].transform.Find("Dropdown").gameObject.GetComponent<TMP_Dropdown>().value;
        commandPanelModel.Value = commandPanelModel.Value.CloseAllPanel();
        ControlBoard(FromAddress);
    }

    public void SetToSmallArea(int num)
    {
        switch (num)
        {
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

    public void SetToLargeArea(int num)
    {
        switch (num)
        {
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

    public void SetToOrder(int num)
    {
        ToAddress.order = num;
        commandPanelModel.Value = commandPanelModel.Value.CloseAllPanel();
        ControlBoard(FromAddress);
    }

    public void SetToHeroNum()
    {
        ToAddress.order = heroNum_;
        commandPanelModel.Value = commandPanelModel.Value.CloseAllPanel();
        ControlBoard(FromAddress);
    }

    public void DrawCard()
    {
        ToAddress.playerID = this.playerID;
        ToAddress.area = Area.playerHand;
        GameBoardAddress from = new GameBoardAddress();
        from.area = Area.deck;
        ControlBoard(from);
    }
    public void DrawMonster()
    {
        if (gameBoard.Value.monsterArea.monsterCardList.Count < 3)
        {
            ToAddress.order = gameBoard.Value.monsterArea.monsterCardList.Count;
            ToAddress.area = Area.monsterList;
            GameBoardAddress from = new GameBoardAddress();
            from.area = Area.monsterDeck;
            ControlBoard(from);
        }
    }

    public void DeckShuffle()
    {
        GameBoardData board = new GameBoardData();
        board = gameBoard.Value.GameBoardToData(gameBoard.Value.DeckShuffle());
        connector.SendText("2:::" + GameBoardToJson(board));
    }
    public void ResetBoard()
    {
        GameBoard board = new GameBoard();
        GameBoardData boardData = gameBoard.Value.GameBoardToData(board.InitializeGameBoard());
        connector.SendText("2:::" + GameBoardToJson(boardData));
    }
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;//ゲームプレイ終了
#else
    Application.Quit();//ゲームプレイ終了
#endif
    }
}
public enum GameState
{
    entrance, wait, ingame
}
public class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = (T)FindObjectOfType(typeof(T));

                if (instance == null)
                {
                    Debug.LogError(typeof(T) + "���V�[���ɑ��݂��܂���B");
                }
            }

            return instance;
        }
    }
}
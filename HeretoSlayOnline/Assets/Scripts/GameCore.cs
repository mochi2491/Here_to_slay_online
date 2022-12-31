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
    GameBoard gameBoard = new GameBoard();
    public GameBoardView gameBoardView = new GameBoardView();
    public GameObject gameBoardObject;

    //Game�̃X�e�[�g
    private ReactiveProperty<GameState> state = new ReactiveProperty<GameState>(GameState.entrance);

    //Entrance
    public GameObject entrance;
    private string userName="";
    private bool isReady = false;
    public int playerID = 0;

    //GUI
    public GameObject[] tabs;
    private IntReactiveProperty visibleTabNum = new IntReactiveProperty(0);
    public GameObject smallCommandPanel;
    public List<GameObject> smallPanels;
    public GameObject largeCommandPanel;
    public List<GameObject> largePanels;

    public bool isHeroItem = false;
    public TMP_Dropdown heroNum;
    public GameBoardAddress FromAddress = new GameBoardAddress();
    public GameBoardAddress ToAddress = new GameBoardAddress();
   
    void Start()
    {
        heroNum = smallPanels[2].transform.Find("Dropdown").gameObject.GetComponent<TMP_Dropdown>();
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
            gameBoard.PlayerID = int.Parse(message[1]); //player��ID��ݒ�
            this.playerID = gameBoard.PlayerID;
        }
        else if (message[0] == "1") { //1:::playerCount 
            if (state.Value != GameState.wait) return;
            gameBoard.PlayerCount = int.Parse(message[1]);
            if (gameBoard.PlayerID == 0){
                gameBoard.InitializeGameBoard();
            }
            entrance.SetActive(false);
            gameBoardObject.SetActive(true);
            state.Value = GameState.ingame;
        }
        else if (message[0] == "2") { //2:::json
            if (state.Value != GameState.ingame) return;

            //�󂯎����json���N���X�ɕϊ���GameBoard�ɓK�p
            gameBoard.ApplyNewBoard(JsonToGameBoard(message[1]),gameBoardView);
        }
        else if (message[0] == "first") {
        }
        else {
            Debug.Log("received wrong message;");
        }
    }
    
    public void ControlBoard(GameBoardAddress From) {
        GameBoardData board = new GameBoardData();
        Debug.Log(From.area+","+ToAddress.area);
        board = gameBoard.GameBoardToData(gameBoard.ControlBoard(From, ToAddress));
        connector.SendText("2:::"+GameBoardToJson(board));
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

    //command button method
    public void OpenCommandPanel(bool isLarge,Vector2 pos) {
        CloseCommandPanel();
        if (isLarge) {
            largeCommandPanel.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(pos.x, pos.y, 10.0f));
            largeCommandPanel.SetActive(true);
        }
        else {
            smallCommandPanel.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(pos.x , pos.y, 10.0f));
            smallCommandPanel.SetActive(true);
        }
    }
    public void CloseCommandPanel() {
        largeCommandPanel.SetActive(false);
        largePanels[0].SetActive(true);
        largePanels[1].SetActive(false);
        largePanels[2].SetActive(false);
        smallCommandPanel.SetActive(false);
        smallPanels[0].SetActive(true);
        smallPanels[1].SetActive(false);
        smallPanels[2].SetActive(false);
    }
    public void SetToPlayerNum(int num) {
        ToAddress.playerID = num;
        if (isHeroItem) {
            var heroCount = gameBoard.playerAreaList[ToAddress.playerID].PlayerHeroCardList;
            smallPanels[2].transform.Find("Dropdown").gameObject.GetComponent<TMP_Dropdown>().ClearOptions();
            List<string> optionList = new List<string>();
            int i = 0;
            foreach(HeroCard sc in heroCount) {
                optionList.Add(i.ToString());
                i++;
            }
            smallPanels[2].transform.Find("Dropdown").gameObject.GetComponent<TMP_Dropdown>().AddOptions(optionList);

            smallPanels[1].SetActive(false);
            smallPanels[2].SetActive(true);

        }
        else {
            smallCommandPanel.SetActive(false);
            ControlBoard(FromAddress);
        }
    }
    public void SetToPlayerNumLarge(int num) {
        ToAddress.playerID = num;
        smallCommandPanel.SetActive(false);
        ControlBoard(FromAddress);
    }
    public void DefineToOrderNum() {
        ToAddress.order = smallPanels[2].transform.Find("Dropdown").gameObject.GetComponent<TMP_Dropdown>().value;
        smallPanels[2].SetActive(false);
        smallCommandPanel.SetActive(false);
        ControlBoard(FromAddress);
    }
    public void SetToSmallArea(int num) {
        switch (num) {
            case 0:
                ToAddress.area = Area.playerHand;
                smallPanels[0].SetActive(false);
                smallPanels[1].SetActive(true);
                isHeroItem = false;
                break;
            case 1:
                ToAddress.area = Area.playerHero;
                smallPanels[0].SetActive(false);
                smallPanels[1].SetActive(true);
                isHeroItem = false;
                break;
            case 2:
                ToAddress.area = Area.discardPile;
                smallCommandPanel.SetActive(false);
                ControlBoard(FromAddress);
                break;
            case 3:
                ToAddress.area = Area.playerHeroItem;
                smallPanels[0].SetActive(false);
                smallPanels[1].SetActive(true);
                isHeroItem = true; 
                break;
            case 4:
                ToAddress.area = Area.deck;
                smallCommandPanel.SetActive(false);
                ControlBoard(FromAddress);
                break;
        }
    }
    public void SetToLargeArea(int num) {
        switch (num) {
            case 0:
                ToAddress.area = Area.slayedMonster;
                largePanels[0].SetActive(false);
                largePanels[1].SetActive(true); //next player
                break;
            case 1:
                ToAddress.area = Area.monsterDeck;
                largeCommandPanel.SetActive(false);
                ControlBoard(FromAddress);
                break;
            case 2:
                ToAddress.area = Area.monsterList;
                largePanels[0].SetActive(false);
                largePanels[2].SetActive(true); //next order
                break;
        }
    }
    public void SetToOrder(int num) {
        ToAddress.order = num;
        largeCommandPanel.SetActive(false);
        ControlBoard(FromAddress);
    }
    public void SetToHeroNum() {
        ToAddress.order = heroNum.value;
        smallPanels[2].SetActive(false);
        smallCommandPanel.SetActive(false);
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
        if (gameBoard.monsterArea.monsterCardList.Count < 3) { 
            ToAddress.order = gameBoard.monsterArea.monsterCardList.Count;
            ToAddress.area = Area.monsterList;
            GameBoardAddress from = new GameBoardAddress();
            from.area = Area.monsterDeck;
            ControlBoard(from);
        }
    }
    public void DeckShuffle() {
        GameBoardData board = new GameBoardData();
        board = gameBoard.GameBoardToData(gameBoard.DeckShuffle());
        connector.SendMessage("2:::" + GameBoardToJson(board));
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
    public MonsterArea monsterArea = new MonsterArea();
    private ChatArea chatArea = new ChatArea();
    public List<PlayerArea> playerAreaList = new List<PlayerArea>();
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
    
    public GameBoard InitializeGameBoard() {
        smallCardImageList = Resources.LoadAll("deck_cards",typeof(Sprite)).Cast<Sprite>().ToArray();
        largeCardImageList = Resources.LoadAll("monster_and_leader_cards",typeof(Sprite)).Cast<Sprite>().ToArray();
        cardBack = Resources.Load("back") as Sprite;
        deckArea.Init();
        monsterArea.Init();
        chatArea.Init();
        for (int i = 0; i < 6; i++) playerAreaList.Add(new PlayerArea());
        foreach (PlayerArea pa in playerAreaList) {
            pa.Init(0);
        }
        return this;
    } //������
    public void ApplyNewBoard(GameBoardData gbd,GameBoardView gbv) {
        deckArea.DataToDeck(gbd.mainDeck);
        deckArea.DataToPile(gbd.discardPile);
        monsterArea.DataToDeck(gbd.monsterDeck);
        monsterArea.DataToList(gbd.monsterCardList);
        DataToPlayerList(gbd.playerList);
        ApplyView(gbd,gbv);
    } //GameBoard���X�V����
    public void ApplyView(GameBoardData gbd,GameBoardView gbv) {
        gbv.ApplyHand(gbd.playerList[playerID].playerHandList,smallCardImageList); //��D�Ƀf�[�^��K�p
        for(int i = 0; i < 6; i++) {
            gbv.ApplyHero(gbd.playerList[i].playerHeroCardList,smallCardImageList,i);
            gbv.ApplySlayedMonster(gbd.playerList[i].slayedMonsterList,largeCardImageList,i);
        } //�q�[���[���X�g�Ƀf�[�^��K�p
        gbv.ApplyDiscardPile(gbd.discardPile, smallCardImageList);
        gbv.ApplyMonster(gbd.monsterCardList, largeCardImageList);
        
    } //�X�V����GameBoard����ʂɓK�p����
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
    public GameBoard DeckShuffle() {
        deckArea.Shuffle();
        return this;
    }
    public GameBoard ControlBoard(GameBoardAddress From,GameBoardAddress To) {
        bool isLarge = false;
        if((From.area == Area.monsterList) || (From.area == Area.monsterDeck) || (From.area == Area.slayedMonster)) {
            isLarge = true;
        }
        //�J�[�h�����ǂ�������
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
                    monsterArea.PushList(moveCard,To.order);
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
                    HeroCard a = playerAreaList[From.playerID].PickHeroCard(From.order);
                    moveCard = a.hero;
                    if(a.equip.ID != -1) playerAreaList[playerID].PushHand(a.equip);
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
                    HeroCard a = new HeroCard(moveCard, null);
                    playerAreaList[To.playerID].PushHeroCard(a);
                    break;
            }
        }
        return this;
    } //�J�[�h���ړ�������(�o�J����������Ȃ̂ł����꒼��)

    
}
public struct GameBoardAddress {
    public Area area;
    public int playerID;
    public int order;
}
public enum Area {
    deck,discardPile, monsterList,monsterDeck,playerHand,playerHero,playerHeroItem,slayedMonster
}
public class DeckArea : MonoBehaviour {
    private List<SmallCard> mainDeck = new List<SmallCard>();
    private List<SmallCard> discardPile = new List<SmallCard>();
    public void Init() {
        //mainDeck init
        for(int i = 0; i <= 73; i++) {
            if (i == 52 || i == 54 || i == 57 || (i >= 66 && i <= 69) || i == 72) { //2��
                mainDeck.Add(new SmallCard(i, ""));
                mainDeck.Add(new SmallCard(i, ""));
            }
            else if (i == 60 || (i >= 62 && i <= 64)) { //4��
                for (int j = 0; j < 4; j++) mainDeck.Add(new SmallCard(i, ""));
            }
            else if (i == 61) { //9��
                for (int k = 0; k < 9; k++) mainDeck.Add(new SmallCard(i, ""));
            }
            else if (i == 73) { //14��
                for (int l = 0; l < 14; l++) mainDeck.Add(new SmallCard(i, ""));
            }
            else mainDeck.Add(new SmallCard(i, "")); //1��

        }
        mainDeck = mainDeck.OrderBy(a => Guid.NewGuid()).ToList();
        //discardPile init
        discardPile.Clear();
    }
    public int deckCount() {
        return mainDeck.Count;
    }
    public void Shuffle() {
        mainDeck = mainDeck.OrderBy(a => Guid.NewGuid()).ToList();
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
            discardPile.Add(new SmallCard(id, ""));
        }
    } //data��DiscardPile��
    public SmallCard PopDeck() {
        SmallCard tmp = mainDeck[0];
        mainDeck.RemoveAt(0);
        return tmp;
    } //deck�̓������o��
    public void PushDeck(SmallCard tmp) {
        mainDeck.Insert(0, tmp);
    } //deck�̓��ɃJ�[�h��ǉ�
    public SmallCard PickDiscard(int order) {
        SmallCard tmp = discardPile[order];
        discardPile.RemoveAt(order);
        return tmp;
    } //discardPile�̎w��̃J�[�h�����o��
    public void PushDiscard(SmallCard tmp) {
        discardPile.Add(tmp);
    } //discardPile�̍Ō�ɃJ�[�h��ǉ�
}
public class MonsterArea : MonoBehaviour {
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
    public LargeCard PopDeck() {
        LargeCard tmp = monsterDeck[0];
        monsterDeck.RemoveAt(0);
        return tmp;
    } //monsterDeck�̐擪�����o��
    public void PushDeck(LargeCard tmp) {
        monsterDeck.Insert(0, tmp);
    } //monsterDeck�̓��ɃJ�[�h��ǉ�
    public LargeCard PopList(int order) {
        LargeCard tmp = monsterCardList[order];
        monsterCardList.RemoveAt(order);
        return tmp;
    } //monsterList����J�[�h�����o��
    public void PushList(LargeCard tmp,int order) {
        if (monsterCardList.Count < 3) monsterCardList.Add(tmp);
        else {
            PushDeck(monsterCardList[order]);
            monsterCardList.RemoveAt(order);
            monsterCardList.Add(tmp); 
        }
    } //monsterList��3�������Ȃ�J�[�h��ǉ�����,3���ȏ�Ȃ�ꖇ�߂��Ă���ǉ�����

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
        this.leaderCard = playerData.leaderCardID;
        DataToHand(playerData.playerHandList);
        DataToHeroList(playerData.playerHeroCardList);
        DataToSlayedList(playerData.slayedMonsterList);
    } //�R���X�g���N�^
    //method
    public void Init(int num) {
        leaderCard = num;
        playerHandList.Clear();
        playerHeroCardList.Clear();
    } //������
    
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
    } //data���v���C���[�̏���
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
    } //�q�[���[���X�g��data��
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
    public SmallCard PickHand(int order) {
        SmallCard tmp = playerHandList[order];
        playerHandList.RemoveAt(order);
        return tmp;
    } //��D����ꖇ�J�[�h�����o��
    public void PushHand(SmallCard tmp) {
        playerHandList.Add(tmp);
    }//�J�[�h����D�ɒǉ�����
    public HeroCard PickHeroCard(int order) {
        Debug.Log(playerHeroCardList.Count);
        HeroCard tmp = playerHeroCardList[order];
        playerHeroCardList.RemoveAt(order);
        return tmp;
    }//�q�[���[���X�g�̒�����J�[�h���ꖇ���o��
    public void PushHeroCard(HeroCard tmp) {
        playerHeroCardList.Add(tmp);
    }//�q�[���[���X�g�ɃJ�[�h��ǉ�����
    public SmallCard PickArmedCard(int order) {
        SmallCard tmp = playerHeroCardList[order].equip;
        playerHeroCardList[order].equip = null;
        return tmp;
    } //�q�[���[���������Ă���A�C�e�������o��
    public void AttachArmedCard(SmallCard tmp,int order) {
        Debug.Log(playerHeroCardList.Count+","+ order);
        if (order >= playerHeroCardList.Count) throw new Exception("���݂��Ȃ��q�[���[���w�肵�Ă��܂�");
        playerHeroCardList[order].equip = tmp;
    } //�q�[���[�ɃA�C�e���𑕔�������
    public LargeCard PickSlayedMonster(int order) {
        LargeCard tmp = slayedMonsterList[order];
        slayedMonsterList.RemoveAt(order);
        return tmp;
    } //�������������X�^�[���X�g����J�[�h���ꖇ���o��
    public void PushSlayedMonster(LargeCard tmp) {
        slayedMonsterList.Add(tmp);
    } //�������������X�^�[���X�g�ɃJ�[�h��ǉ�����


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
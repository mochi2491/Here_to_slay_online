using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using Unity.VisualScripting;

public class CardView : MonoBehaviour
{
    //instance
    private int heroID = 0;
    private int itemID = 0;
    private bool isLarge = false;
    [SerializeField] private Image heroImage = null;
    [SerializeField] private Image itemImage = null;
    public ObservableEventTrigger heroTrigger;
    public ObservableEventTrigger itemTrigger;

    public int HeroID {
        get { return heroID; }
    }
    public int ItemID {
        get { return itemID; }
    }
    public bool IsLarge {
        get { return isLarge; }
    }
    public void Awake() {
        heroTrigger = this.transform.Find("hero").AddComponent<ObservableEventTrigger>();
        itemTrigger = this.transform.Find("item").AddComponent<ObservableEventTrigger>();
    }
    public void ApplyData(Sprite heroSprite,Sprite itemSprite) {
        heroImage.sprite = heroSprite;
        itemImage.sprite = itemSprite;
    }
    public void SetData(int heroID,int itemID,bool isLarge) {
        this.heroID= heroID;
        this.itemID= itemID;
        this.isLarge= isLarge;
    }
    public void DestroySelf() {
        Destroy(this.gameObject);
    }
    /*
    private int? cardID = null;
    private Sprite cardSprite = null;
    private int? itemID = null;
    private Image image = null;
    private Image itemImage = null;
    private int holderNum = 0;
    private int orderNum = 0;　//領域の中で何個目に表示されているか
    private Area area;
    private EventTrigger trigger;
    public GameCore gameCore;
    public bool isLarge = false;

    private void Start() {
        trigger = this.transform.Find("Card").gameObject.AddComponent<EventTrigger>();
        trigger.triggers = new List<EventTrigger.Entry>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback.AddListener((eventData) => { OnClick(eventData); });
        trigger.triggers.Add(entry);
    }
    public int OrderNum {
        get { return orderNum; }
    }
    public void OnClick(BaseEventData ped) {
        PointerEventData pointerEventData = (PointerEventData)ped;
        GameBoardAddress gba = new GameBoardAddress();
        gba.area = this.area;
        gba.order = this.orderNum;
        gba.playerID = holderNum;
        gameCore.SetFromAddress(gba);
        
        if(pointerEventData.pointerId == -2) {
            //gameCore.commandPanelView.OpenCommandPanel(isLarge, Input.mousePosition);
            if (isLarge) gameCore.commandPanelModel.Value = gameCore.commandPanelModel.Value.OpenLargeCommandPanel(CommandPanelView.PanelName.main,Input.mousePosition);
            else gameCore.commandPanelModel.Value = gameCore.commandPanelModel.Value.OpenSmallCommandPanel(CommandPanelView.PanelName.main, Input.mousePosition);
        }
        else if(pointerEventData.pointerId == -1) {
            gameCore.leftClickIsLarge = isLarge;
            gameCore.SetClickedID((int)cardID);
        }
    }
    public void ApplyData(int id, Sprite sprite,int orderNum,Area area,GameCore gameCore,int holderNum,bool isLarge){
        this.cardID = id;
        this.cardSprite = sprite;
        this.image = this.transform.Find("Card").GetComponent<Image>();
        image.sprite = cardSprite;
        this.orderNum = orderNum;
        this.area = area;
        this.gameCore = gameCore;
        this.holderNum = holderNum;
        this.isLarge = isLarge;
    }
    public void ApplyHeroData(int id, Sprite sprite, int itemID,Sprite itemSprite,int orderNum ,Area area,GameCore gameCore,int holderNum,bool isLarge) {
        this.cardID = id;
        this.cardSprite = sprite;
        this.image = this.transform.Find("Card").GetComponent<Image>();
        this.itemImage = this.transform.Find("Item").GetComponent<Image>();
        if (itemSprite != null) this.itemImage.enabled = true;
        else this.itemImage.enabled = false;
        image.sprite = cardSprite;
        itemImage.sprite = itemSprite;
        this.area = area;
        this.orderNum=orderNum;
        this.gameCore = gameCore;
        this.holderNum = holderNum;
        this.isLarge=isLarge;
    }
    public void DestroySelf() {
        Destroy(this.gameObject);
    }
    public int? DisarmItem() {
        int? itemTmp = itemID;
        itemImage.sprite = null;
        itemID = null;
        return itemTmp;
    }*/
}

public class SmallCardView : MonoBehaviour {
    private int? cardID = null;
    private Sprite cardSprite = null;
    private Image image = null;

    public SmallCardView(int cardID, Sprite cardSprite) {
        this.cardID = cardID;
        this.cardSprite = cardSprite;
        this.image = this.gameObject.GetComponent<Image>();
    }
}

public class LargeCardView : MonoBehaviour {
    private int? cardID = null;
    private Sprite cardSprite = null;
    public LargeCardView(int cardID, Sprite cardSprite) { 
        this.cardID=cardID;
        this.cardSprite=cardSprite;
        this.GetComponent<Image>().sprite = cardSprite;
    }
}

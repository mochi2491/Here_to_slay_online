using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;

public class CardView : MonoBehaviour
{
    private int? cardID = null;
    private Sprite cardSprite = null;
    private int? itemID = null;
    private Image image = null;
    private Image itemImage = null;
    private int holderNum = 0;
    private int orderNum = 0;Å@//óÃàÊÇÃíÜÇ≈âΩå¬ñ⁄Ç…ï\é¶Ç≥ÇÍÇƒÇ¢ÇÈÇ©
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

        EventTrigger.Entry entry2 = new EventTrigger.Entry();
        entry2.eventID = EventTriggerType.Cancel;
        entry2.callback.AddListener(_ => { Cancel(); });
        trigger.triggers.Add(entry2);

        EventTrigger.Entry entry3 = new EventTrigger.Entry();
        entry3.eventID = EventTriggerType.PointerClick;
        entry3.callback.AddListener(_ => { OnLeftClick(); });
        trigger.triggers.Add(entry3);

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
            gameCore.OpenCommandPanel(isLarge, Input.mousePosition);
        }
    }
    public void OnLeftClick() {
        gameCore.leftClickIsLarge = isLarge;
        gameCore.leftClickedID.Value = (int)cardID;
    }
    
    public void Cancel() {
        gameCore.CloseCommandPanel();
        Debug.Log("aaa");
    }
    public void Update() {
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
    }
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

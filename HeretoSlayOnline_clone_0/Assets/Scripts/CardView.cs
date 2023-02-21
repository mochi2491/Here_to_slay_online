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
    [SerializeField] private Image heroImage;
    [SerializeField] private Image itemImage;
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
    public void ApplyData(Sprite heroSprite,Sprite itemSprite) {
        heroImage.sprite = heroSprite;
        itemImage.sprite = itemSprite;
    }
    public void SetData(int heroID,int itemID,bool isLarge) {
        this.heroID= heroID;
        this.itemID= itemID;
        this.isLarge= isLarge;
        if(itemID == -1) {
            this.transform.Find("item").gameObject.SetActive(false);
        }
    }
    public void DestroySelf() {
        Destroy(this.gameObject);
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

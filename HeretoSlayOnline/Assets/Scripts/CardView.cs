using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardView : MonoBehaviour
{
    private int? cardID = null;
    private Sprite cardSprite = null;
    private Image image = null;

    public CardView(int cardID,Sprite cardSprite) {
        this.cardID = cardID;
        this.cardSprite = cardSprite;
        this.image = this.gameObject.GetComponent<Image>();
        image.sprite = cardSprite;
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

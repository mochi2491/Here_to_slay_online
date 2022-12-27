using UnityEngine;
using UnityEngine.UI;

public class CardView : MonoBehaviour
{
    private int? cardID = null;
    private Sprite cardSprite = null;
    private int? itemID = null;
    private Image image = null;
    private Image itemImage = null;
    private int orderNum=0;　//領域の中で何個目に表示されているか
    private Area area = Area.deck;

    public int OrderNum {
        get { return orderNum; }
    }

    public void ApplyData(int id, Sprite sprite,int orderNum,Area are){
        this.cardID = id;
        this.cardSprite = sprite;
        this.image = this.transform.Find("Card").GetComponent<Image>();
        image.sprite = cardSprite;
        this.orderNum = orderNum;
        this.area = are;
    }
    public void ApplyHeroData(int id, Sprite sprite, int itemID,Sprite itemSprite,int orderNum ,Area area) {
        this.cardID = id;
        this.cardSprite = sprite;
        this.image = this.transform.Find("Card").GetComponent<Image>();
        this.itemImage = this.transform.Find("Item").GetComponent<Image>();
        image.sprite = cardSprite;
        itemImage.sprite = itemSprite;
        this.orderNum=orderNum;
        this.area = area;
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

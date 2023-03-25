using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PeepPanelView : MonoBehaviour
{
    public GameObject peepPanel;
    public GameObject content;
    public Button quitButton;
    private void Start() {
        peepPanel.SetActive(false);
    }
    private void Reset(GameObject content) {
        foreach (CardView a in content.GetComponentsInChildren<CardView>()) {
            a.DestroySelf();
        }
    }
    public void ApplyView(List<SmallCard> list) {
        Reset(content);
        foreach(SmallCard sc in list) {
            GameObject card = (GameObject)Resources.Load("CardObject");
            GameObject a = Instantiate(card, content.transform);
            CardView view = a.GetComponent<CardView>();
            view.ApplyData(CardSprites.GetSprite(sc.ID,false), CardSprites.GetNullSprite());
            view.SetData(sc.ID, -1, false);
        }
    }
}

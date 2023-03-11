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
    [SerializeField] private Sprite nullSprite;
    private Sprite[] smallCardImageList = new Sprite[GameBoard.SMALLCARD_COUNT];
    private Sprite[] largeCardImageList = new Sprite[GameBoard.LARGECARD_COUNT];
    private void Awake() {
        smallCardImageList = Resources.LoadAll("deck_cards", typeof(Sprite)).Cast<Sprite>().ToArray();
        largeCardImageList = Resources.LoadAll("monster_and_leader_cards", typeof(Sprite)).Cast<Sprite>().ToArray();
    }
    private void Start() {
        peepPanel.SetActive(false);
    }
    public void ApplyView(List<SmallCard> list) {
        foreach(SmallCard sc in list) {
            GameObject card = (GameObject)Resources.Load("CardObject");
            GameObject a = Instantiate(card, content.transform);
            CardView view = a.GetComponent<CardView>();
            view.ApplyData(smallCardImageList[sc.ID], nullSprite);
            view.SetData(sc.ID, -1, false);
        }
    }
}

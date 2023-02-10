using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

public class GameBoardView : MonoBehaviour
{
    public GameCore gameCore;
    public GameObject handObject;
    public List<GameObject> heroObject;
    public List<GameObject> slayedmonsterObject;
    public GameObject discardpileObject;
    public GameObject monsterObject;

    public List<CardView> allCardList;

    [SerializeField]private Sprite nullSprite;
    private Sprite[] smallCardImageList = new Sprite[GameBoard.SMALLCARD_COUNT];
    private Sprite[] largeCardImageList = new Sprite[GameBoard.LARGECARD_COUNT];

    //private methods
    private void Reset(GameObject content) { 
        foreach(CardView a in content.GetComponentsInChildren<CardView>()) {
            a.DestroySelf();
        }
    }
    private void Apply(List<int> data,GameObject parent,Area area,int holderNum,bool isLarge) {
        int i = 0;
        foreach (int id in data) {
            GameObject card = (GameObject)Resources.Load("CardObject");
            GameObject a = Instantiate(card, parent.transform);
            CardView view = a.GetComponent<CardView>();
            if(id != -1) {
                if (isLarge) {
                    view.ApplyData(largeCardImageList[id], nullSprite);
                }
                else view.ApplyData(smallCardImageList[id], nullSprite);
            }
            else {
                view.ApplyData(nullSprite,nullSprite);
            }
            view.SetData(data[i], -1, isLarge);
            i++;
            allCardList.Add(view);
        }
    }
    private void Apply(List<int>data ,GameObject parent,Area area,int holderNum,bool isLarge,List<int> armedCardData) {
        for (int i = 0; i < data.Count; i++) {
            GameObject card = (GameObject)Resources.Load("CardObject");
            GameObject a = Instantiate(card, parent.transform);
            CardView view = a.AddComponent<CardView>();
            if (armedCardData[i] != -1) view.ApplyData(smallCardImageList[data[i]], nullSprite);
            else view.ApplyData(largeCardImageList[data[i]], smallCardImageList[armedCardData[i]]);
            view.SetData(data[i], armedCardData[i], isLarge);
            allCardList.Add(view);
        }
    }

    //public methods
    public void ApplyHand(List<int> data){
        Reset(handObject);
        Apply(data,handObject,Area.playerHand,gameCore.playerID,false);

    }
    public void ApplyHero(List<int> heroData, List<int> armedCardData,int playerNum){
        Reset(heroObject[playerNum]);
        Apply(heroData,heroObject[playerNum],Area.playerHero,playerNum,false,armedCardData);
           
    }
    public void ApplySlayedMonster(List<int> data,int playerNum){
        Reset(slayedmonsterObject[playerNum]);
        Apply(data,slayedmonsterObject[playerNum],Area.slayedMonster,playerNum,true);
    }
    public void ApplyDiscardPile(List<int> data){
        Reset(discardpileObject);
        Apply(data,discardpileObject,Area.discardPile,0, false);
    }
    public void ApplyMonster(List<int> data){
        Reset(monsterObject);
        Apply(data, monsterObject,Area.monsterList,0,true);
    }

    private void Awake() {
        smallCardImageList = Resources.LoadAll("deck_cards",typeof(Sprite)).Cast<Sprite>().ToArray();
        largeCardImageList = Resources.LoadAll("monster_and_leader_cards",typeof(Sprite)).Cast<Sprite>().ToArray();
    }
}

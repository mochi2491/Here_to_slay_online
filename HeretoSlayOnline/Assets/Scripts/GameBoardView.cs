using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using UniRx;
using UnityEngine;
using Zenject.Asteroids;

public class GameBoardView : MonoBehaviour
{
    public GameCore gameCore;
    public GameObject handObject;
    public List<GameObject> heroObject;
    public List<GameObject> slayedmonsterObject;
    public GameObject discardpileObject;
    public GameObject monsterObject;

    public GameObject cardImageIndicator;
    public List<GameObject> allCardList;

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
            if (view.heroTrigger == null) Debug.Log("gg");
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
            allCardList.Add(a);
        }
    }
    private void Apply(List<int>data ,GameObject parent,Area area,int holderNum,bool isLarge,List<int> armedCardData) {
        for (int i = 0; i < data.Count; i++) {
            GameObject card = (GameObject)Resources.Load("CardObject");
            GameObject a = Instantiate(card, parent.transform);
            CardView view = a.GetComponent<CardView>();
            if (armedCardData[i] != -1) view.ApplyData(smallCardImageList[data[i]], smallCardImageList[armedCardData[i]]);
            else {
                view.ApplyData(smallCardImageList[data[i]], nullSprite); 
            }
            view.SetData(data[i], armedCardData[i], isLarge);
            allCardList.Add(a);
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
    public void OpenIndicator(int cardID , bool isLarge,GameObject card) {
        float movex = 0.5f;
        float movey = 0.5f;
        if (isLarge) {
            movex = 1;
            movey = -2.5f;
        }   
        Vector3 a = new Vector3(card.transform.position.x+movex,card.transform.position.y+movey,card.transform.position.z);
        cardImageIndicator.transform.position = a;
        if (isLarge) cardImageIndicator.GetComponent<Image>().sprite = largeCardImageList[cardID];
        else cardImageIndicator.GetComponent<Image>().sprite = smallCardImageList[cardID];
        cardImageIndicator.SetActive(true);
    }
    public void CloseIndicator() {
        cardImageIndicator.SetActive(false);
    }

    private void Awake() {
        smallCardImageList = Resources.LoadAll("deck_cards",typeof(Sprite)).Cast<Sprite>().ToArray();
        largeCardImageList = Resources.LoadAll("monster_and_leader_cards",typeof(Sprite)).Cast<Sprite>().ToArray();
    }
}

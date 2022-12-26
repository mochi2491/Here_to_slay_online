using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoardView : MonoBehaviour
{
    //手札表示
    public GameObject handObject;
    //各プレイヤーのヒーロー
    public List<GameObject> heroObject = new List<GameObject>();
    //倒したモンスター
    public List<GameObject> slayedmonsterObject = new List<GameObject>();
    //捨て札
    public GameObject discardpileObject;
    //モンスター
    public GameObject monsterObject;

    //private methods
    private void Reset(GameObject content) { 
        foreach(CardView a in content.GetComponentsInChildren<CardView>()) {
            a.DestroySelf();
        }
    }
    private void Apply(List<int> data, Sprite[] sprites,GameObject parent) {
        int i = 0;
        foreach (int id in data) {
            GameObject card = (GameObject)Resources.Load("Card");
            CardView view = card.AddComponent<CardView>();
            view.ApplyData(id, sprites[id],i);
            Instantiate(card, parent.transform);
            i++;
        }
    }
    private void ApplyHero(List<HeroCard> data , Sprite[] sprites,GameObject parent) {
        int i = 0;
        foreach (HeroCard hero in data) {
            GameObject card = (GameObject)Resources.Load("Card");
            CardView view = card.AddComponent<CardView>();
            view.ApplyHeroData(hero.cardID, sprites[hero.cardID], hero.armedCardID, sprites[hero.armedCardID],i);
            Instantiate(card, parent.transform);
            i++;
        }
    }

    //public methods
    public void ApplyHand(List<int> data, Sprite[] sprites){
        //handを初期化
        Reset(handObject);
        Apply(data, sprites,handObject);
    } //手札にデータを適用
    public void ApplyHero(List<HeroCard> data, Sprite[] sprites,int playerNum){
        Reset(heroObject[playerNum]);
        ApplyHero(data, sprites, heroObject[playerNum]);
    } //ヒーローリストにデータを適用
    public void ApplySlayedMonster(List<int> data, Sprite[] sprite,int playerNum){
        Reset(slayedmonsterObject[playerNum]);
        Apply(data, sprite, slayedmonsterObject[playerNum]);
    } //倒したモンスターリストにデータを適用
    public void ApplyDiscardPile(List<int> data, Sprite[] sprite){
        Reset(discardpileObject);
        Apply(data, sprite,discardpileObject);
    }
    public void ApplyMonster(List<int> data, Sprite[] sprite){
        Reset(monsterObject);
        Apply(data, sprite, monsterObject);
    }

    private void Start() {
        
    }
}

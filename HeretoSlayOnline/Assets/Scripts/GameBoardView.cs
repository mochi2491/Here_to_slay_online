using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoardView : MonoBehaviour
{
    public GameCore gameCore;

    //手札表示
    public GameObject handObject;
    //各プレイヤーのヒーロー
    public List<GameObject> heroObject;
    //倒したモンスター
    public List<GameObject> slayedmonsterObject;
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
    private void Apply(List<int> data, Sprite[] sprites,GameObject parent,Area area,int holderNum,bool isLarge) {
        int i = 0;
        foreach (int id in data) {
            GameObject card = (GameObject)Resources.Load("Card");
            GameObject a = Instantiate(card, parent.transform);
            CardView view = a.AddComponent<CardView>();
            if(id != -1) view.ApplyData(id, sprites[id], i, area, gameCore, holderNum, isLarge);
            else view.ApplyData(id, null, i, area, gameCore, holderNum, isLarge);
            i++;
        }
    }
    private void ApplyHero(List<HeroCardData> data , Sprite[] sprites,GameObject parent,Area area,int holderNum,bool isLarge) {
        int i = 0;
        foreach (HeroCardData hero in data) {
            GameObject card = (GameObject)Resources.Load("Card");
            GameObject a = Instantiate(card, parent.transform);
            CardView view = a.AddComponent<CardView>();
            if(hero.armedCardID != -1) view.ApplyHeroData(hero.cardID, sprites[hero.cardID], hero.armedCardID, sprites[hero.armedCardID],i,area,gameCore,holderNum,isLarge);
            else view.ApplyHeroData(hero.cardID, sprites[hero.cardID], hero.armedCardID, null, i, area, gameCore, holderNum, isLarge);
            i++;
        }
    }

    //public methods
    public void ApplyHand(List<int> data, Sprite[] sprites){
        //handを初期化
        Reset(handObject);
        Apply(data, sprites,handObject,Area.playerHand,gameCore.playerID,false);

    } //手札にデータを適用
    public void ApplyHero(List<HeroCardData> data, Sprite[] sprites,int playerNum){
        Reset(heroObject[playerNum]);
        ApplyHero(data, sprites, heroObject[playerNum],Area.playerHero,playerNum,false);
    } //ヒーローリストにデータを適用
    public void ApplySlayedMonster(List<int> data, Sprite[] sprite,int playerNum){
        Reset(slayedmonsterObject[playerNum]);
        Apply(data, sprite, slayedmonsterObject[playerNum],Area.slayedMonster,playerNum,true);
    } //倒したモンスターリストにデータを適用
    public void ApplyDiscardPile(List<int> data, Sprite[] sprite){
        Reset(discardpileObject);
        Apply(data, sprite,discardpileObject,Area.discardPile,0, false);
    }
    public void ApplyMonster(List<int> data, Sprite[] sprite){
        Reset(monsterObject);
        Apply(data, sprite, monsterObject,Area.monsterList,0,true);
    }

    private void Start() {
        
    }
}

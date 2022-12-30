using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoardView : MonoBehaviour
{
    public GameCore gameCore;

    //��D�\��
    public GameObject handObject;
    //�e�v���C���[�̃q�[���[
    public List<GameObject> heroObject = new List<GameObject>();
    //�|���������X�^�[
    public List<GameObject> slayedmonsterObject = new List<GameObject>();
    //�̂ĎD
    public GameObject discardpileObject;
    //�����X�^�[
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
            CardView view = card.AddComponent<CardView>();
            view.ApplyData(id, sprites[id],i,area,gameCore,holderNum,isLarge);
            Instantiate(card, parent.transform);
            i++;
        }
    }
    private void ApplyHero(List<HeroCard> data , Sprite[] sprites,GameObject parent,Area area,int holderNum,bool isLarge) {
        int i = 0;
        foreach (HeroCard hero in data) {
            GameObject card = (GameObject)Resources.Load("Card");
            CardView view = card.AddComponent<CardView>();
            view.ApplyHeroData(hero.cardID, sprites[hero.cardID], hero.armedCardID, sprites[hero.armedCardID],i,area,gameCore,holderNum,isLarge);
            Instantiate(card, parent.transform);
            i++;
        }
    }

    //public methods
    public void ApplyHand(List<int> data, Sprite[] sprites){
        //hand��������
        Reset(handObject);
        Apply(data, sprites,handObject,Area.playerHand,gameCore.playerID,false);

    } //��D�Ƀf�[�^��K�p
    public void ApplyHero(List<HeroCard> data, Sprite[] sprites,int playerNum){
        Reset(heroObject[playerNum]);
        ApplyHero(data, sprites, heroObject[playerNum],Area.playerHero,playerNum,false);
    } //�q�[���[���X�g�Ƀf�[�^��K�p
    public void ApplySlayedMonster(List<int> data, Sprite[] sprite,int playerNum){
        Reset(slayedmonsterObject[playerNum]);
        Apply(data, sprite, slayedmonsterObject[playerNum],Area.slayedMonster,playerNum,true);
    } //�|���������X�^�[���X�g�Ƀf�[�^��K�p
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoardView : MonoBehaviour
{
    public GameCore gameCore;

    //��D�\��
    public GameObject handObject;
    //�e�v���C���[�̃q�[���[
    public List<GameObject> heroObject;
    //�|���������X�^�[
    public List<GameObject> slayedmonsterObject;
    //�̂ĎD
    public GameObject discardpileObject;
    //�����X�^�[
    public GameObject monsterObject;

    private Sprite[] smallCardImageList = new Sprite[gameCore._gameBoard.SMALLCARD_COUNT];
    private Sprite[] largeCardImageList = new Sprite[gameCore._gameBoard.LARGECARD_COUNT];

    //private methods
    private void Reset(GameObject content) { 
        foreach(CardView a in content.GetComponentsInChildren<CardView>()) {
            a.DestroySelf();
        }
    }
    private void Apply(List<int> data,GameObject parent,Area area,int holderNum,bool isLarge) {
        int i = 0;
        foreach (int id in data) {
            GameObject card = (GameObject)Resources.Load("Card");
            GameObject a = Instantiate(card, parent.transform);
            CardView view = a.AddComponent<CardView>();
            if(id != -1) {
                if(isLarge)view.ApplyData(id, largeCardImageList[id], i, area, gameCore, holderNum, isLarge);
                else view.ApplyData(id, smallCardImageList[id], i, area, gameCore, holderNum, isLarge);
            }
            else {
                view.ApplyData(id, null, i, area, gameCore, holderNum, isLarge);
            }
            i++;
        }
    }
    private void ApplyHero(List<int> heroData,List<int> armedCardData,GameObject parent,Area area,int holderNum,bool isLarge) {
        for(int i = 0; i < heroData.Count;i++){
            GameObject card = (GameObject)Resources.Load("Card");
            GameObject a = Instantiate(card, parent.transform);
            CardView view = a.AddComponent<CardView>();
            if(hero.armedCardID != -1) view.ApplyHeroData(heroData[i], smallCardImageList[heroData[i]], armedCardData[i], smallCardImageList[armedCardData[i]],i,area,gameCore,holderNum,isLarge);
            else view.ApplyHeroData(heroData[i], smallCardImageList[hero.cardID], armedCardData[i], null, i, area, gameCore, holderNum, isLarge);
        }
    }

    //public methods
    public void ApplyHand(List<int> data){
        //hand��������
        Reset(handObject);
        Apply(data,handObject,Area.playerHand,gameCore.playerID,false);

    } //��D�Ƀf�[�^��K�p
    public void ApplyHero(List<int> heroData, List<int> armedCardData,int playerNum){
        Reset(heroObject[playerNum]);
        ApplyHero(heroData,armedCardData,heroObject[playerNum],Area.playerHero,playerNum,false);
    } //�q�[���[���X�g�Ƀf�[�^��K�p
    public void ApplySlayedMonster(List<int> data,int playerNum){
        Reset(slayedmonsterObject[playerNum]);
        Apply(data, sprite, slayedmonsterObject[playerNum],Area.slayedMonster,playerNum,true);
    } //�|���������X�^�[���X�g�Ƀf�[�^��K�p
    public void ApplyDiscardPile(List<int> data){
        Reset(discardpileObject);
        Apply(data, sprite,discardpileObject,Area.discardPile,0, false);
    }
    public void ApplyMonster(List<int> data){
        Reset(monsterObject);
        Apply(data, monsterObject,Area.monsterList,0,true);
    }

    private void Start() {
        smallCardImageList = Resources.LoadAll("deck_cards",typeof(Sprite)).Cast<Sprite>().ToArray();
        largeCardImageList = Resources.LoadAll("monster_and_leader_cards",typeof(Sprite)).Cast<Sprite>().ToArray();
    }
}

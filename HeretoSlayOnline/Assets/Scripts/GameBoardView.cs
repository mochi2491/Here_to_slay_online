using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoardView : MonoBehaviour
{
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
    private void Apply(List<int> data, Sprite[] sprites) {
        foreach (int id in data) {
            GameObject card = (GameObject)Resources.Load("Card");
            CardView view = card.AddComponent<CardView>();
            view.ApplyData(id, sprites[id]);
            Instantiate(card, handObject.transform);
        }
    }

    //public methods
    public void ApplyHand(List<int> data, Sprite[] sprites){
        //hand��������
        Reset(handObject);
        Apply(data, sprites);
    }
    public void ApplyHero(List<int> data, Sprite[] sprites,int playerNum){
        Reset(heroObject[playerNum]);
        Apply(data, sprites);
    }

    public void ApplySlayedMonster(List<int> data, Sprite sprite,int playerNum){
        foreach(int id in data){
            GameObject card = (GameObject)Resources.Load("Card");
            CardView  view = card.AddComponent<CardView>();
            view.ApplyData(id,sprite);
            Instantiate(card, slayedmonsterObject[0].transform);
        }
    }

    
    public void ApplyDiscardPile(List<int> data, Sprite sprite){
        foreach(int id in data){
            GameObject card = (GameObject)Resources.Load("Card");
            CardView  view = card.AddComponent<CardView>();
            view.ApplyData(id,sprite);
            Instantiate(card,discardpileObject.transform);
        }
    }

   
    public void ApplyMonster(List<int> data, Sprite sprite){
        foreach(int id in data){
            GameObject card = (GameObject)Resources.Load("Card");
            CardView  view = card.AddComponent<CardView>();
            view.ApplyData(id,sprite);
            Instantiate(card,monsterObject.transform);
        }
    }

    private void Start() {
        
    }
}

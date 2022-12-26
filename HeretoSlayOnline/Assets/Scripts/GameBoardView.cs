using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoardView : MonoBehaviour
{
    //手札表示
    public GameObject handObject;

    public void ApplyHand(List<int> data, Sprite sprite){
        foreach(int id in data){
            GameObject card = (GameObject)Resources.Load("Card");
            CardView  view = card.AddComponent<CardView>();
            view.ApplyData(id,sprite);
            Instantiate(card,handObject.transform);
        }
    }
    //各プレイヤーのヒーロー
    [SerializeField]public List<GameObject> heroObject = new List<GameObject>();
    public void ApplyHero(List<int> data, Sprite sprite){
        foreach(int id in data){
            GameObject card = (GameObject)Resources.Load("Card");
            CardView  view = card.AddComponent<CardView>();
            view.ApplyData(id,sprite);
            Instantiate(card,heroObject.transform);
        }
    }

    //倒したモンスター
    public GameObject slayedmonsterObject;
    public void ApplySlayedMonster(List<int> data, Sprite sprite){
        foreach(int id in data){
            GameObject card = (GameObject)Resources.Load("Card");
            CardView  view = card.AddComponent<CardView>();
            view.ApplyData(id,sprite);
            Instantiate(card,slayedmonsterObject.transform);
        }
    }

    //捨て札
    public GameObject discardpileObject;
    public void ApplyDiscardPile(List<int> data, Sprite sprite){
        foreach(int id in data){
            GameObject card = (GameObject)Resources.Load("Card");
            CardView  view = card.AddComponent<CardView>();
            view.ApplyData(id,sprite);
            Instantiate(card,discardpileObject.transform);
        }
    }

    //モンスター
    public GameObject monsterObject;
    public void ApplyMonster(List<int> data, Sprite sprite){
        foreach(int id in data){
            GameObject card = (GameObject)Resources.Load("Card");
            CardView  view = card.AddComponent<CardView>();
            view.ApplyData(id,sprite);
            Instantiate(card,monsterObject.transform);
        }
    }

}

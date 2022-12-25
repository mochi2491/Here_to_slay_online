using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoardView : MonoBehaviour
{
    //手札表示
    public GameObject handObject;

    public void ApplyHand(List<int> data){
        foreach(int id in data){
            GameObject card = Resources.Load("Card");
            Instantiate(card.AddComponent(new CardView(id,"")),handObject.transform);
        }
    }

    //各プレイヤーのヒーロー

    //倒したモンスター

    //捨て札

    //モンスター

}

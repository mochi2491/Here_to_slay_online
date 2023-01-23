using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class CardPresenter : MonoBehaviour
{
    //model
    public GameCore gameCore;

    //view
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

    public List<CardView> Hand;
    public List<CardView> Discard;
    public List<List<CardView>> Hero;
    public List<CardView> monsterList;
    public List<CardView> slayerMonser;

    private void Start() {
        gameCore._gameBoard.Subscribe(
            board => {


            }
            );
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class CardPresenter : MonoBehaviour
{
    //model
    public GameCore gameCore;

    //view
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

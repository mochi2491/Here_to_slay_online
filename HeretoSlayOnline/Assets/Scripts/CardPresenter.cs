using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardPresenter : MonoBehaviour
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

    public List<CardView> Hand;
    public List<CardView> Discard;
    public List<List<CardView>> Hero;
    public List<CardView> monsterList;
    public List<CardView> slayerMonser;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoardView : MonoBehaviour
{
    //��D�\��
    public GameObject handObject;

    public void ApplyHand(List<int> data){
        foreach(int id in data){
            GameObject card = Resources.Load("Card");
            Instantiate(card.AddComponent(new CardView(id,"")),handObject.transform);
        }
    }

    //�e�v���C���[�̃q�[���[

    //�|���������X�^�[

    //�̂ĎD

    //�����X�^�[

}

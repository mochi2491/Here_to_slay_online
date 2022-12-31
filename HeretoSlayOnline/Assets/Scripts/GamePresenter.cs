using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class GamePresenter : MonoBehaviour
{
    //model
    [SerializeField] private GameCore gameCore;

    //view
    [SerializeField] private EntranceView entranceView;
    void Start()
    {
        //view -> model �C�x���g����
        //entrance
        IEntrance _entrance = gameCore._entrance;

        //setUserName InputField�ɓ��͂��ꂽ�������K�p
        entranceView.userNameText.onValueChanged.AsObservable().Subscribe(
            x => {
                _entrance.SetUserName(x);
            }    
        ).AddTo(this);

        //sendUserName Button�������ꂽ��UserName�𑗐M
        entranceView.sendButton.onClick.AsObservable().Subscribe(
            _ => {
                _entrance.SendUserName();
            }
        ).AddTo(this);

        //isReady IsReady�̏�Ԃ�K�p
        entranceView.isReadyToggle.onValueChanged.AsObservable().Subscribe(
            x => {
                _entrance.ApplyIsReady(x);
            }
        ).AddTo(this);

        //model -> view ���A�N�e�B�u
        
        //isReadyToggle�̑���̔\�ۂ̊Ǘ�
        gameCore._state.Subscribe(
            state => {
                if(state == GameState.wait) entranceView.isReadyToggle.interactable = true;
                else entranceView.isReadyToggle.interactable = false;
            }
        );
        
    }
    void Update()
    {
        
    }
}

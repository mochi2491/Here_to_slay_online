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
        //view -> model イベント発火
        //entrance
        IEntrance _entrance = gameCore._entrance;

        //setUserName InputFieldに入力された文字列を適用
        entranceView.userNameText.onValueChanged.AsObservable().Subscribe(
            x => {
                _entrance.SetUserName(x);
            }    
        ).AddTo(this);

        //sendUserName Buttonが押されたらUserNameを送信
        entranceView.sendButton.onClick.AsObservable().Subscribe(
            _ => {
                _entrance.SendUserName();
            }
        ).AddTo(this);

        //isReady IsReadyの状態を適用
        entranceView.isReadyToggle.onValueChanged.AsObservable().Subscribe(
            x => {
                _entrance.ApplyIsReady(x);
            }
        ).AddTo(this);

        //model -> view リアクティブ
        
        //isReadyToggleの操作の能否の管理
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

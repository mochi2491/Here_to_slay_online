using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class GamePresenter : MonoBehaviour
{
    //model
    [SerializeField] private GameCore gameCore;

    //view
    [SerializeField] private EntranceView entranceView;
    [SerializeField] private FieldTabsView fieldTabsView;
    [SerializeField] private ChatView chatView;
    void Start()
    {
        int i = 0;
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

        //FieldTabs
        IFieldTabs _fieldTabs = gameCore._fieldTabs;
        #region
        fieldTabsView.fieldButtons[0].onClick.AsObservable().Subscribe(
            _ => {
                _fieldTabs.SetVisibleTabNum(0);
            }
        );
        fieldTabsView.fieldButtons[1].onClick.AsObservable().Subscribe(
            _ => {
                _fieldTabs.SetVisibleTabNum(1);
            }
        );
        fieldTabsView.fieldButtons[2].onClick.AsObservable().Subscribe(
            _ => {
                _fieldTabs.SetVisibleTabNum(2);
            }
        );
        fieldTabsView.fieldButtons[3].onClick.AsObservable().Subscribe(
            _ => {
                _fieldTabs.SetVisibleTabNum(3);
            }
        );
        fieldTabsView.fieldButtons[4].onClick.AsObservable().Subscribe(
            _ => {
                _fieldTabs.SetVisibleTabNum(4);
            }
        ); 
        fieldTabsView.fieldButtons[5].onClick.AsObservable().Subscribe(
            _ => {
                _fieldTabs.SetVisibleTabNum(5);
            }
        ); 
        fieldTabsView.fieldButtons[6].onClick.AsObservable().Subscribe(
            _ => {
                _fieldTabs.SetVisibleTabNum(6);
            }
        ); 
        fieldTabsView.fieldButtons[7].onClick.AsObservable().Subscribe(
            _ => {
                _fieldTabs.SetVisibleTabNum(7);
            }
        );
        #endregion

        //chat area
        //send message
        chatView.sendButton.onClick.AsObservable().Subscribe(
            _ => {
                gameCore.ControlLog(chatView.input.text);
                chatView.chatLog.text = ""; //inputを消去する
            }
            
        );
        //dice roll
        chatView.diceButton.onClick.AsObservable().Subscribe(
            _ => {
                gameCore.ControlLog("rolled" + Random.Range(1, 6) + "," + Random.Range(1, 6));
            }
        );

        //model -> view リアクティブ

        //entrance
        //isReadyToggleの操作の能否の管理
        gameCore._state.Subscribe(
            state => {
                if(state == GameState.wait) entranceView.isReadyToggle.interactable = true;
                else entranceView.isReadyToggle.interactable = false;
            }
        );

        //fieldTabs
        //tabの切り替え
        gameCore.fieldTabs._visibleTabNum.Subscribe(
            num => {
                i = 0;
                foreach(GameObject tab in fieldTabsView.fieldTabs) {
                    if (i == num) tab.SetActive(true);
                    else tab.SetActive(false);
                    i++;
                }
            }
        );

        //chat area
        //chatの更新
        gameCore.gameBoard.chatArea._chatLog.Subscribe(
            log => {
                chatView.chatLog.text = log;
            }
        );
        
    }
    void Update()
    {
        
    }
}

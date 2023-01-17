using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UniRx;

public class GamePresenter : MonoBehaviour
{
    //model
    [SerializeField] private GameCore gameCore;

    //view
    [SerializeField] private EntranceView entranceView;
    [SerializeField] private FieldTabsView fieldTabsView;
    [SerializeField] private ChatView chatView;
    [SerializeField] private DescriptionView descriptionView;
    [SerializeField] private CommandPanelView commandPanelView;
    void Start() {
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
        //tabの切り替え
        IFieldTabs _fieldTabs = gameCore._fieldTabs;
        #region
        fieldTabsView.fieldButtons[0].onClick.AsObservable().Subscribe(
            _ => {
                _fieldTabs.SetVisibleTabNum(0);
            }
        ).AddTo(this);
        fieldTabsView.fieldButtons[1].onClick.AsObservable().Subscribe(
            _ => {
                _fieldTabs.SetVisibleTabNum(1);
            }
        ).AddTo(this);
        fieldTabsView.fieldButtons[2].onClick.AsObservable().Subscribe(
            _ => {
                _fieldTabs.SetVisibleTabNum(2);
            }
        ).AddTo(this);
        fieldTabsView.fieldButtons[3].onClick.AsObservable().Subscribe(
            _ => {
                _fieldTabs.SetVisibleTabNum(3);
            }
        ).AddTo(this);
        fieldTabsView.fieldButtons[4].onClick.AsObservable().Subscribe(
            _ => {
                _fieldTabs.SetVisibleTabNum(4);
            }
        ).AddTo(this);
        fieldTabsView.fieldButtons[5].onClick.AsObservable().Subscribe(
            _ => {
                _fieldTabs.SetVisibleTabNum(5);
            }
        ).AddTo(this);
        fieldTabsView.fieldButtons[6].onClick.AsObservable().Subscribe(
            _ => {
                _fieldTabs.SetVisibleTabNum(6);
            }
        ).AddTo(this);
        fieldTabsView.fieldButtons[7].onClick.AsObservable().Subscribe(
            _ => {
                _fieldTabs.SetVisibleTabNum(7);
            }
        ).AddTo(this);
        #endregion

        //leaderの切り替え
        fieldTabsView.leaderSelector[gameCore.playerID].onValueChanged.AsObservable().Subscribe(
            value => {
                gameCore.ControlLeaderNum(value);
            }
        ).AddTo(this);
        //leaderSkillButton
        #region
        fieldTabsView.LeaderSkillButton[0].onClick.AsObservable().Subscribe(
            _ => {
                gameCore.gameBoard.Value.AddLog("player1 use leader skill.");
            }
        );
        fieldTabsView.LeaderSkillButton[1].onClick.AsObservable().Subscribe(
            _ => {
                gameCore.gameBoard.Value.AddLog("player2 use leader skill.");
            }
        );
        fieldTabsView.LeaderSkillButton[2].onClick.AsObservable().Subscribe(
            _ => {
                gameCore.gameBoard.Value.AddLog("player3 use leader skill.");
            }
        );
        fieldTabsView.LeaderSkillButton[3].onClick.AsObservable().Subscribe(
            _ => {
                gameCore.gameBoard.Value.AddLog("player4 use leader skill.");
            }
        );
        fieldTabsView.LeaderSkillButton[4].onClick.AsObservable().Subscribe(
            _ => {
                gameCore.gameBoard.Value.AddLog("player5 use leader skill.");
            }
        );
        fieldTabsView.LeaderSkillButton[5].onClick.AsObservable().Subscribe(
            _ => {
                gameCore.gameBoard.Value.AddLog("player6 use leader skill.");
            }
        );
        #endregion
        //leaderDescription
        #region
        fieldTabsView.leaderDescriptionTrigger[0].OnPointerDownAsObservable()
            .Subscribe(
            _ => {
                gameCore.leftClickIsLarge = true;
                gameCore.SetClickedID(gameCore.gameBoard.Value.playerAreaList[0]._leaderCardID.Value);
            }
        ).AddTo(this);

        #endregion
        //chat area
        //send message
        chatView.sendButton.onClick.AsObservable().Subscribe(
            _ => {
                gameCore.ControlLog(chatView.input.text);
                chatView.input.text = ""; //inputを消去する
            }

        ).AddTo(this);
        //dice roll
        chatView.diceButton.onClick.AsObservable().Subscribe(
            _ => {
                gameCore.ControlLog("rolled" + Random.Range(1, 6) + "," + Random.Range(1, 6));
            }
        ).AddTo(this);


        //model -> view リアクティブ


        //entrance
        //isReadyToggleの操作の能否の管理
        gameCore._state.Subscribe(
            state => {
                if (state == GameState.wait) entranceView.isReadyToggle.interactable = true;
                else entranceView.isReadyToggle.interactable = false;
            }
        ).AddTo(this);


        //fieldTabs
        //tabの切り替え
        gameCore.fieldTabs._visibleTabNum.Subscribe(
            num => {
                i = 0;
                foreach (GameObject tab in fieldTabsView.fieldTabs) {
                    if (i == num) tab.SetActive(true);
                    else tab.SetActive(false);
                    i++;
                }
            }
        ).AddTo(this);

        //leaderSelectorの制御
        gameCore.gameBoard.Value._playerID.Subscribe(
            playerID => {
                i = 0;
                foreach (TMP_Dropdown selector in fieldTabsView.leaderSelector) {
                    if (i == playerID) selector.interactable = true;
                    else selector.interactable = false;
                    i++;
                }
            }
        ).AddTo(this);
        //leaderSkillButtonの制御
        gameCore.gameBoard.Value._playerID.Subscribe(
            playerID => {
                i = 0;
                foreach (Button button in fieldTabsView.LeaderSkillButton) {
                    if (i == playerID) button.interactable = true;
                    else button.interactable = false;
                    i++;
                }

                fieldTabsView.playerID.text = "ID" + playerID;
            }
        ).AddTo(this);
        //leaderCardの見た目
        #region
        gameCore.gameBoard.Subscribe(
            board => {
                fieldTabsView.leaderImage[0].sprite = fieldTabsView.leaderSprite[board.playerAreaList[0]._leaderCardID.Value];
                fieldTabsView.leaderImage[1].sprite = fieldTabsView.leaderSprite[board.playerAreaList[1]._leaderCardID.Value];
                fieldTabsView.leaderImage[2].sprite = fieldTabsView.leaderSprite[board.playerAreaList[2]._leaderCardID.Value];
                fieldTabsView.leaderImage[3].sprite = fieldTabsView.leaderSprite[board.playerAreaList[3]._leaderCardID.Value];
                fieldTabsView.leaderImage[4].sprite = fieldTabsView.leaderSprite[board.playerAreaList[4]._leaderCardID.Value];
                fieldTabsView.leaderImage[5].sprite = fieldTabsView.leaderSprite[board.playerAreaList[5]._leaderCardID.Value];
            }
            ).AddTo(this);
/*
        gameCore.gameBoard.Value.playerAreaList[0]._leaderCardID.Subscribe(
            num => {

                fieldTabsView.leaderImage[0].sprite = fieldTabsView.leaderSprite[num];
            }
        );
        gameCore.gameBoard.Value.playerAreaList[1]._leaderCardID.Subscribe(
            num => {
                fieldTabsView.leaderImage[1].sprite = fieldTabsView.leaderSprite[num];
            }
        );
        gameCore.gameBoard.Value.playerAreaList[2]._leaderCardID.Subscribe(
            num => {
                fieldTabsView.leaderImage[2].sprite = fieldTabsView.leaderSprite[num];
            }
        );
        gameCore.gameBoard.Value.playerAreaList[3]._leaderCardID.Subscribe(
            num => {
                fieldTabsView.leaderImage[3].sprite = fieldTabsView.leaderSprite[num];
            }
        );
        gameCore.gameBoard.Value.playerAreaList[4]._leaderCardID.Subscribe(
            num => {
                fieldTabsView.leaderImage[4].sprite = fieldTabsView.leaderSprite[num];
            }
        );
        gameCore.gameBoard.Value.playerAreaList[5]._leaderCardID.Subscribe(
            num => {
                fieldTabsView.leaderImage[5].sprite = fieldTabsView.leaderSprite[num];
            }
        );
*/
        #endregion


        //chat area
        //chatの更新
        gameCore.gameBoard.Value.chatArea._chatLog.Subscribe(
            log => {
                chatView.chatLog.text = log;
                chatView.scrollRect.verticalNormalizedPosition = 0f;
            }
        ).AddTo(this);

        //description area
        gameCore._leftClickedID.Subscribe(
            x => {
                if (gameCore.leftClickIsLarge) {
                    descriptionView.text.text = gameCore.cardDataManager.GetLargeScript(x);
                }
                else {
                    descriptionView.text.text = gameCore.cardDataManager.GetSmallScript(x);
                }
            }
            ).AddTo(this);
        //commandPanel
        
        gameCore._commandPanelModel.Subscribe(
            x => {
                commandPanelView.ApplyModel(x);
            }
            );
    }
    void Update()
    {
        
    }
}

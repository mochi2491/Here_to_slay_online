using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UniRx;
using System;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using Cysharp.Threading.Tasks.Triggers;

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
    [SerializeField] private MenuPanelView menuPanelView;
    [SerializeField] private PeepPanelView peepPanelView;

    IDisposable aaa;
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
        
        for (i = 0; i < 6; i++) {
            fieldTabsView.leaderSelector[i].onValueChanged.AsObservable().Subscribe(
            value => {
                gameCore.ControlLeaderNum(value);
            }
        ).AddTo(this);
        }


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
        
        //chat area
        //send message
        chatView.sendButton.onClick.AsObservable().Subscribe(
            _ => {
                gameCore.ControlLog(chatView.input.text);
                chatView.input.text = ""; 
            }

        ).AddTo(this);
        //dice roll
        chatView.diceButton.onClick.AsObservable().Subscribe(
            _ => {
                gameCore.ControlLog("rolled" + UnityEngine.Random.Range(1, 6) + "," + UnityEngine.Random.Range(1, 6));
            }
        ).AddTo(this);

        //menuPanel
        menuPanelView.QuitButton.OnClickAsObservable().Subscribe(
            _ => {
                gameCore.QuitGame();
            }
        ).AddTo(this);
        menuPanelView.ResetButton.OnClickAsObservable().Subscribe(
            _ => {
                gameCore.ResetBoard();
            }
        );
        menuPanelView.OpenButton.OnClickAsObservable().Subscribe(
            _ => {
                gameCore.menuPanelModel.Value = gameCore.menuPanelModel.Value.Open();
            }
        );
        menuPanelView.CloseButton.OnClickAsObservable().Subscribe(
            _ => {
                gameCore.menuPanelModel.Value = gameCore.menuPanelModel.Value.Close();
            }
        );
        commandPanelView.closerTrigger.OnPointerDownAsObservable().Subscribe(
            _ => {
                gameCore.commandPanelModel.Value = gameCore.commandPanelModel.Value.CloseAllPanel();
            }
        );
        commandPanelView.smallButtons[0].OnClickAsObservable().Subscribe(
            _ => {
                gameCore.gameBoard.Value.AddLog(gameCore.playerID+"use hero skill.");
                //ここにスキルの発動処理を書く
            }
            );

        //peepPanel
        //quitボタンが押されたらパネルを閉じる
        peepPanelView.quitButton.OnClickAsObservable().Subscribe(
            _ => {
                gameCore.peepPanelModel.Value =  gameCore.peepPanelModel.Value.SetActive(false);
            }
            );
        for(i = 0; i < 6; i++) {
            int count = i;
            fieldTabsView.peepButton[i].OnClickAsObservable().Subscribe(
                _ => {
                    //覗くパネルをアクティブにする
                    gameCore.peepPanelModel.Value = gameCore.peepPanelModel.Value.SetActive(true);

                    //覗く手札を表示する
                    gameCore.peepPanelModel.Value = gameCore.peepPanelModel.Value.SetHandList(gameCore.gameBoard.Value.playerAreaList[count].PlayerHandList);
                }
                );
        }
        //pull
        for (i = 0; i < 6; i++) {
            int count = i;
            fieldTabsView.pullSelector[count].onValueChanged.AsObservable().Subscribe(
                value => {
                    gameCore.gameBoard.Value.playerAreaList[count].pullNum = value;
                }
                );
        }
        for (i = 0; i < 6; i++) {
            int count = i;
            fieldTabsView.pullButton[count].OnClickAsObservable().Subscribe(
                _ => {
                    GameBoardAddress from = new GameBoardAddress();
                    from.playerID = count;
                    from.area = Area.playerHand;
                    from.order = gameCore.gameBoard.Value.playerAreaList[count].pullNum;
                    GameBoardAddress to = new GameBoardAddress();
                    to.playerID = gameCore.playerID;
                    to.area = Area.playerHand;
                    gameCore.gameBoard.Value = gameCore.gameBoard.Value.ControlBoard(from,to);
                }
                );
        }

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
        //IDとUserNameの表示
        gameCore.gameBoard.Subscribe(
            board => {
                i = 0;
                foreach (Button button in fieldTabsView.LeaderSkillButton) {
                    if (i == board.PlayerID) button.interactable = true;
                    else button.interactable = false;
                    i++;
                }

                fieldTabsView.playerID.text = "ID:" + board.PlayerID + ",Name:" + gameCore.entrance.UserName;
            }
        ).AddTo(this);
        //leaderCardの見た目
        //handの枚数表示
        gameCore._gameBoard.Subscribe(
            board => {
                fieldTabsView.leaderImage[0].sprite = fieldTabsView.leaderSprite[board.playerAreaList[0]._leaderCardID.Value];
                fieldTabsView.leaderImage[1].sprite = fieldTabsView.leaderSprite[board.playerAreaList[1]._leaderCardID.Value];
                fieldTabsView.leaderImage[2].sprite = fieldTabsView.leaderSprite[board.playerAreaList[2]._leaderCardID.Value];
                fieldTabsView.leaderImage[3].sprite = fieldTabsView.leaderSprite[board.playerAreaList[3]._leaderCardID.Value];
                fieldTabsView.leaderImage[4].sprite = fieldTabsView.leaderSprite[board.playerAreaList[4]._leaderCardID.Value];
                fieldTabsView.leaderImage[5].sprite = fieldTabsView.leaderSprite[board.playerAreaList[5]._leaderCardID.Value];

                fieldTabsView.handCount[0].text = board.playerAreaList[0].PlayerHandList.Count.ToString();
                fieldTabsView.handCount[1].text = board.playerAreaList[1].PlayerHandList.Count.ToString();
                fieldTabsView.handCount[2].text = board.playerAreaList[2].PlayerHandList.Count.ToString();
                fieldTabsView.handCount[3].text = board.playerAreaList[3].PlayerHandList.Count.ToString();
                fieldTabsView.handCount[4].text = board.playerAreaList[4].PlayerHandList.Count.ToString();
                fieldTabsView.handCount[5].text = board.playerAreaList[5].PlayerHandList.Count.ToString();
            }
            ).AddTo(this);


        //pull
        //各プレイヤーエリアのpullSelectorの選択肢を設定する。
        gameCore._gameBoard.Subscribe(
            board => {
                int j = 0;
                foreach(PlayerArea pa in board.playerAreaList) {
                    fieldTabsView.pullSelector[j].ClearOptions();
                    for(int i = 0; i < pa.PlayerHandList.Count;i++) {
                        fieldTabsView.pullSelector[j].options.Add(new TMP_Dropdown.OptionData(i.ToString()));
                    }
                    j++;
                }
            }
            );

        //chat area
        //chatの更新
        gameCore.gameBoard.Value.chatArea._chatLog.Subscribe(
            log => {
                chatView.chatLog.text = log;
                chatView.scrollRect.verticalNormalizedPosition = 0f;
            }
        ).AddTo(this);

        //description area
        //カードをポイントした時のカード表示
        gameCore._cardIndicatorModel.Subscribe(
            x => {
                if (x.IsLarge) descriptionView.text.text = gameCore.cardDataManager.GetLargeScript(x.GetID);
                else descriptionView.text.text = gameCore.cardDataManager.GetSmallScript(x.GetID);
            }
            );

        //commandPanel
        gameCore._commandPanelModel.Subscribe(
            x => {
                commandPanelView.ApplyModel(x);
            }
            );
        gameCore.gameBoard.Subscribe(
            board => {
                int i = 0;
                gameCore.gameBoard.Value.playerAreaList[gameCore.playerID].UserName =  gameCore.entrance.UserName;
                if (commandPanelView.smallPlayerButtonTexts.Count > 0 && commandPanelView.largePlayerButtonTexts.Count > 0) {
                    foreach (string name in board.GetUserNameList()) {
                        commandPanelView.smallPlayerButtonTexts[i].text = name;
                        commandPanelView.largePlayerButtonTexts[i].text = name;
                        i++;
                    }
                }
            }
            );

        //menuPanel
        gameCore.menuPanelModel.Subscribe(
            x => {
                menuPanelView.MenuPanel.SetActive(x.isActive);
            }
            );
        gameCore.peepPanelModel.Subscribe(
            x => {
                peepPanelView.peepPanel.SetActive(x.IsActive);
                peepPanelView.ApplyView(x.HandList);
            }
            );

    }
    void Update()
    { 
    }
}

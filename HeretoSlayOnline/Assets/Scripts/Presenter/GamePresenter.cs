using JetBrains.Annotations;
using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] private ChallengePanelView challengePanelView;

    private void Start()
    {
        BindPeepPanel();
        BindEntrace();
        BindFieldTab();
        BindChatArea();
        BindMenuPanel();
        BindCommandPanel();
        BindChallenge();
    }

    private void BindPeepPanel()
    {
        gameCore.peepPanelModel._handList.Subscribe(
            x =>
            {
                peepPanelView.ApplyView(x);
            });
        //peepPanel
        //quitボタンが押されたらパネルを閉じる
        peepPanelView.quitButton.OnClickAsObservable().Subscribe(
            _ =>
            {
                peepPanelView.peepPanel.SetActive(false);
            }
            );
        for (int i = 0; i < 6; i++)
        {
            int count = i;
            fieldTabsView.peepButton[i].OnClickAsObservable().Subscribe(
                _ =>
                {
                    //覗くパネルをアクティブにする
                    peepPanelView.peepPanel.SetActive(true);
                    //覗く手札を表示する
                    gameCore.peepPanelModel.SetHandList(gameCore.gameBoard.Value.playerAreaList[count].PlayerHandList);
                }
                );
        }
    }
    private void BindEntrace()
    {
        IEntrance _entrance = gameCore._entrance;

        //view -> model
        //setUserName InputFieldに入力された文字列を適用
        entranceView.userNameText.onValueChanged.AsObservable().Subscribe(
            x =>
            {
                _entrance.SetUserName(x);
            }
        ).AddTo(this);

        //sendUserName Buttonが押されたらUserNameを送信
        entranceView.sendButton.onClick.AsObservable().Subscribe(
            _ =>
            {
                _entrance.SendUserName();
                if (gameCore._state.Value == GameState.entrance)
                {
                    gameCore.connector.SendText("0:::" + _entrance.GetUserName());
                    gameCore.ChangeState(GameState.wait);
                }
            }
        ).AddTo(this);

        //isReady IsReadyの状態を適用
        entranceView.isReadyToggle.onValueChanged.AsObservable().Subscribe(
            x =>
            {
                _entrance.ApplyIsReady(x);
                if (gameCore._state.Value != GameState.wait) return;
                if (x) gameCore.connector.SendText("1:::1");
                else gameCore.connector.SendText("1:::0");
            }
        ).AddTo(this);

        //quitButton
        entranceView.quitButton.OnClickAsObservable().Subscribe(
            _ =>
            {
                _entrance.QuitGame();
            }
            ).AddTo(this);
        //entrance

        //model -> view
        //isReadyToggleの操作の能否の管理
        gameCore._state.Subscribe(
            state =>
            {
                if (state == GameState.wait) entranceView.isReadyToggle.interactable = true;
                else entranceView.isReadyToggle.interactable = false;
            }
        ).AddTo(this);
    }
    private void BindFieldTab()
    {
        //FieldTabs
        //tabの切り替え
        IFieldTabs _fieldTabs = gameCore._fieldTabs;

        //view -> model

        //FieldTabの切り替えボタン
        for (int i = 0; i < 8; i++)
        {
            int count = i;
            fieldTabsView.fieldButtons[count].onClick.AsObservable().Subscribe(
                _ =>
                {
                    _fieldTabs.SetVisibleTabNum(count);
                }
            ).AddTo(this);
        }

        //leaderの切り替え
        for (int i = 0; i < 6; i++)
        {
            fieldTabsView.leaderSelector[i].onValueChanged.AsObservable().Subscribe(
            value =>
            {
                gameCore.ControlLeaderNum(value);
            }
            ).AddTo(this);
        }

        //leaderSkillButton
        for (int i = 0; i < 6; i++)
        {
            int count = i;
            fieldTabsView.LeaderSkillButton[count].onClick.AsObservable().Subscribe(
                _ =>
                {
                    gameCore.gameBoard.Value.AddLog("player " + (count + 1) + " use leader skill.");
                }
            ).AddTo(this);
        }

        //dropdownからpullする手札の番号を指定
        for (int i = 0; i < 6; i++)
        {
            int count = i;
            fieldTabsView.pullSelector[count].onValueChanged.AsObservable().Subscribe(
                value =>
                {
                    gameCore.gameBoard.Value.playerAreaList[count].pullNum = value;
                }
            );
        }

        //実際にpullでカードを移動させる
        for (int i = 0; i < 6; i++)
        {
            int count = i;
            fieldTabsView.pullButton[count].OnClickAsObservable().Subscribe(
                _ =>
                {
                    GameBoardAddress from = new GameBoardAddress();
                    from.playerID = count;
                    from.area = Area.playerHand;
                    from.order = gameCore.gameBoard.Value.playerAreaList[count].pullNum;
                    GameBoardAddress to = new GameBoardAddress();
                    to.playerID = gameCore.playerID;
                    to.area = Area.playerHand;
                    gameCore.gameBoard.Value = gameCore.gameBoard.Value.ControlBoard(from, to);
                }
                );
        }


        //model -> view

        //tabの切り替え
        gameCore.fieldTabs._visibleTabNum.Subscribe(
            num =>
            {
                int i = 0;
                foreach (GameObject tab in fieldTabsView.fieldTabs)
                {
                    if (i == num) tab.SetActive(true);
                    else tab.SetActive(false);
                    i++;
                }
            }
        ).AddTo(this);

        //leaderSelectorの制御
        gameCore.gameBoard.Value._playerID.Subscribe(
            playerID =>
            {
                int i = 0;
                foreach (TMP_Dropdown selector in fieldTabsView.leaderSelector)
                {
                    if (i == playerID) selector.interactable = true;
                    else selector.interactable = false;
                    i++;
                }
            }
        ).AddTo(this);

        //leaderSkillButtonの制御
        //IDとUserNameの表示
        gameCore.gameBoard.Subscribe(
            board =>
            {
                int i = 0;
                foreach (Button button in fieldTabsView.LeaderSkillButton)
                {
                    if (i == board.PlayerID) button.interactable = true;
                    else button.interactable = false;
                    i++;
                }

                fieldTabsView.playerID.text = "ID:" + board.PlayerID + ",Name:" + gameCore.entrance.UserName;
            }
        ).AddTo(this);
        //leaderCardの見た目
        //handの枚数表示
        for (int i = 0; i < 6; i++)
        {
            int count = i;
            gameCore._gameBoard.Subscribe(
                board =>
                {
                    fieldTabsView.leaderImage[count].sprite = fieldTabsView.leaderSprite[board.playerAreaList[count]._leaderCardID.Value];
                    fieldTabsView.handCount[count].text = board.playerAreaList[count].PlayerHandList.Count.ToString();
                }
            ).AddTo(this);
        }
        //pull
        //各プレイヤーエリアのpullSelectorの選択肢を設定する。
        gameCore._gameBoard.Subscribe(
            board =>
            {
                int j = 0;
                foreach (PlayerArea pa in board.playerAreaList)
                {
                    fieldTabsView.pullSelector[j].ClearOptions();
                    for (int i = 0; i < pa.PlayerHandList.Count; i++)
                    {
                        fieldTabsView.pullSelector[j].options.Add(new TMP_Dropdown.OptionData(i.ToString()));
                    }
                    j++;
                }
            }
            );

    }
    private void BindChatArea()
    {
        //view -> model
        //send message
        chatView.sendButton.onClick.AsObservable().Subscribe(
            _ =>
            {
                gameCore.ControlLog(chatView.input.text);
                chatView.input.text = "";
            }

        ).AddTo(this);
        //dice roll
        chatView.diceButton.onClick.AsObservable().Subscribe(
            _ =>
            {
                gameCore.ControlLog("rolled" + UnityEngine.Random.Range(1, 6) + "," + UnityEngine.Random.Range(1, 6));
            }
        ).AddTo(this);


        //model -> view
        gameCore.gameBoard.Value.chatArea._chatLog.Subscribe(
            log =>
            {
                chatView.chatLog.text = log;
                chatView.scrollRect.verticalNormalizedPosition = 0f;
            }
        ).AddTo(this);

        //description area
        //カードをポイントした時のカード表示
        gameCore._cardIndicatorModel.Subscribe(
            x =>
            {
                if (x.IsLarge) descriptionView.text.text = gameCore.cardDataManager.GetLargeScript(x.GetID);
                else descriptionView.text.text = gameCore.cardDataManager.GetSmallScript(x.GetID);
            }
            );
    }
    private void BindMenuPanel()
    {
        //view -> model
        //menuPanel
        menuPanelView.QuitButton.OnClickAsObservable().Subscribe(
            _ =>
            {
                gameCore.QuitGame();
            }
        ).AddTo(this);
        menuPanelView.ResetButton.OnClickAsObservable().Subscribe(
            _ =>
            {
                gameCore.ResetBoard();
            }
        );
        menuPanelView.OpenButton.OnClickAsObservable().Subscribe(
            _ =>
            {
                gameCore.menuPanelModel.Value = gameCore.menuPanelModel.Value.Open();
            }
        );
        menuPanelView.CloseButton.OnClickAsObservable().Subscribe(
            _ =>
            {
                gameCore.menuPanelModel.Value = gameCore.menuPanelModel.Value.Close();
            }
        );

        //model -> view
        //menuPanel
        gameCore.menuPanelModel.Subscribe(
            x =>
            {
                menuPanelView.MenuPanel.SetActive(x.isActive);
            }
            );
    }
    private void BindCommandPanel()
    {
        //view -> model
        commandPanelView.closerTrigger.OnPointerDownAsObservable().Subscribe(
                    _ =>
                    {
                        gameCore.commandPanelModel.Value = gameCore.commandPanelModel.Value.CloseAllPanel();
                    }
                );
        commandPanelView.smallButtons[0].OnClickAsObservable().Subscribe( //Use Skill
            _ =>
            {
                gameCore.gameBoard.Value.AddLog(gameCore.playerID + ":" + "" + "use hero skill.");
                //ここにスキルの発動処理を書く
            }
        );

        //orderNum設定
        commandPanelView.orderList.onValueChanged.AsObservable().Subscribe(
            num =>
            {
                gameCore.heroNum_ = num;
            }
        );

        //model -> view
        gameCore._commandPanelModel.Subscribe(
            x =>
            {
                commandPanelView.ApplyModel(x);
            }
            );
        gameCore.gameBoard.Subscribe(
            board =>
            {
                int i = 0;
                gameCore.gameBoard.Value.playerAreaList[gameCore.playerID].UserName = gameCore.entrance.UserName;
                if (commandPanelView.smallPlayerButtonTexts.Count > 0 && commandPanelView.largePlayerButtonTexts.Count > 0)
                {
                    foreach (string name in board.GetUserNameList())
                    {
                        commandPanelView.smallPlayerButtonTexts[i].text = name;
                        commandPanelView.largePlayerButtonTexts[i].text = name;
                        i++;
                    }
                }
            }
            );


    }
    private void BindChallenge()
    {
        float displayTime = 2f;

        gameCore.gameBoard.Subscribe(
            board =>
            {
                int lastIndex = board.GetdeckArea.discardPile.Count;
                int cardID = 0;
                if (lastIndex > 0)
                {
                    cardID = board.GetdeckArea.discardPile[lastIndex - 1].ID;
                }
                if (cardID == 73)
                {
                    challengePanelView.challengePanel.SetActive(true);
                    Observable.Timer(TimeSpan.FromSeconds(displayTime))
                    .Subscribe(_ =>
                    {
                        challengePanelView.challengePanel.SetActive(false);
                    }
                    );
                }
            }
        );
    }

}
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
    void Start()
    {
        int i = 0;
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

        //FieldTabs
        //tab�̐؂�ւ�
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

        //leader�̐؂�ւ�
        fieldTabsView.leaderSelector[gameCore.playerID].onValueChanged.AsObservable().Subscribe(
            value => {
                gameCore.ControlLeaderNum(value);
            }    
        );
        //leaderSkillButton
        #region
        fieldTabsView.LeaderSkillButton[0].onClick.AsObservable().Subscribe(
            _ => {
                gameCore.gameBoard.AddLog("player1 use leader skill.");
            }    
        );
        fieldTabsView.LeaderSkillButton[1].onClick.AsObservable().Subscribe(
            _ => {
                gameCore.gameBoard.AddLog("player2 use leader skill.");
            }
        );
        fieldTabsView.LeaderSkillButton[2].onClick.AsObservable().Subscribe(
            _ => {
                gameCore.gameBoard.AddLog("player3 use leader skill.");
            }
        );
        fieldTabsView.LeaderSkillButton[3].onClick.AsObservable().Subscribe(
            _ => {
                gameCore.gameBoard.AddLog("player4 use leader skill.");
            }
        );
        fieldTabsView.LeaderSkillButton[4].onClick.AsObservable().Subscribe(
            _ => {
                gameCore.gameBoard.AddLog("player5 use leader skill.");
            }
        );
        fieldTabsView.LeaderSkillButton[5].onClick.AsObservable().Subscribe(
            _ => {
                gameCore.gameBoard.AddLog("player6 use leader skill.");
            }
        );
        #endregion
        //leaderDescription
        #region
        fieldTabsView.leaderDescriptionTrigger[0].OnPointerDownAsObservable()
            .Subscribe(
            _ => {
                gameCore.leftClickIsLarge = true;
                gameCore.SetClickedID(gameCore.gameBoard.playerAreaList[0]._leaderCardID.Value);
            }
        );

        #endregion
        //chat area
        //send message
        chatView.sendButton.onClick.AsObservable().Subscribe(
            _ => {
                gameCore.ControlLog(chatView.input.text);
                chatView.input.text = ""; //input����������
            }
            
        );
        //dice roll
        chatView.diceButton.onClick.AsObservable().Subscribe(
            _ => {
                gameCore.ControlLog("rolled" + Random.Range(1, 6) + "," + Random.Range(1, 6));
            }
        );


        //model -> view ���A�N�e�B�u


        //entrance
        //isReadyToggle�̑���̔\�ۂ̊Ǘ�
        gameCore._state.Subscribe(
            state => {
                if(state == GameState.wait) entranceView.isReadyToggle.interactable = true;
                else entranceView.isReadyToggle.interactable = false;
            }
        );


        //fieldTabs
        //tab�̐؂�ւ�
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

        //leaderSelector�̐���
        gameCore.gameBoard._playerID.Subscribe(
            playerID => {
                i = 0;
                foreach(TMP_Dropdown selector in fieldTabsView.leaderSelector) {
                    if (i == playerID) selector.interactable = true;
                    else selector.interactable = false;
                    i++;
                }
            }
        );
        //leaderSkillButton�̐���
        gameCore.gameBoard._playerID.Subscribe(
            playerID => {
                i = 0;
                foreach(Button button in fieldTabsView.LeaderSkillButton) {
                    if (i == playerID) button.interactable = true;
                    else button.interactable = false;
                    i++;
                }

                fieldTabsView.playerID.text = "ID" + playerID;
            }
        );
        //leaderCard�̌�����
        #region

        gameCore.gameBoard.playerAreaList[0]._leaderCardID.Subscribe(
            num => {
                
                fieldTabsView.leaderImage[0].sprite = fieldTabsView.leaderSprite[num];
            }
        );
        gameCore.gameBoard.playerAreaList[1]._leaderCardID.Subscribe(
            num => {
                fieldTabsView.leaderImage[1].sprite = fieldTabsView.leaderSprite[num];
            }
        ); 
        gameCore.gameBoard.playerAreaList[2]._leaderCardID.Subscribe(
            num => {
                fieldTabsView.leaderImage[2].sprite = fieldTabsView.leaderSprite[num];
            }
        ); 
        gameCore.gameBoard.playerAreaList[3]._leaderCardID.Subscribe(
            num => {
                fieldTabsView.leaderImage[3].sprite = fieldTabsView.leaderSprite[num];
            }
        ); 
        gameCore.gameBoard.playerAreaList[4]._leaderCardID.Subscribe(
            num => {
                fieldTabsView.leaderImage[4].sprite = fieldTabsView.leaderSprite[num];
            }
        ); 
        gameCore.gameBoard.playerAreaList[5]._leaderCardID.Subscribe(
            num => {
                fieldTabsView.leaderImage[5].sprite = fieldTabsView.leaderSprite[num];
            }
        );

        #endregion


        //chat area
        //chat�̍X�V
        gameCore.gameBoard.chatArea._chatLog.Subscribe(
            log => {
                chatView.chatLog.text = log;
                chatView.scrollRect.verticalNormalizedPosition = 0f;
            }
        );

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
            );
        
    }
    void Update()
    {
        
    }
}

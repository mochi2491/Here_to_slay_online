using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
using UnityEngine.EventSystems;

public class CardPresenter : MonoBehaviour
{
    //model
    public GameCore gameCore;

    //view
    public GameBoardView gameBoardView;

    private void Start() {
        //gameboard -> cardView
        gameCore._gameBoard.Subscribe(
            board => {
                gameBoardView.allCardList.Clear();
                gameBoardView.ApplyHand(board.playerAreaList[board.PlayerID].HandToData());
                for (int i = 0; i < 6; i++) {
                    List<int> heroList = new List<int>();
                    List<int> armedCardList = new List<int>();
                    for (int j = 0; j < board.playerAreaList[i].PlayerHeroCardList.Count; j++) {
                        heroList.Add(board.playerAreaList[i].PlayerHeroCardList[j].hero.ID);
                        armedCardList.Add(board.playerAreaList[i].PlayerHeroCardList[j].equip.ID);
                    }
                    gameBoardView.ApplyHero(heroList, armedCardList, i);
                    gameBoardView.ApplySlayedMonster(board.playerAreaList[i].SlayedListToData(), i);
                }
                gameBoardView.ApplyDiscardPile(board.GetdeckArea.PileToData());
                gameBoardView.ApplyMonster(board.GetmonsterArea.ListToData());
                foreach (GameObject a in gameBoardView.allCardList) {
                    CardView cv = a.GetComponent<CardView>();

                    //�q�[���[�J�[�h���N���b�N���ꂽ�Ƃ�
                    cv.heroTrigger.OnPointerDownAsObservable()
                        .Subscribe(
                            x => {
                                if (x.pointerId == -1) { //���N���b�N
                                    gameCore.IndicateCard(cv.IsLarge,cv.HeroID);
                                }
                                else if (x.pointerId == -2) { //�E�N���b�N
                                    GameBoardAddress gba = new GameBoardAddress();
                                    gba = gameCore.gameBoard.Value.SearchCard(cv.HeroID, cv.IsLarge);
                                    //Debug.Log(gba.area + "," + gba.playerID + "," + gba.order);
                                    gameCore.SetFromAddress(gba);
                                    if (cv.IsLarge) gameCore.commandPanelModel.Value = gameCore.commandPanelModel.Value.OpenLargeCommandPanel(CommandPanelView.PanelName.main, Input.mousePosition);
                                    else gameCore.commandPanelModel.Value = gameCore.commandPanelModel.Value.OpenSmallCommandPanel(CommandPanelView.PanelName.main, Input.mousePosition);
                                }
                                
                            }
                            );

                    //�q�[���[�J�[�h�̏�Ƀ|�C���^�[��������
                    const float mouse_over_time = 0.75f;
                    cv.heroTrigger.OnPointerEnterAsObservable()
                        .SelectMany(_ => Observable.Timer(TimeSpan.FromSeconds(mouse_over_time)))
                        .TakeUntil(cv.heroTrigger.OnPointerExitAsObservable()) //PointerExit���ꂽ��X�g���[�������Z�b�g����
                        .RepeatUntilDestroy(this.gameObject) // ���ʂ܂ňȏ���J��Ԃ�
                        .Subscribe(
                            x => {
                                gameBoardView.OpenIndicator(cv.HeroID, cv.IsLarge,a);
                            }
                            );
                    cv.heroTrigger.OnPointerExitAsObservable()
                        .Subscribe(
                            x => {
                                gameBoardView.CloseIndicator();
                            }
                            );

                    //�A�C�e���J�[�h���N���b�N���ꂽ��
                    cv.itemTrigger.OnPointerDownAsObservable()
                        .Subscribe(
                            x => {
                                if (x.pointerId == -1) { //left click
                                    gameCore.IndicateCard(cv.IsLarge, cv.ItemID);
                                }
                                if (x.pointerId == -2) { //right click
                                    GameBoardAddress gba = new GameBoardAddress();
                                    gba = gameCore.gameBoard.Value.SearchCard(cv.ItemID, cv.IsLarge);
                                    Debug.Log(gba.area + "," + gba.playerID + "," + gba.order);
                                    gameCore.SetFromAddress(gba);
                                    if (cv.IsLarge) gameCore.commandPanelModel.Value = gameCore.commandPanelModel.Value.OpenLargeCommandPanel(CommandPanelView.PanelName.main, Input.mousePosition);
                                    else gameCore.commandPanelModel.Value = gameCore.commandPanelModel.Value.OpenSmallCommandPanel(CommandPanelView.PanelName.main, Input.mousePosition);
                                }
                            }
                            );
                    //�A�C�e���J�[�h�̏�Ƀ|�C���^�[��������
                    cv.itemTrigger.OnPointerEnterAsObservable()
                        .SelectMany(_ => Observable.Timer(TimeSpan.FromSeconds(mouse_over_time)))
                        .TakeUntil(cv.heroTrigger.OnPointerExitAsObservable()) //PointerExit���ꂽ��X�g���[�������Z�b�g����
                        .RepeatUntilDestroy(this.gameObject) // ���ʂ܂ňȏ���J��Ԃ�
                        .Subscribe(
                            x => {
                                gameBoardView.OpenIndicator(cv.ItemID, cv.IsLarge,a);
                            }
                            );
                    cv.itemTrigger.OnPointerExitAsObservable()
                        .Subscribe(
                            x => {
                                gameBoardView.CloseIndicator();
                            }
                            );
                }
            }

            ).AddTo(this);
    }
}

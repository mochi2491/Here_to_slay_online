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
                    cv.heroTrigger.OnPointerDownAsObservable()
                        .Subscribe(
                            x => {
                                if (x.pointerId == -1) { //left click
                                    gameCore.leftClickIsLarge = cv.IsLarge;
                                    gameCore.SetClickedID(cv.HeroID);
                                }
                                else if (x.pointerId == -2) { //right click
                                    GameBoardAddress gba = new GameBoardAddress();
                                    gba = gameCore.gameBoard.Value.SearchCard(cv.HeroID, cv.IsLarge);
                                    Debug.Log(gba.area + "," + gba.playerID + "," + gba.order);
                                    gameCore.SetFromAddress(gba);
                                    if (cv.IsLarge) gameCore.commandPanelModel.Value = gameCore.commandPanelModel.Value.OpenLargeCommandPanel(CommandPanelView.PanelName.main, Input.mousePosition);
                                    else gameCore.commandPanelModel.Value = gameCore.commandPanelModel.Value.OpenSmallCommandPanel(CommandPanelView.PanelName.main, Input.mousePosition);
                                }
                                
                            }
                            );
                    const float mouse_over_time = 0.75f;
                    cv.heroTrigger.OnPointerEnterAsObservable()
                        .SelectMany(_ => Observable.Timer(TimeSpan.FromSeconds(mouse_over_time)))
                        .TakeUntil(cv.heroTrigger.OnPointerExitAsObservable()) //PointerExitされたらストリームをリセットする
                        .RepeatUntilDestroy(this.gameObject) // 死ぬまで以上を繰り返す
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
                    cv.itemTrigger.OnPointerDownAsObservable()
                        .Subscribe(
                            x => {
                                if (x.pointerId == -1) { //left click
                                    gameCore.leftClickIsLarge = cv.IsLarge;
                                    gameCore.SetClickedID(cv.ItemID);
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
                    cv.itemTrigger.OnPointerEnterAsObservable()
                        .SelectMany(_ => Observable.Timer(TimeSpan.FromSeconds(mouse_over_time)))
                        .TakeUntil(cv.heroTrigger.OnPointerExitAsObservable()) //PointerExitされたらストリームをリセットする
                        .RepeatUntilDestroy(this.gameObject) // 死ぬまで以上を繰り返す
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

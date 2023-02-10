using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

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
                foreach (CardView cv in gameBoardView.allCardList) {
                    cv.heroTrigger.OnPointerDownAsObservable()
                        .Subscribe(
                            x => {
                                GameBoardAddress gba = new GameBoardAddress();
                                gba = gameCore.gameBoard.Value.SearchCard(cv.HeroID, cv.IsLarge);
                                gameCore.SetFromAddress(gba);
                                if (x.pointerId == -2) { //right click
                                    if (cv.IsLarge) gameCore.commandPanelModel.Value = gameCore.commandPanelModel.Value.OpenLargeCommandPanel(CommandPanelView.PanelName.main, Input.mousePosition);
                                    else gameCore.commandPanelModel.Value = gameCore.commandPanelModel.Value.OpenSmallCommandPanel(CommandPanelView.PanelName.main, Input.mousePosition);
                                }
                                
                            }
                            );
                    cv.itemTrigger.OnPointerDownAsObservable()
                        .Subscribe(
                            x => {
                                GameBoardAddress gba = new GameBoardAddress();
                                gba = gameCore.gameBoard.Value.SearchCard(cv.ItemID, cv.IsLarge);
                                gameCore.SetFromAddress(gba);
                                if (x.pointerId == -2) { //right click
                                    if (cv.IsLarge) gameCore.commandPanelModel.Value = gameCore.commandPanelModel.Value.OpenLargeCommandPanel(CommandPanelView.PanelName.main, Input.mousePosition);
                                    else gameCore.commandPanelModel.Value = gameCore.commandPanelModel.Value.OpenSmallCommandPanel(CommandPanelView.PanelName.main, Input.mousePosition);
                                }
                            }
                            );
                }
            }

            );
    }
}

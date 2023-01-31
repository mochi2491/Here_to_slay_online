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
        gameCore._gameBoard.Subscribe(
            board => {
                gameBoardView.ApplyHand(board.playerAreaList[board.PlayerID].HandToData());
                for(int i = 0; i < 6; i++) {
                    List<int> heroList = new List<int>();
                    List<int> armedCardList = new List<int>();
                    for(int j = 0;j < board.playerAreaList[i].PlayerHeroCardList.Count;j++){
                        heroList.Add(board.playerAreaList[i].PlayerHeroCardList[j].hero.ID);
                        armedCardList.Add(board.playerAreaList[i].PlayerHeroCardList[j].equip.ID);
                    }
                    gameBoardView.ApplyHero(heroList,armedCardList,i);
                    gameBoardView.ApplySlayedMonster(board.playerAreaList[i].SlayedListToData(),i);
                }
                gameBoardView.ApplyDiscardPile(board.GetdeckArea.PileToData());
                gameBoardView.ApplyMonster(board.GetmonsterArea.ListToData());
            }
            );
    }
}

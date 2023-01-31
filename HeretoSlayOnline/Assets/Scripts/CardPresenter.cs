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
                gameBoardView.ApplyHand(board.playerList[playerID.Value].playerHandList);
                for(int i = 0; i < 6; i++) {
                    List<int> heroList = new List<int>();
                    List<int> armedCardList = new List<int>();
                    for(int j = 0;j < board.playerList[i].playerHeroCardList.Count;j++){
                        heroList.Add(board.playerList[i].playerHeroCardList[j].cardID);
                        armedCardList.Add(board.playerList[i].playerHeroCardList[j].armedCardID);
                    }
                    gameBoardView.ApplyHero(heroList,armedCardList,i);
                    gameBoardView.ApplySlayedMonster(board.playerList[i].slayedMonsterList,i);
                }
                gameBoardView.ApplyDiscardPile(board.discardPile);
                gameBoardView.ApplyMonster(board.monsterCardList);
            }
            );
    }
}

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
                //initial
                //arrange cards

                //subscribe
            }
            );
    }
}

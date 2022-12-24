using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCore : MonoBehaviour
{
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
public class GameBoardModel : MonoBehaviour {

}

struct GameBoardData {
    List<int> discardPile;
    List<int> mainDeck;

    List<int> monsterCardList;
    List<int> monsterDeck;

    int turnPlayerNum;
    List<string> playerIDList;
    int leaderCardID;
    List<int> playerHandList;
    List<int> playerHeroCardList;
}


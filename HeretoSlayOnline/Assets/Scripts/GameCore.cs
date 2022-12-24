using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using UniRx;

public class GameCore : MonoBehaviour
{
    public ServerConnector connector;
    void Start()
    {
        connector._receivedMessage.Subscribe(
            x => {
                ApplyRecentBoard(x);
                }
        );
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void ApplyRecentBoard(string boardText) {
        
    }
}
public class GameBoardModel : MonoBehaviour {

}

public class ServerConnector : MonoBehaviour {
    private WebSocket ws;
    private ReactiveProperty<string> receivedMessage;
    public IReactiveProperty<string> _receivedMessage => receivedMessage;
    void Start() {
        ws = new WebSocket("wss://htsserver.5m8d.net/");
        ws.OnOpen += (sender, e) => {
            Debug.Log("Connect to server.");
        };
        ws.OnMessage += (sender, e) => {
            Debug.Log("received message.");
            receivedMessage.Value = e.Data;
        };

        ws.OnError += (sender, e) => {
            Debug.Log("WebSocket Error Message: " + e.Message);
        };

        ws.OnClose += (sender, e) => {
            Debug.Log("disconnect to server.");
        };
        ws.Connect();
    }

    void Update() {
    }


    void OnDestroy() {
        ws.Close();
        ws = null;
    }
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


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using UniRx;
using Newtonsoft.Json;

public class GameCore : SingletonMonoBehaviour<GameCore>
{
    public ServerConnector connector;
    GameBoardData currentGameBoard;
    void Start()
    {
        connector._receivedMessage.Subscribe(
            x => {
                ApplyRecentBoard(JsonToGameBoard(x));
            }
        );
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void ApplyRecentBoard(GameBoardData newBoard) {
        
    }

    private string GameBoardToJson(GameBoardData gbd) { 
        return JsonConvert.SerializeObject(gbd);
    }
    private GameBoardData JsonToGameBoard(string json) {
        return JsonConvert.DeserializeObject<GameBoardData>(json);
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

    string chatText;

    int turnPlayerNum;
    List<PlayerDate> playerList;
}

struct PlayerDate {
    List<string> playerIDList;
    int leaderCardID;
    List<int> playerHandList;
    List<int> playerHeroCardList;
}
 
public class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour {
    private static T instance;

    public static T Instance {
        get {
            if (instance == null) {
                instance = (T)FindObjectOfType(typeof(T));

                if (instance == null) {
                    Debug.LogError(typeof(T) + "Ç™ÉVÅ[ÉìÇ…ë∂ç›ÇµÇ‹ÇπÇÒÅB");
                }
            }

            return instance;
        }
    }

}

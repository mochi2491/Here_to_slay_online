using UnityEngine;
using System.Collections;
using WebSocketSharp;

public class SampleWebSocket : MonoBehaviour {

    private WebSocket ws;

    void Start() {
        ws = new WebSocket("wss://htsserver.5m8d.net/");

        ws.OnOpen += (sender, e) =>
        {
            Debug.Log("WebSocket Open");
        };

        ws.OnMessage += (sender, e) =>
        {
            Debug.Log("WebSocket Message Type: " + ", Data: " + e.Data);
        };

        ws.OnError += (sender, e) =>
        {
            Debug.Log("WebSocket Error Message: " + e.Message);
        };

        ws.OnClose += (sender, e) =>
        {
            Debug.Log("WebSocket Close");
        };

        ws.Connect();

    }

    void Update() {

        if (Input.GetKeyUp("a")) {
            ws.Send("0:::asdfa");
        }
        if (Input.GetKeyUp("s")) {
            ws.Send("2:::kon^^");
        }
        if (Input.GetKeyUp("d")) {
            ws.Send("3:::bye!");
        }

    }

    void OnDestroy() {
        ws.Close();
        ws = null;
    }
}
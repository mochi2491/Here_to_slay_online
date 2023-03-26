using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using WebSocketSharp;
using UniRx;
public class ServerConnector : MonoBehaviour
{
    private WebSocket ws;
    private ReactiveProperty<string> receivedMessage = new ReactiveProperty<string>("first");
    public IReactiveProperty<string> _receivedMessage => receivedMessage;

    private void Start()
    {
        ws = new WebSocket("wss://htsserver.5m8d.net/");
        var context = SynchronizationContext.Current;
        ws.OnOpen += (sender, e) =>
        {
            Debug.Log("Connect to server.");
        };
        ws.OnMessage += (sender, e) =>
        {
            Debug.Log("received message" + e.Data);
            context.Post(state =>
            {
                receivedMessage.Value = state.ToString();
            }, e.Data);
        };

        ws.OnError += (sender, e) =>
        {
            Debug.Log("WebSocket Error Message: " + e.Message);
        };

        ws.OnClose += (sender, e) =>
        {
            Debug.Log("disconnect to server.");
        };
        ws.Connect();
    }

    public void SendText(string text)
    {
        ws.Send(text);
    }

    private void OnDestroy()
    {
        ws.Close();
        ws = null;
    }
}

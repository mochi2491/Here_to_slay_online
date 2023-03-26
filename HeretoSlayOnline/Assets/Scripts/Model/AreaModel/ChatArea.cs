using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public interface IChatArea
{

    public void Init();

    public void AddLog(string text);
}

public class ChatArea
{
    private DateTime dt;
    private string userName = "";
    private ReactiveProperty<string> chatLog = new ReactiveProperty<string>("");
    public IReactiveProperty<string> _chatLog => chatLog;

    public ChatArea()
    {
        userName = "";
        chatLog.Value = "";
    }

    public void Init()
    {
        chatLog.Value = "";
    }

    public void SetUserName(string name)
    {
        userName = name;
    }

    public void AddLog(string text)
    {
        //chatLogにtextを追加する
        dt = DateTime.Now;
        chatLog.Value = chatLog.Value + "\n" + userName + ":" + dt.ToString("HH:mm:ss") + "\n" + text;
    }

    public void ApplyLog(string text)
    {
        chatLog.Value = text;
    }
}


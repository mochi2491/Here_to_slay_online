using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
public interface IEntrance
{
    public string GetUserName();
    public void SetState(GameState state);

    public void SetUserName(string name);

    public void SendUserName();

    public void ApplyIsReady(bool isReady);

    public void QuitGame();
}

public class Entrance : MonoBehaviour, IEntrance
{
    //member method
    //ユーザーネーム
    private ReactiveProperty<string> userName = new ReactiveProperty<string>("");
    public IReadOnlyReactiveProperty<string> _userName => userName;
    private bool isReady = false; //Playerの準備状況
    private GameState state; //現在のGameCoreのState


    //setter and getter
    public string UserName
    {
        get { return userName.Value; }
    }
    public string GetUserName()
    {
        return userName.Value;
    }

    //public method
    public void SetState(GameState state)
    {
        this.state = state;
    }

    public void SetUserName(string name)
    {
        this.userName.Value = name;
    }

    public void SendUserName()
    {
        if (state == GameState.entrance)
        {
            //changeState.Invoke(GameState.wait);
        }
    }

    public void ApplyIsReady(bool isReady)
    {
        if (state != GameState.wait) return;
        this.isReady = isReady;
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;//ゲームプレイ終了
#else
    Application.Quit();//ゲームプレイ終了
#endif
    }
}


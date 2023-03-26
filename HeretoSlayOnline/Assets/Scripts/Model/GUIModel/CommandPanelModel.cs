using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
public class CommandPanelModel
{
    private List<bool> isActive = new List<bool>();
    public bool closerActive = false;
    private Vector3 mousePos = Vector3.zero;
    public List<bool> IsActive { get { return isActive; } }
    public Vector3 MousePos { get { return mousePos; } }

    private ReactiveProperty<int> heroNum = new ReactiveProperty<int>(0);

    public CommandPanelModel()
    {
        int i = 0;
        for (i = 0; i < 6; i++)
        {
            isActive.Add(false);
        }
    }

    private CommandPanelModel(List<bool> list, Vector3 mousePos, bool closerActive)
    {
        isActive = list;
        this.mousePos = mousePos;
        this.closerActive = closerActive;
    }

    //全てのコマンドパネルを閉じるメソッド
    public CommandPanelModel CloseAllPanel()
    {
        int i = 0;
        List<bool> list = new List<bool>();
        for (i = 0; i < 6; i++)
        {
            list.Add(false);
        }
        return new CommandPanelModel(list, this.mousePos, false);
    }

    //最初にコマンドパネルを開くメソッド
    public CommandPanelModel OpenSmallCommandPanel(CommandPanelView.PanelName panel, Vector3 mousePos)
    {
        int i = 0;
        List<bool> list = new List<bool>();
        for (i = 0; i < 6; i++)
        {
            list.Add(false);
        }
        switch (panel)
        {
            case CommandPanelView.PanelName.main:
                list[0] = true;
                break;

            case CommandPanelView.PanelName.player:
                list[1] = true;
                break;

            case CommandPanelView.PanelName.order:
                list[2] = true;
                break;
        }
        return new CommandPanelModel(list, mousePos, true);
    }

    public CommandPanelModel OpenLargeCommandPanel(CommandPanelView.PanelName panel, Vector3 mousePos)
    {
        int i = 0;
        List<bool> list = new List<bool>();
        for (i = 0; i < 6; i++)
        {
            list.Add(false);
        }
        switch (panel)
        {
            case CommandPanelView.PanelName.main:
                list[3] = true;
                break;

            case CommandPanelView.PanelName.player:
                list[4] = true;
                break;

            case CommandPanelView.PanelName.order:
                list[5] = true;
                break;
        }
        return new CommandPanelModel(list, mousePos, true);
    }

    //開いたコマンドパネルを遷移させるメソッド
    public CommandPanelModel TransitionSmallPanel(CommandPanelView.PanelName panel)
    {
        int i = 0;
        List<bool> list = new List<bool>();
        for (i = 0; i < 6; i++)
        {
            list.Add(false);
        }
        switch (panel)
        {
            case CommandPanelView.PanelName.main:
                list[0] = true;
                break;

            case CommandPanelView.PanelName.player:
                list[1] = true;
                break;

            case CommandPanelView.PanelName.order:
                list[2] = true;
                break;
        }
        return new CommandPanelModel(list, this.mousePos, true);
    }

    public CommandPanelModel TransitionLargePanel(CommandPanelView.PanelName panel)
    {
        int i = 0;
        List<bool> list = new List<bool>();
        for (i = 0; i < 6; i++)
        {
            list.Add(false);
        }
        switch (panel)
        {
            case CommandPanelView.PanelName.main:
                list[3] = true;
                break;

            case CommandPanelView.PanelName.player:
                list[4] = true;
                break;

            case CommandPanelView.PanelName.order:
                list[5] = true;
                break;
        }
        return new CommandPanelModel(list, this.mousePos, true);
    }
}
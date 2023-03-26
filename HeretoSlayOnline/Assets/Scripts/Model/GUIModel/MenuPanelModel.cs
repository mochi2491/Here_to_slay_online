using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuPanelModel
{
    //今のところ必要のないクラスだが後に音量調整等を実装する際に必要となるので残す。
    public bool isActive = false;

    public MenuPanelModel()
    {
        isActive = false;
    }

    private MenuPanelModel(bool b)
    {
        isActive = b;
    }

    public MenuPanelModel Open()
    {
        return new MenuPanelModel(true);
    }

    public MenuPanelModel Close()
    {
        return new MenuPanelModel(false);
    }
}

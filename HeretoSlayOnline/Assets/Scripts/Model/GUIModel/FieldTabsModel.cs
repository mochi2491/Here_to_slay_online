using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public interface IFieldTabs
{

    public void SetVisibleTabNum(int num);
}
public class FieldTabsModel : MonoBehaviour, IFieldTabs
{
    private IntReactiveProperty visibleTabNum = new IntReactiveProperty(0);
    public IReadOnlyReactiveProperty<int> _visibleTabNum => visibleTabNum;

    public void SetVisibleTabNum(int num)
    {
        visibleTabNum.Value = num;
    }//tabの切り替え
}


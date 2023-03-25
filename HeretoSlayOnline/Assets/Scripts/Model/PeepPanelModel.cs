using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

public class PeepPanelModel {
    /// <summary>
    /// 相手の手札を覗くときのパネルのモデル
    /// </summary>
    //パネルが表示されているかどうか

    //表示する手札のリスト
    private ReactiveProperty<List<SmallCard>> handList = new ReactiveProperty<List<SmallCard>>();
    public IReadOnlyReactiveProperty<List<SmallCard>> _handList => handList;
    public PeepPanelModel(List<SmallCard> handList) {
        this.handList.Value = handList;
    }
    private PeepPanelModel WithHandList(List<SmallCard> handList) {
        return new PeepPanelModel(handList);
    }
    public void SetHandList(List<SmallCard> handList) {
        this.handList.Value = handList;
    }
}
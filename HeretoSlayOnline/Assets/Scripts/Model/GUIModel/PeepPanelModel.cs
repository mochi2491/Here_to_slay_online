using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

public class PeepPanelModel {
    /// <summary>
    /// ����̎�D��`���Ƃ��̃p�l���̃��f��
    /// </summary>
    //�p�l�����\������Ă��邩�ǂ���

    //�\�������D�̃��X�g
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
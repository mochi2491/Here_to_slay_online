using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;

public class CommandPanelView : MonoBehaviour
{
    public GameObject smallCommandPanel;
    public List<GameObject> smallPanels;
    public TMPro.TMP_Dropdown orderList;
    public GameObject largeCommandPanel;
    public List<GameObject> largePanels;
    public GameObject closerPanel;
    public ObservableEventTrigger closerTrigger;

    private void Start() {
        closerTrigger = closerTrigger.GetComponent<ObservableEventTrigger>();
    }

    public void ApplyModel(CommandPanelModel cpm) {
        int i = 0;
        foreach (bool active in cpm.IsActive) {
            if (i < 3) smallPanels[i].SetActive(active);
            else largePanels[i-3].SetActive(active);
            i++;
        }
        SetPosition(cpm.MousePos);
        closerPanel.SetActive(cpm.closerActive);
    }
    public void SetPosition(Vector3 pos) {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(pos.x, pos.y, 10.0f));
        if(mousePos.y < -2.5f) {
            mousePos.y = -2.5f;
        }
        smallCommandPanel.transform.position = mousePos;
        largeCommandPanel.transform.position = mousePos;
    }
    public void transitionSmallPanel(PanelName panel) {
        switch (panel) {
            case PanelName.main:
                smallPanels[0].SetActive(true);
                smallPanels[1].SetActive(false);
                smallPanels[2].SetActive(false);
                break;
            case PanelName.player:
                smallPanels[0].SetActive(false);
                smallPanels[1].SetActive(true);
                smallPanels[2].SetActive(false);
                break;
            case PanelName.order:
                smallPanels[0].SetActive(false);
                smallPanels[1].SetActive(false);
                smallPanels[2].SetActive(true);
                break;
        }
    }
    public void transitionLargePanel(PanelName panel) {
        switch(panel) { 
            case PanelName.main:
                largePanels[0].SetActive(true);
                largePanels[1].SetActive(false);
                largePanels[2].SetActive(false);
                break;
            case PanelName.player:
                largePanels[0].SetActive(false);
                largePanels[1].SetActive(true);
                largePanels[2].SetActive(false);
                break;
            case PanelName.order:
                largePanels[0].SetActive(false);
                largePanels[1].SetActive(false);
                largePanels[2].SetActive(true);
                break;
        }
    }
    public enum PanelName {
        main,player,order
    }
    
}

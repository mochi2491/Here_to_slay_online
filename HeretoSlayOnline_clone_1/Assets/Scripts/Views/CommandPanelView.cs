using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class CommandPanelView : MonoBehaviour
{
    public GameObject smallCommandPanel;
    public List<GameObject> smallPanels;
    public List<TextMeshProUGUI> smallPlayerButtonTexts;
    public List<TextMeshProUGUI> largePlayerButtonTexts;
    public TMP_Dropdown orderList;
    public GameObject largeCommandPanel;
    public List<GameObject> largePanels;
    public GameObject closerPanel;
    public ObservableEventTrigger closerTrigger;

    public Button[] smallButtons;

    private void Start() {
        for (int i = 0; i < smallPanels[1].transform.childCount; i++) {
            smallPlayerButtonTexts.Add(smallPanels[1].transform.GetChild(i).transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>());
            largePlayerButtonTexts.Add(largePanels[1].transform.GetChild(i).transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>());
        }
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

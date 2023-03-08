using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PeepPanelView : MonoBehaviour
{
    public GameObject peepPanel;
    public Button quitButton;

    private void Start() {
        peepPanel.SetActive(false);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using TMPro;
using UniRx.Triggers;

public class FieldTabsView : MonoBehaviour
{
    public int visibleTabNum = 0;
    public TextMeshProUGUI playerID;
    public GameObject[] fieldTabs;
    public Button[] fieldButtons;

    public TMP_Dropdown[] leaderSelector;
    public Image[] leaderImage;
    public Sprite[] leaderSprite = new Sprite[20];
    public ObservableEventTrigger[] leaderDescriptionTrigger;
    public TextMeshProUGUI[] handCount;

    public Button[] LeaderSkillButton;
    public Button[] peepButton; 
    private void Start() {
        leaderSprite = Resources.LoadAll("monster_and_leader_cards", typeof(Sprite)).Cast<Sprite>().ToArray();
    }
}

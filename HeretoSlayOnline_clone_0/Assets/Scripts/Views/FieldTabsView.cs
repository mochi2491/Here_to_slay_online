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
    public List<GameObject> fieldTabs;
    public List<Button> fieldButtons;

    public List<TMP_Dropdown> leaderSelector;
    public List<Image> leaderImage;
    public Sprite[] leaderSprite = new Sprite[20];
    public ObservableEventTrigger[] leaderDescriptionTrigger;

    public List<Button> LeaderSkillButton;
    private void Start() {
        leaderSprite = Resources.LoadAll("monster_and_leader_cards", typeof(Sprite)).Cast<Sprite>().ToArray();
    }
}

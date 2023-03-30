using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class EntranceView : MonoBehaviour
{
    public TMP_InputField userNameText;
    public Toggle isReadyToggle;
    public Button sendButton;
    public Button quitButton;
    public GameObject EntranceObject;

    private void Start()
    {
        EntranceObject.SetActive(true);
    }
}

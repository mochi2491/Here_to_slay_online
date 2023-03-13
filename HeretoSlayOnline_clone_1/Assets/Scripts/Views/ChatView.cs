using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChatView : MonoBehaviour
{
    public TMP_InputField input;    
    public TMP_Text chatLog;
    public Button diceButton;
    public Button sendButton;
    public ScrollRect scrollRect;

    private void Start() {
        scrollRect.verticalNormalizedPosition = 1.0f;
    }
}

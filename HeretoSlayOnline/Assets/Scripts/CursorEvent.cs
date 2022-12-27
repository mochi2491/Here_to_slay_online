using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CursorEvent : MonoBehaviour
{
    [SerializeField]
    private GameObject BasePanel;
    [SerializeField]
    private GameObject ActionSelect;
    [SerializeField]
    private GameObject PlayerSelect;
    [SerializeField]
    private GameObject HeroSelect;
    [SerializeField]
    private GameObject[] ActionButtons;

    private GameObject ClickedObject;
    private int order = 0;
    private int selectedPlayer = 0;

    public void onMouseEvent(BaseEventData eventData){
        var pointerEventData = eventData as PointerEventData;
        if(pointerEventData.pointerEnter.tag == "Card"){
            Debug.Log("効果表示の処理");
        }
    }
    public void exitMouseEvent(BaseEventData eventData){
        var pointerEventData = eventData as PointerEventData;
    }
    public void clickMouseEvent(BaseEventData eventData){
        var pointerEventData = eventData as PointerEventData;
        if(pointerEventData.pointerEnter.tag == "Card"){
            Debug.Log("テキストの表示");
            ActionSelect.SetActive(true);
            BasePanel.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x+250,Input.mousePosition.y+250,10.0f));
            ClickedObject = pointerEventData.pointerEnter;
            this.SelectButton(ClickedObject);
        }else{
            ActionSelect.SetActive(false);
        }
    }
    private void SelectButton(GameObject card){
        foreach(GameObject button in ActionButtons){
            button.SetActive(false);
        }
        ActionButtons[0].SetActive(true);
        ActionButtons[1].SetActive(true);
        ActionButtons[2].SetActive(true);
        ActionButtons[3].SetActive(true);
        ActionButtons[4].SetActive(true);
        ActionButtons[5].SetActive(true);
    }
    public void clickMovePanelButton(int orderNum){
        PlayerSelect.SetActive(true);
        ActionSelect.SetActive(false);
        order = orderNum;
    }
    public void clickSelectPlayerButton(int playerID){
        PlayerSelect.SetActive(false);
        switch(order){
            case 0:
                Debug.Log("ClickedObject -> Player"+playerID+"'s handの処理");
            break;
            case 1:
                Debug.Log("ClickedObject -> Player"+playerID+"'s fieldの処理");
            break;
            case 2:
                Debug.Log("Player"+playerID+"'s hand -> hand の処理");
            break;
            case 3:
                selectedPlayer = playerID;
                HeroSelect.SetActive(true);
            break;
        }
    }
    public void clickSelectHeroButton(int heroIndex){
        Debug.Log("ClickedObject -> "+selectedPlayer+"'s Hero["+heroIndex+"] の処理");
        HeroSelect.SetActive(false);
    }
    public void clickPileButton(){
        Debug.Log("ClickedObject -> Pile の処理");
        ActionSelect.SetActive(false);
    }
    public void clickDrawButton(){
        Debug.Log("deck -> hand の処理");
        ActionSelect.SetActive(false);
    }

}


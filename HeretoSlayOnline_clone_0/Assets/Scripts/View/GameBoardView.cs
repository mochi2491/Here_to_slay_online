using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameBoardView : MonoBehaviour {
    public GameCore gameCore;
    public GameObject handObject;
    public List<GameObject> heroObject;
    public List<GameObject> slayedmonsterObject;
    public GameObject discardpileObject;
    public GameObject monsterObject;

    public GameObject cardImageIndicator;
    public List<GameObject> allCardList;

    //private methods
    private void Reset(GameObject content) {
        foreach (CardView a in content.GetComponentsInChildren<CardView>()) {
            a.DestroySelf();
        }
    }

    private void Apply(List<int> data, GameObject parent, Area area, int holderNum, bool isLarge) {
        int i = 0;
        foreach (int id in data) {
            GameObject card = (GameObject)Resources.Load("CardObject");
            GameObject a = Instantiate(card, parent.transform);
            CardView view = a.GetComponent<CardView>();
            if (view.heroTrigger == null) Debug.Log("gg");
            if (id != -1) {
                view.ApplyData(CardSprites.GetSprite(id, isLarge), CardSprites.GetNullSprite());
            }
            else {
                view.ApplyData(CardSprites.GetNullSprite(), CardSprites.GetNullSprite());
            }
            view.SetData(data[i], -1, isLarge);
            i++;
            allCardList.Add(a);
        }
    }

    private void Apply(List<int> data, GameObject parent, Area area, int holderNum, bool isLarge, List<int> armedCardData) {
        for (int i = 0; i < data.Count; i++) {
            GameObject card = (GameObject)Resources.Load("CardObject");
            GameObject a = Instantiate(card, parent.transform);
            CardView view = a.GetComponent<CardView>();
            if (armedCardData[i] != -1) view.ApplyData(CardSprites.GetSprite(data[i], false), CardSprites.GetSprite(armedCardData[i], false));
            else {
                view.ApplyData(CardSprites.GetSprite(data[i], false), CardSprites.GetNullSprite());
            }
            view.SetData(data[i], armedCardData[i], isLarge);
            allCardList.Add(a);
        }
    }

    //public methods
    public void ApplyHand(List<int> data) {
        Reset(handObject);
        Apply(data, handObject, Area.playerHand, gameCore.playerID, false);
    }

    public void ApplyHero(List<int> heroData, List<int> armedCardData, int playerNum) {
        Reset(heroObject[playerNum]);
        Apply(heroData, heroObject[playerNum], Area.playerHero, playerNum, false, armedCardData);
    }

    public void ApplySlayedMonster(List<int> data, int playerNum) {
        Reset(slayedmonsterObject[playerNum]);
        Apply(data, slayedmonsterObject[playerNum], Area.slayedMonster, playerNum, true);
    }

    public void ApplyDiscardPile(List<int> data) {
        Reset(discardpileObject);
        Apply(data, discardpileObject, Area.discardPile, 0, false);
    }

    public void ApplyMonster(List<int> data) {
        Reset(monsterObject);
        Apply(data, monsterObject, Area.monsterList, 0, true);
    }

    public void OpenIndicator(int cardID, bool isLarge, GameObject card) {
        float movex = 0.5f;
        float movey = 0.5f;
        if (isLarge) {
            movex = 1;
            movey = -2.5f;
        }
        Vector3 a = new Vector3(card.transform.position.x + movex, card.transform.position.y + movey, card.transform.position.z);
        cardImageIndicator.transform.position = a;
        cardImageIndicator.GetComponent<Image>().sprite = CardSprites.GetSprite(cardID, isLarge);
        cardImageIndicator.SetActive(true);
    }

    public void CloseIndicator() {
        cardImageIndicator.SetActive(false);
    }
}
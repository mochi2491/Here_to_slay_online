using System.Linq;
using UnityEngine;

public static class CardSprites {
    private const string _smallCardPath = "deck_cards";
    private const string _largeCardPath = "monster_and_leader_cards";
    private const string _nullCardPath = "null";

    public static readonly int SMALLCARD_COUNT = 74; //デッキのカードの種類数
    public static readonly int LARGECARD_COUNT = 20; //モンスターカードとリーダーカードの種類数

    private static Sprite _nullSprite; //透明カードのスプライト
    private static Sprite[] _smallCardImageList = new Sprite[SMALLCARD_COUNT];
    private static Sprite[] _largeCardImageList = new Sprite[LARGECARD_COUNT];

    [RuntimeInitializeOnLoadMethod]
    private static void LoadSprite() {
        _smallCardImageList = Resources.LoadAll(_smallCardPath, typeof(Sprite)).Cast<Sprite>().ToArray();
        _largeCardImageList = Resources.LoadAll(_largeCardPath, typeof(Sprite)).Cast<Sprite>().ToArray();
        _nullSprite = Resources.Load<Sprite>(_nullCardPath);
    }

    public static Sprite GetSprite(int index, bool isLarge) {
        if (isLarge) {
            if (index < 0 || index >= LARGECARD_COUNT) {
                return null;
            }
            else {
                return _largeCardImageList[index];
            }
        }
        else {
            if (index < 0 || index >= SMALLCARD_COUNT) {
                return null;
            }
            else {
                return _smallCardImageList[index];
            }
        }
    }

    public static Sprite GetNullSprite() {
        return _nullSprite;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CardData {
    public class CardDataManager {
        //instance
        private List<CardData> largeCardData;
        private List<CardData> smallCardData;

        //constructor
        public CardDataManager() { 
            largeCardData = new List<CardData>();
            smallCardData = new List<CardData>();
        }
        private CardDataManager(List<CardData>largeList ,List<CardData>smallList) {
            largeCardData = largeList;
            smallCardData = smallList;
        }
        //private method

        //public method
        public string GetLargeScript(int id) {
            return largeCardData[id].GetDescription();
        }
        public string GetSmallScript(int id) {
            return smallCardData[id].GetDescription();
        }
        /// <summary>
        /// リーダーカードのデータを読み込む
        /// </summary>
        /// <param name="dataList"></param>
        /// <returns></returns>
        public CardDataManager LoadLeader(string[][] dataList) {
            CardDataManager cdm = new CardDataManager(largeCardData,smallCardData);
            foreach (string[] data in dataList) {
                LeaderCardScript a = new LeaderCardScript(data[0], data[1], data[2]);
                largeCardData.Add(a);
            }
            return cdm;
        }
        /// <summary>
        /// ヒーローカードのデータを読み込む
        /// </summary>
        /// <param name="dataList"></param>
        /// <returns></returns>
        public CardDataManager LoadHeroCard(string[][] dataList) {
            CardDataManager cdm = new CardDataManager(largeCardData,smallCardData);
            foreach (string[] data in dataList) {
                HeroCardScript a = new HeroCardScript(data[0], data[1], data[2], data[3]);
                smallCardData.Add(a);
            }
            return cdm;
        }
        /// <summary>
        /// モンスターカードのデータを読み込む
        /// </summary>
        /// <param name="dataList"></param>
        /// <returns></returns>
        public CardDataManager LoadMonsterCard(string[][] dataList) {
            CardDataManager cdm = new CardDataManager(largeCardData, smallCardData);
            foreach (string[] data in dataList) {
                MonsterCardScript a = new MonsterCardScript(data[0], data[1], data[2], data[3], data[4], data[5], data[6]);
                largeCardData.Add(a);
            }
            return cdm;
        }
        /// <summary>
        /// アイテムカードのデータを読み込む
        /// </summary>
        /// <param name="dataList"></param>
        /// <returns></returns>
        public CardDataManager LoadItemCard(string[][] dataList) {
            CardDataManager cdm = new CardDataManager(largeCardData, smallCardData);
            foreach (string[] data in dataList) {
                ItemCardScript a = new ItemCardScript(data[0], data[1], data[2]);
                smallCardData.Add(a);
            }
            return cdm;
        }
        /// <summary>
        /// マジックカードのデータを読み込む
        /// </summary>
        /// <param name="dataList"></param>
        /// <returns></returns>
        public CardDataManager LoadMagicCard(string[][] dataList) {
            CardDataManager cdm = new CardDataManager(largeCardData, smallCardData);
            foreach (string[] data in dataList) {
                MagicCardScript a = new MagicCardScript(data[0], data[1]);
                largeCardData.Add(a);
            }
            return cdm;
        }
        /// <summary>
        /// モディファイア―カードのデータを読み込む
        /// </summary>
        /// <param name="dataList"></param>
        /// <returns></returns>
        public CardDataManager LoadModifierCard(string[][] dataList) {
            CardDataManager cdm = new CardDataManager(largeCardData, smallCardData);
            foreach (string[] data in dataList) {
                ModifierCardScript a = new ModifierCardScript(data[0], data[1]); 
                smallCardData.Add(a);
            }
            return cdm;
        }
        /// <summary>
        /// チャレンジカードのデータをよみこむ
        /// </summary>
        /// <param name="dataList"></param>
        /// <returns></returns>
        public CardDataManager LoadChallengeCard() {
            CardDataManager cdm = new CardDataManager(largeCardData, smallCardData);
            ChallengeCardScript a = new ChallengeCardScript("challenge", "チャレンジ1年生");
            smallCardData.Add(a);
            return cdm;
        }
    }
    public interface CardData {
        public string GetDescription();
    }
    public class LeaderCardScript : CardData{
        private string name;
        private string className;
        private string effect;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="name">リーダーカードの名前</param>
        /// <param name="className">クラスの種類</param>
        /// <param name="effect">リーダースキルの効果</param>
        internal LeaderCardScript(string name, string className, string effect) {
            this.name = name;
            this.className = className;
            this.effect = effect;
        }
        public string GetDescription() {
            string description = name + "\n" + className + "\n" + effect;
            return description;
        }
    }
    public class HeroCardScript : CardData{
        private string name;
        private string className;
        private string dice;
        private string effect;
        internal HeroCardScript(string name, string className, string dice, string effect) {
            this.name = name;
            this.className = className;
            this.dice = dice;
            this.effect = effect;
        }
        public string GetDescription() {
            string description = name +"\n" + className + "\n" + dice + "\n" + effect ;
            return description;
        }
    }
    public class MonsterCardScript : CardData {
        private string name;
        private string requirement;
        private string range_A;
        private string range_B;
        private string effect_A;
        private string effect_B;
        private string effect;
        internal MonsterCardScript(string name, string requirement, string range_A, string range_B, string effect_A, string effect_B, string effect) {
            this.name = name;
            this.requirement = requirement;
            this.range_A = range_A;
            this.range_B = range_B;
            this.effect_A = effect_A;
            this.effect_B = effect_B;
            this.effect = effect;
        }
        public string GetDescription() {
            string description = name + "\n" + requirement + "\n" + range_A + ":" + effect_A + "\n" + range_B + ":" + effect_B + "\n" + effect;
            return description;
        }
    }
    public class ItemCardScript : CardData {
        private string name;
        private string type;
        private string effect;
        internal ItemCardScript(string name, string type, string effect) {
            this.name = name;
            this.type = type;
            this.effect = effect;
        }
        public string GetDescription() {
            string description = name + "\n" + type + "\n" + effect;
            return description;
        }
    }
    public class MagicCardScript : CardData{
        private string name;
        private string effect;
        internal MagicCardScript(string name, string effect) {
            this.name = name;
            this.effect = effect;
        }
        public string GetDescription() {
            string description = name + "\n" + effect;
            return description;
        }
    }
    public class ModifierCardScript : CardData {
        private string name;
        private string effect;
        internal ModifierCardScript(string name, string effect) {
            this.name = name;
            this.effect = effect;
        }
        public string GetDescription() {
            string description = name + "\n" + effect;
            return description;
        }
    }
    public class ChallengeCardScript : CardData{
        private string name;
        private string effect;
        public ChallengeCardScript(string name, string effect) {
            this.name = name;
            this.effect = effect;
        }
        public string GetDescription() {
            string description = name + "\n" + effect;
            return description;
        }
    }
}
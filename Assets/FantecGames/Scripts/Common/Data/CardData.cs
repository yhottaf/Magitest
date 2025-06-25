using System;
using System.Collections.Generic;
using fantec.Master;
using NUnit.Framework;

namespace fantec.Common
{
    public class CardData
    {
        public int cardId;
        public CardRarityType rarityType;
        public int totalExp;
        public DateTime purchaseDateTime;
        public int OverrideSkillLevel;
        public List<int> positionIndex = new List<int>();

        // 限界突破要素のステータスを追加するならここに記載

        public CardData(int cardId, CardRarityType rarityType, int totalExp, DateTime purchaseDateTime, int overrideSkillLevel,List<int>positionIndex)
        {
            this.cardId = cardId;
            this.rarityType = rarityType;
            this.totalExp = totalExp;
            this.purchaseDateTime = purchaseDateTime;
            this.OverrideSkillLevel = overrideSkillLevel;
            this.positionIndex = positionIndex;
        }

        public PlayerCardData CardMasterData() => MasterDataManager.Instance.PlayerCardMaster.GetData(cardId);
        public int GetHp()=>MasterDataManager.Instance.PlayerCardMaster.GetData(cardId).GetHpByExp(totalExp,rarityType);
        public int GetAtk() => MasterDataManager.Instance.PlayerCardMaster.GetData(cardId).GetAtkByExp(totalExp, rarityType);// 後々overLimitも追加することになるかも・・・
        public int GetSpd() => MasterDataManager.Instance.PlayerCardMaster.GetData(cardId).GetSpdByExp(totalExp, rarityType);
        public int GetSpec() => MasterDataManager.Instance.PlayerCardMaster.GetData(cardId).GetSpecByExp(totalExp, rarityType);

        // 属性耐性などのステータスを追加するならここに記載

        //進化データなど記載するならここに記載
    }
}
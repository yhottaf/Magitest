using UnityEngine;
using fantec.Master;

namespace fantec.Battle
{
    public class CardStateEntity
    {
        public readonly int HP;
        public readonly int ATK;
        public readonly int SPD;
        public readonly int damageRange;
        public readonly int originId;
        public readonly int Move;             // ˆÚ“®—Í
        public readonly string CharaName;     // ƒLƒƒƒ‰ƒl[ƒ€ 

        public CardStateEntity(AbstructCardData cardData, int level,bool leader)
        {
            float rate = leader ? 1.1f : 1.0f;

            this.HP = (int)(cardData.GetHpByLevel(level) * rate);
            this.ATK = (int)(cardData.GetAtkByLevel(level) * rate);
            this.SPD = (int)(cardData.GetSpdByLevel(level) * rate);
            this.damageRange = cardData.damageRange;
            this.originId= cardData.originId;
            this.Move = cardData.Move;
            this.CharaName = cardData.charaName;
        }

        public CardStateEntity(PlayerCardData cardData, int level, CardRarityType rarityType, int overLimitCount,bool leader)
        {
            float rate = leader ? 1.1f : 1.0f;

            this.HP = (int)(cardData.GetHpByLevel(level) * rate);
            this.ATK = (int)(cardData.GetAtkByLevel(level) * rate);
            this.SPD = (int)(cardData.GetSpdByLevel(level) * rate);
            this.damageRange = cardData.damageRange;
            this.originId = cardData.originId;
            this.Move = cardData.Move;
            this.CharaName=cardData.charaName;
        }
    }
    public static class CardStateEntityExtensions
    {
        public static CardStateEntity ToStateEntity(this AbstructCardData data, int level,bool leader)
        {
            return new CardStateEntity(data, level,leader);
        }

        public static CardStateEntity ToStateEntity(this PlayerCardData data, int level, CardRarityType rarityType, int overLimitCount,bool leader)
        {
            return new CardStateEntity(data, level, rarityType, overLimitCount,leader);
        }
    }
}
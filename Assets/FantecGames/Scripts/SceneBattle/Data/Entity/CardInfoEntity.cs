using fantec.Master;
using System.Collections.Generic;

namespace fantec.Battle
{
    public class CardInfoEntity
    {
        public readonly int cardId;
        public readonly int originId;
        public readonly string charaName;
        public readonly string[] tagNames;
        public readonly CardRarityType rarityType;
        public readonly AffectMoveType moveType;
        public readonly AffectHitType hitType;
        public readonly List<int> behaviorAI;
        public int positionIndex {  get;  set; }


        public CardInfoEntity(AbstructCardData cardData,CardRarityType rarityType,int positionIndex)
        {
            this.cardId= cardData.cardId;
            this.originId= cardData.originId;
            this.tagNames= cardData.tagNames;
            this.charaName= cardData.charaName;
            this.rarityType = rarityType;
            this.moveType = cardData.moveType;
            this.positionIndex= positionIndex;
            this.hitType = cardData.hitType;
            this.behaviorAI=cardData.behaviorAI;
        }

        // ボスなどの特殊なオーラをまとったり、スケール値をいじる場合はここに記載
    }

    public static class UnitEntityExtensions
    {
        public static CardInfoEntity ToInfoEntity(this AbstructCardData data,CardRarityType rarityType,int positionIndex)
        {
            return new CardInfoEntity(data,rarityType,positionIndex);
        }

        //public static CardInfoEntity ToInfoEntity(this AbstructCardData data,CardRarityType rarityType,int positionIndex,AuraType auraType,float charaScale)
        //{
        // return 
        //}
    }
}
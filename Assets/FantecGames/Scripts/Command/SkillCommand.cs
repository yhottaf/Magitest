using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
namespace fantec
{
    [System.Serializable]
    public class SkillCommand
    {
        public int affectValue;   // 効果量
        public int actionCount;   // 行動回数
        public int sustainTurn;   // 持続ターン
        public int randomCount;   // 効果対象の数 (ランダムでのみ使用)
        public bool isFirst;      // 攻撃よりも先に発動するか否か
        public AffectCategoryType categoryType;    // 種類
        public AffectAttributeType attributeType;  // 属性
        public AffectRangeType rangeType;          // 範囲
        public AffectEfficacyType efficacyType;    // 効果量

        public int successRate;   // 効果の付与率
        public int conditionValue;// 発動の条件値
        public AffectCategoryType parentCategoryType; // 付与効果などで使用される親のカテゴリー

        /// <summary>
        /// クラスを複製する
        /// </summary>
        /// <returns></returns>
        public SkillCommand Clone()
        {
            return (SkillCommand)MemberwiseClone();
        }

        /// <summary>
        /// 親カテゴリーを取得する
        /// </summary>
        /// <returns>　NONE の場合は categoryType で補完する　</returns>
        public AffectCategoryType GetComplementParentCategoryType()
        {
            return parentCategoryType == AffectCategoryType.NONE
                ? categoryType
                : parentCategoryType;
        }
    }

    public static class SkillCommandExtensions
    {
        public static SkillCommand SetTurn(this SkillCommand skillCommand, int turn)
        {
            skillCommand.sustainTurn = turn;
            return skillCommand;
        }

        public static List<SkillCommand> SetTurn(this List<SkillCommand>skillCommands,int turn)
        {
            foreach (var skillCommand in skillCommands) skillCommand.SetTurn(turn);
            return skillCommands;
        }

        public static SkillCommand SetIsFirst(this SkillCommand SkillCommand,bool enable)
        {
            SkillCommand.isFirst = enable;
            return SkillCommand;
        }

        public static List<SkillCommand>SetIsFirst(this List<SkillCommand> skillCommands,bool enable)
        {
            foreach(var skillCommand in skillCommands)skillCommand.SetIsFirst(enable);
            return skillCommands;
        }

        public static SkillCommand SetRangeType(this SkillCommand skillCommand,AffectRangeType rangeType)
        {
            skillCommand.rangeType = rangeType;
            return skillCommand;
        }

        public static List<SkillCommand>SetRangeType(this List<SkillCommand>skillCommands,AffectRangeType rangeType)
        {
            foreach(var skillCommand in skillCommands)skillCommand.SetRangeType(rangeType);
            return skillCommands;
        }

        public static SkillCommand SetParentCategoryType(this SkillCommand skillCommand,AffectCategoryType categoryType)
        {
            skillCommand.parentCategoryType= categoryType;
            return skillCommand;
        }

        public static List<SkillCommand>SetParentCategoryType(this List<SkillCommand>skillCommands,AffectCategoryType categoryType)
        {
            foreach (var skillCommand in skillCommands) skillCommand.SetParentCategoryType(categoryType);
            return skillCommands;
        }

        public static IEnumerable<SkillCommand>SetRangeType(this IEnumerable<SkillCommand>skillCommands,AffectRangeType rangeType)
        {
            foreach (var skillCommand in skillCommands) skillCommand.SetRangeType(rangeType);
            return skillCommands;
        }

        public static SkillCommand SetAttributeType(this SkillCommand skillCommand,AffectAttributeType attributeType)
        {
            skillCommand.attributeType= attributeType;
            return skillCommand;
        }

        public static IEnumerable<SkillCommand>SetAttributeType(this IEnumerable<SkillCommand>skillCommands,AffectAttributeType attributeType)
        {
            foreach(var skillCommand in skillCommands)skillCommand.SetAttributeType(attributeType);
            return skillCommands;
        }

        public static SkillCommand SetSuccessRate(this SkillCommand skillCommand,int successRate)
        {
            skillCommand.successRate= successRate;
            return skillCommand;
        }

        public static IEnumerable<SkillCommand>SetSuccessRate(this IEnumerable<SkillCommand>skillCommands,int successRate)
        {
            foreach(var skillCommand in skillCommands)skillCommand.SetSuccessRate(successRate);
            return skillCommands;
        }

        public static SkillCommand SetConditionValue(this SkillCommand skillCommand,int conditionValue)
        {
            skillCommand.conditionValue=conditionValue;
            return skillCommand;
        }

        public static IEnumerable<SkillCommand>SetConditionValue(this IEnumerable<SkillCommand>skillCommands,int conditionValue)
        {
            foreach (var skillCommand in skillCommands)skillCommand.SetConditionValue(conditionValue);
            return skillCommands;
        }

        public static IEnumerable<SkillCommand>InterpolationAttributeType(this IEnumerable<SkillCommand>@this,AffectAttributeType attributeType)
        {
            foreach(var skillCommand in @this)
            {
                if(skillCommand.attributeType==AffectAttributeType.NONE)
                {
                    skillCommand.attributeType= attributeType;
                }
            }
            return @this;
        }
    }
}
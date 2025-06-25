using UnityEngine;
using fantec.Battle.Manager;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace fantec.Battle
{
    public class TokenCell
    {
        public readonly SkillCommand command;
        public int Turn { get; set; }　// 持続ターン
        public bool IsFresh { get; set; } // 付与されたターンか否か
        public bool IsConsumable => Turn == 0; // 消費可能か否か

        //  public TokenCell(AffectInfo info) : this(info.Command) { }

        public TokenCell(SkillCommand command)
        {
            this.command = command;
            this.Turn = command.sustainTurn;
            this.IsFresh = command.GetComplementParentCategoryType().GetIsUnfreshable() ? false : true;
        }

        /// <summary>
        /// Turnを進行させる
        /// </summary>
        public int Progress()
        {
            if (Turn == -1) return Turn;
            else
            {
                return Turn=Mathf.Max(Turn-1, 0);
            }
        }

        /// <summary>
        /// 鮮度を落とす
        /// </summary>
        public void FleshDrop()
        {
            IsFresh = false;
        }

        /// <summary>
        /// ターンの消費種別を取得する
        /// </summary>
        /// <returns></returns>
        public AffectTurnConsumeType GetTurnConsumeType()
        {
            return command.GetComplementParentCategoryType().GetTurnConsumeType();
        }

        /// <summary>
        /// 正規化された引数が条件値以上か否か
        /// </summary>
        public bool GetIsAboveNormalizedValue(float value)
        {
            return command.conditionValue / 100.0f < value;
        }

        /// <summary>
        /// 正規化された引数が条件値以下か否か
        /// </summary>
        public bool GetIsBelowNormalizedValue(float value)
        {
            return command.conditionValue / 100.0f > value;
        }

        /// <summary>
        ///  Entityに変換する
        /// </summary>
        public TokenEntity ToEntity()
        {
            return Locator.Resolve<IBattleMasterManager>().OtherSkillMaster.GetData(command.categoryType, command.efficacyType).ToTokenEntity();
        }

        /// <summary>
        /// Entity に変換する (属性付き)
        /// </summary>
        public TokenEntity ToEntity(AffectAttributeType attributeType)
        {
            return Locator.Resolve<IBattleMasterManager>().OtherSkillMaster.GetData(command.categoryType,command.efficacyType).ToTokenEntity(attributeType);
        }

        public static TokenCell Create(AffectInfo info)
        {
            return new TokenCell(info.Command);
        }

        public static TokenCell Create(SkillCommand command)
        {
            return new TokenCell(command);
        }

        public static IEnumerable<TokenCell>Create(IEnumerable<SkillCommand>commands)
        {
            return commands.Select(command => Create(command));
        }
    }
}
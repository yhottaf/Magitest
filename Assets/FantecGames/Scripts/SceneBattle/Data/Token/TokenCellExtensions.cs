using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace fantec.Battle
{
    public static class TokenCellExtensions
    {
        public static bool GetisAnyToken(this IEnumerable<TokenCell> @this, AffectCategoryType categoryType)
        {
            return @this.Any(token => token.command.categoryType == categoryType);
        }

        public static bool GetTryTokenForCategory(this IEnumerable<TokenCell> @this, AffectCategoryType categoryType, out IEnumerable<TokenCell> result)
        {
            result = @this.Where(cell => cell.command.categoryType == categoryType);
            return result.Count() > 0;
        }

        public static bool GetTryBuffToken(this IEnumerable<TokenCell> @this, out IEnumerable<TokenCell> result)
        {
            result = @this.Where(cell => cell.command.categoryType.GetIsBuff());
            return result.Count() > 0;
        }

        public static bool GetTryDebuffToken(this IEnumerable<TokenCell> @this, out IEnumerable<TokenCell> result)
        {
            result = @this.Where(cell => cell.command.categoryType.GetIsDebuff());
            return result.Count() > 0;
        }

        public static bool GetTryDiselableToken(this IEnumerable<TokenCell> @this, out IEnumerable<TokenCell> result)
        {
            result = @this.Where(cell => cell.command.categoryType.GetIsDispelable());
            return result.Count() > 0;
        }

        public static bool GetTryAbnormalConditionToken(this IEnumerable<TokenCell> @this, out IEnumerable<TokenCell> result)
        {
            result = @this.Where(cell => cell.command.categoryType.GetIsAbnormalCondition());
            return result.Count() > 0;
        }

        public static TokenEntity ConvertToAbnormalConditionEntity(this IEnumerable<TokenCell> @this, AffectCategoryType categoryType, AffectAttributeType attributeType = AffectAttributeType.NONE)
        {
            return @this.Where(x => x.command.categoryType == categoryType) // カテゴリを絞る
                    .OrderByDescending(s => s.command.affectValue)          // 効果順に並べ変える
                    .First()                                                // 先頭(効果量が最も高い)　を取得
                    .ToEntity(attributeType);                               // エンティティに変換する
        }
    }
}
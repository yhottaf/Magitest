using System.Collections;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace fantec.Battle
{
    public static class AffectInfoExtensions
    {
        public static IEnumerable<AffectInfo>GetAttackInfos(this IEnumerable<AffectInfo>@this)
        {
            return @this.Where(info => info.Command.categoryType.GetIsAttack());
        }

        public static IEnumerable<AffectInfo>GetFirstInfos(this IEnumerable<AffectInfo>@this)
        {
            return @this
                .Where(info =>
                info.Command.isFirst &&                            // Å‰‚ÉŒÄ‚Î‚ê‚é
                info.Command.categoryType.GetIsAttack() == false); // UŒ‚ˆÈŠO
        }

        public static IEnumerable<AffectInfo>GetLateInfos(this IEnumerable<AffectInfo>@this)
        {
            return @this
                .Where(info =>
                info.Command.isFirst == false &&                   // Å‰‚ÉŒÄ‚Î‚ê‚¸
                info.Command.categoryType.GetIsAttack() == false); // UŒ‚ˆÈŠO
        }

        public static IEnumerable<AffectInfo>GetMainInfos(this IEnumerable<AffectInfo>@this)
        {
            return @this.Where(info => info.IsMain);
        }

        public static IEnumerable<AffectInfo>SetMain(this IEnumerable<AffectInfo>@this,bool enable)
        {
            return @this.Select(info=>info.SetMain(enable));
        }

        public static IEnumerable<AffectInfo>SortByPriority(this IEnumerable<AffectInfo>@this)
        {
            return @this.OrderBy(info=>info.Command.rangeType.GetPriority());
        }
    }
}
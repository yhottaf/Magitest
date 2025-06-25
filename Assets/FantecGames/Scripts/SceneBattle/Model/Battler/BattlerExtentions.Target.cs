using fantec.Battle.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace fantec.Battle
{
    public static partial class BattlerExtentions
    {
        #region State
        // ---------------------------------------------------------------------------
        // State
        // ---------------------------------------------------------------------------

        // TODO : 一番上、一番上から真ん中、一番下。一番後ろ、真ん中などを取得できるように改良しなけばならない

        /// <summary> 存在しているバトラーを取得 </summary>
        public static IEnumerable<IBattler> GetExistBattlers(this IEnumerable<IBattler> battlers) =>
            battlers.Where(battler=>battler.Unit.IsExist==true);

        /// <summary> 生存しているバトラーを取得 </summary>
        public static IEnumerable<IBattler> GetSurvivedBattlers(this IEnumerable<IBattler> battlers) =>
            battlers.Where(battler => battler.Unit.IsExist == true && battler.State.Health.IsDead == false);

        /// <summary> 死亡しているバトラーを取得 </summary>
        public static IEnumerable<IBattler> GetDeadBattlers(this IEnumerable<IBattler> battlers) =>
            battlers.Where(battler => battler.Unit.IsExist == true && battler.State.Health.IsDead == true);

        /// <summary> 自分を取得する </summary>
        public static IEnumerable<IBattler> GetMyselfBattlers(this IEnumerable<IBattler> battlers, IBattler me) =>
            battlers.Where(battler => battler.Equals(me) == true);

        /// <summary> 自分以外のバトラーを取得する  </summary>
        public static IEnumerable<IBattler> GetBesidesMeBattler(this IEnumerable<IBattler> battlers, IBattler me) =>
            battlers.Where(battler => battler.Equals(me) == false);

        /// <summary> カードクラスタイプが一致するBattlerを取得する </summary>
        //public static IEnumerable<IBattler> GetClassMatchBattlers(this IEnumerable<IBattler> battlers, CardClassType classType) =>
        //    battlers.Where(battler => battler.Unit.Entity.classType == classType);
        #endregion

        #region Param
        // ------------------------------------------------------------------------------------
        // Param
        // ------------------------------------------------------------------------------------

        /// <summary> Battler配列からランダムに1体取得する  </summary>
        public static IBattler GetRandomBattler(this IEnumerable<IBattler>battlers)
        {
            try { return battlers.ElementAt(UnityEngine.Random.Range(0, battlers.Count())); }
            catch { throw new IndexOutOfRangeException($"[count: {battlers.Count()}]"); }
        }

        /// <summary> HP が最も少ない Battler を取得  </summary>
        public static IBattler GetMinimumHpBattler(this IEnumerable<IBattler> battlers)
        {
            try { return battlers.OrderBy(battler => battler.State.CurrentHP).First(); }
            catch { throw new IndexOutOfRangeException($"{GetBattlerLog(battlers)}"); }
        }

        /// <summary> HP が最も高い Battler を取得  </summary>
        public static IBattler GetMaximumHpBattler(this  IEnumerable<IBattler>battlers)
        {
            try { return battlers.OrderByDescending(battler => battler.State.CurrentHP).FirstOrDefault(); }
            catch { throw new IndexOutOfRangeException($"{GetBattlerLog(battlers)}"); }
        }

        /// <summary> ATK が最も少ない Battler を取得  </summary>
        public static IBattler GetMinimumAtkBattler(this IEnumerable<IBattler>battlers)
        {
            try { return battlers.OrderBy(battler => battler.State.CurrentATK).First(); }
            catch { throw new IndexOutOfRangeException($"{GetBattlerLog(battlers)}"); }
        }

        /// <summary> ATK が最も高い Battler を取得  </summary>
        public static IBattler GetMaximumAtkBattler(this IEnumerable<IBattler>battlers)
        {
            try { return battlers.OrderByDescending(battler => battler.State.CurrentATK).First(); }
            catch { throw new IndexOutOfRangeException($"{GetBattlerLog(battlers)}"); }
        }

        /// <summary> SPD が最も少ない Battler を取得  </summary>
        public static IBattler GetMinimumSpdBattler(this IEnumerable<IBattler>battlers)
        {
            try { return battlers.OrderBy(battler => battler.State.CurrentSPD).FirstOrDefault(); }
            catch { throw new IndexOutOfRangeException($"{GetBattlerLog(battlers)}"); }
        }

        /// <summary> SPD が最も高い Battler を取得  </summary>
        public static IBattler GetMaximumSpdBattler(this IEnumerable<IBattler>battlers)
        {
            try { return battlers.OrderByDescending(battler => battler.State.CurrentSPD).First(); }
            catch { throw new IndexOutOfRangeException($"{GetBattlerLog(battlers)}"); }
        }

        /// <summary>
        /// 指定バトラーと同じ positionIndex を持ち、かつ自身を除いたバトラー一覧を取得
        /// </summary>
        public static IEnumerable<IBattler> GetSamePositionIndexOthers(this IEnumerable<IBattler> battlers, IBattler me)
        {
            int myIndex = me.Unit.Entity.positionIndex;
            return battlers.Where(b => !b.Equals(me) && b.Unit.Entity.positionIndex == myIndex);
        }

        /// <summary> 指定した個数分ランダムに取得 (被りなし)  </summary>
        public static IEnumerable<IBattler> GetRandomBattler(this IEnumerable<IBattler> battlers, int count) => GetBattlerUnoverlapping(battlers, count, GetRandomBattler);

        /// <summary> HP が最も高い Battler を指定数取得 (被りなし)  </summary>
        public static IEnumerable<IBattler> GetMaximumHpBattler(this IEnumerable<IBattler> battlers, int count) => GetBattlerUnoverlapping(battlers, count, GetMaximumHpBattler);

        /// <summary> HP が最も低い Battler を指定数取得 (被りなし)  </summary>
        public static IEnumerable<IBattler> GetMinimumHpBattler(this IEnumerable<IBattler> battlers, int count) => GetBattlerUnoverlapping(battlers, count, GetMinimumHpBattler);

        /// <summary> ATK が最も高い Battler を指定数取得 (被りなし)  </summary>
        public static IEnumerable<IBattler> GetMaximumAtkBattler(this IEnumerable<IBattler> battlers, int count) => GetBattlerUnoverlapping(battlers, count, GetMaximumAtkBattler);

        /// <summary> ATK が最も低い Battler を指定数取得 (被りなし)  </summary>
        public static IEnumerable<IBattler> GetMinimumAtkBattler(this IEnumerable<IBattler> battlers, int count) => GetBattlerUnoverlapping(battlers, count, GetMinimumAtkBattler);

        /// <summary> SPD が最も高い Battler を指定数取得 (被りなし)  </summary>
        public static IEnumerable<IBattler> GetMaximumSpdBattler(this IEnumerable<IBattler> battlers, int count) => GetBattlerUnoverlapping(battlers, count, GetMaximumSpdBattler);

        /// <summary> SPD が最も低い Battler を指定数取得 (被りなし)  </summary>
        public static IEnumerable<IBattler> GetMinimumSpdBattler(this IEnumerable<IBattler> battlers, int count) => GetBattlerUnoverlapping(battlers, count, GetMinimumSpdBattler);

        #endregion

        // ------------------------------------------------------------------------------------------
        // Unique
        // ------------------------------------------------------------------------------------------

        // TODO: AIの攻撃対象の優先順位を決定する
        /// <summary>
        /// 攻撃対象とする Battler を取得する
        /// </summary>
        /// <param name="battlers"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        public static IBattler GetTargetBattler(this IEnumerable<IBattler>battlers,AffectInfo info)
        {
          //  var positionIndex = info.Owner.Unit.Entity.positionIndex;
            var survivedBattlers=GetSurvivedBattlers(battlers);
            var candidateBattlerList = new List<IBattler>();


            foreach (IBattler battler in survivedBattlers)
            {
                if (battler.Unit.Entity.positionIndex == 2
                    || battler.Unit.Entity.positionIndex == 6
                    || battler.Unit.Entity.positionIndex == 3) 
                    candidateBattlerList.Add(battler);// 前衛にいる相手を狙う予定リストに格納
            }

            // 抽選対象がいれば
            if (candidateBattlerList.Count != 0)
            {
                return candidateBattlerList.GetMinimumHpBattler(); // 相手の中で一番HPの低い相手を狙う
            }
            // 抽選対象がいなければ
            else
            {
                return survivedBattlers.GetRandomBattler();
            }
        }

        #region Utils

        // ------------------------------------------------------------------------------------------
        // Utils
        // ------------------------------------------------------------------------------------------

        /// <summary>
        /// 指定関数を利用し重複させずに指定個数取得する
        /// </summary>
        private static IEnumerable<IBattler>GetBattlerUnoverlapping(IEnumerable<IBattler>battlers,int count,Func<IEnumerable<IBattler>,IBattler>onFunc)
        {
            if (count == 0) throw new Exception("0 を指定することはできません。");

            var task = battlers.ToList();
            var result =new List<IBattler>();

            for(int i=0;i<count;i++)
            {
                if (task.Count() <= 0) break;

                var rand = onFunc(task);
                result.Add(rand);
                task.Remove(rand);
            }

            return result;
        }

        #endregion
    }
}
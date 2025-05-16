using System;
using System.Collections.Generic;
using System.Linq;

namespace fantec.Battle.Model
{
    public interface IBattleModelAdventSkill : ILocatable, IDisposable
    {
        IObservable<IBattler[]> OnUpdateOrderObservable { get; }
        List<IBattler> ReserveList { get; }
        IBattler ReserveHead { get; }
        IBattler ReserveLast { get; }
        bool IsConsumable { get; }

        void SortByHeadInsert();

        void SortByHeadInsertDefinitelyFirstPlayer();


        void SortBySpeed();
    }

    public static class BattleModelAdventSkillExtentions
    {
        public static void RegistBattler(this IBattleModelAdventSkill @this,IBattler battler)
        {
            @this.ReserveList.Add(battler);
        }

        public static void RemoveBattler(this IBattleModelAdventSkill @this, IBattler battler)
        {
            @this.ReserveList.Remove(battler);
        }

        public static void Consume(this IBattleModelAdventSkill @this)
        {
            @this.ReserveHead.AdventSkill.Consume();
        }

        public static void ConsumeAll(this IBattleModelAdventSkill @this)
        {
            @this.ReserveHead.AdventSkill.ConsumeAll();
        }

        public static void Lottery(this IBattleModelAdventSkill @this)
        {
            var head = @this.ReserveHead;
            //TODO :ドライブスキルの条件追加
            if(head.GetIsEnemy()==true)
            {
                head.AdventSkill.AddDanger(); //敵の場合
            }
            else
            {
                head.AdventSkill.AddLottery();
            }
        }

        public static void ReplacePlayerBattlers(this IBattleModelAdventSkill @this,IEnumerable<IBattler> battlers)
        {
            @this.ReserveList.RemoveAll(battler=>battler.GetIsPlayer()); // 前回分クリア
            @this.ReserveList.AddRange(battlers);                        // 更新する
        }

        public static void ReplaceEnemyBattlers(this IBattleModelAdventSkill @this,IEnumerable<IBattler>battlers)
        {
            @this.ReserveList.RemoveAll(battler => battler.GetIsEnemy()); // 前回分クリア
            @this.ReserveList.AddRange(battlers);                         // 更新する
        }

        public static bool GetIsContains(this IBattleModelAdventSkill @this,IBattler battler)
        {
            return @this.ReserveList.Contains(battler);
        }

        public static bool GetIsConsumable(this IBattleModelAdventSkill @this)
        {
            return
                @this.ReserveHead.AdventSkill.IsConsumable == true; // 消費可能なスキルが存在している
        }

        public static IBattler GetReserveHeadBattler(this IBattleModelAdventSkill @this)
        {
            try { return @this.ReserveList.First(); }
            catch { throw new InvalidOperationException("要素が存在しません。"); }
        }

        public static IBattler GetReserveLastBattler(this IBattleModelAdventSkill @this)
        {
            try { return @this.ReserveList.Last(); }
            catch { throw new InvalidOperationException("要素が存在しません。"); }
        }

        public static IBattler GetReserveHeadPlayer(this IBattleModelAdventSkill @this)
        {
            try { return @this.ReserveList.Where(battler => battler.GetIsPlayer()).First(); }
            catch { throw new InvalidOperationException("要素が見つかりません。"); }
        }

        public static List<IBattler> GetSortByHeadInsertBattlerList(this IBattleModelAdventSkill @this)
        {
            var battler = @this.GetReserveHeadBattler();
            var result = new List<IBattler>(@this.ReserveList);
            result.Remove(battler);
            result.Insert(result.Count, battler);
            return result;
        }

        public static List<IBattler> GetSortBySpeedReserveList(this IBattleModelAdventSkill @this)
        {
            return @this.ReserveList.OrderByDescending(battler => battler.State.CurrentSPD).ToList();
        }
    }
}
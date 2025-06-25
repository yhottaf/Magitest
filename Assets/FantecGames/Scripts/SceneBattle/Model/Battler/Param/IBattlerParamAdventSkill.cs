using System;
using System.Collections.Generic;
using UnityEngine;

namespace fantec.Battle.Model
{
    public interface IBattlerParamAdventSkill : IDisposable,IResetable,IActionNotify
    {
        // スキルの発動を監視
        IObservable<AffectInfo> OnActivationObservable { get; }

        // 現在先頭にある発動待ちのスキルがなければ通常攻撃
        AbstructSkillEntity HeadEntity { get; }
        NormalAttackEntity NormalEntity { get; }

        // 次に発動可能なスキルが存在するか否か
        bool IsConsumable { get; }

        ActionCapsule GetActionCapsule(int index);
        void Consume();
        void ConsumeAll();

        void AddLottery();

        void AddDanger();
    }

    public interface IBattlerParamAdventSkillPrivate:IBattlerParamAdventSkill
    {
        // 保有しているスキルリスト
  //      List<AdventSkillEntity> OwnedEntityList { get; }
        // 事前予約中のスキルIDリスト配列
        ActionCapsule[] AdvanceActionCapsules { get; }
        // 発動待ちのスキルリスト
        ActionCapsule ReserveActionCapsule { get; }

    }

    public static class BattlerParamAdventSkillPrivateExtentions
    {
        public static AbstructSkillEntity GetHeadEntity(this IBattlerParamAdventSkillPrivate @this)
        {
            // 現状通常攻撃のみを取得

                return @this.NormalEntity;
        }

        /// <summary>
        /// 前詰めし、最後尾に通常攻撃を追加する
        /// </summary>
        public static void AddLottelyAdvanceReserve(this IBattlerParamAdventSkillPrivate @this)
        {
            @this.ShiftForwardAdvanceReserve(new ActionCapsule(@this.NormalEntity));
        }

        /// <summary>
        /// 前詰めし、最後尾にデンジャー保留を追加する
        /// </summary>
        public static void AddDangerAdvanceReserve(this IBattlerParamAdventSkillPrivate @this)
        {
            @this.ShiftForwardAdvanceReserve(new ActionCapsule(DangerEntity.GetEntity()));
        }

        /// <summary>
        /// 要素を前にずらし引数の値を後ろに加える
        /// </summary>
        private static void ShiftForwardAdvanceReserve(this IBattlerParamAdventSkillPrivate @this, ActionCapsule actionCapsule)
        {
            if (@this.AdvanceActionCapsules == null) return;
            for (int i = 0; i < @this.AdvanceActionCapsules.Length; i++)
            {
                // 最後尾の要素であれば
                if (i == @this.AdvanceActionCapsules.Length - 1)
                {
                    // 引数の値を入れ込む
                    @this.AdvanceActionCapsules[i] = actionCapsule;
                }
                // それ以外は
                else
                {
                    // ひとつ前にズラす
                    @this.AdvanceActionCapsules[i] = @this.AdvanceActionCapsules[i + 1];
                }
            }
        }

        /// <summary>
        /// 予約リスト内の要素すべてを抽選する
        /// </summary>
        public static ActionCapsule[] CreateAdvanceReserve(this IBattlerParamAdventSkillPrivate @this, int count)
        {
            // 作成したい個数分の配列を用意
            var resultLists = new ActionCapsule[count];

            for (int i = 0; i < count; i++)
            {
                // 抽選結果をリストに入れ込む(今回はスキルではなく通常攻撃を入れるのみ TODO :スキル関係)
                resultLists[i] = new ActionCapsule(@this.NormalEntity);
            }

            return resultLists;
        }

        /// <summary>
        /// 予約スキルを更新する
        /// </summary>
        public static void UpdateReserve(this IBattlerParamAdventSkillPrivate @this)
        {
            // 先頭を入れ込む
            @this.ReserveActionCapsule.Copy(@this.AdvanceActionCapsules[0]);
        }
    }
}
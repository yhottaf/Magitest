using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniRx;

namespace fantec.Battle.Model
{
    public interface IBattleModelOverrideSkill : ILocatable,IDisposable,IFookable,IResetable
    {
        /// <summary> カットイン監視 </summary>
        IObservable<OverrideSkillEntity> OnCutinObservable { get; }

        /// <summary> 発動中フラグ変動監視 </summary>
        IReadOnlyReactiveProperty<bool> OnIsActive { get; }

        /// <summary> 予約中のオーバーライドスキルが存在するか否か </summary>
        bool IsReserveExists { get; }

        /// <summary> 予約リストに追加する </summary>
        void Reserve(IBattler affecter);

        /// <summary> 予約リストから取り除く </summary>
        void Cancell(IBattler affecter);

        /// <summary> カットイン開始 </summary>
        void Activation();

        /// <summary> スキルの終了 </summary>
        void Deactivate();

        /// <summary> 予約中のリスト内の先頭にいるバトラー </summary>
        IBattler GetReserveHeadBattler();

        /// <summary> リストが空かもしれないが、安全に取得した場合に使用 </summary>
        public bool TryGetReserveHeadBattler(out IBattler battler);
    }
}
using System;
using System.Collections.Generic;
using UniRx;

namespace fantec.Battle.Model
{
    public interface IBattlerParamOverrideSkill : IDisposable,IResetable,IActionNotify
    {
        /// <summary> 予約状態の変動を監視 </summary>
        IObservable<bool> OnIsReserveReactive { get; }
        /// <summary> 封印状態の変動を監視 </summary>
        IObservable<bool> OnIsSealedReactive { get; }
        /// <summary> 発動中であるか否かを監視 </summary>
        IObservable<bool> OnIsInActivationReactive { get; }

        /// <summary> スキルが発動したか監視 </summary>
        IObservable<OverrideSkillEntity> OnCutinObservable { get; }
        /// <summary> スキルに伴った行動監視 </summary>
        IObservable<AffectInfo> OnActivationObservable { get; }
        /// <summary> 発動したスキルが終了したか監視 </summary>
        IObservable<Unit> OnDeactivationObservable { get; }

        /// <summary> ブラーを監視  </summary>
        IObservable<Unit> OnBlurObservable { get; }

        /// <summary> データ本体のリスト </summary>
        List<OverrideSkillEntity> EntityList { get; }

        /// <summary> 発動するデータの本体 </summary>
        OverrideSkillEntity Entity { get; }

        /// <summary> 封印状態か否か </summary>
        bool IsSealed { get; }
        /// <summary> 予約状態か否か </summary>
        bool IsReserve { get; }
        /// <summary> 発動中か否か </summary>
        bool IsActive { get; }
        /// <summary> 予約可能な状態か否か </summary>
        bool IsReserveable { get; }


        /// <summary> スキルを消費する </summary>
        public void Consume();
        /// <summary> スキルを終了する </summary>
        public void Deactivation();
        /// <summary> 予約状態の変更 </summary>
        void SetIsReserve(bool enable);
        /// <summary> 封印状態の変更 </summary>
        void SetIsSealed(bool enable);

        /// <summary> ミュート状態にする </summary>
        void SetMute(bool enable);

        /// <summary> 発動できるスキルがあるかをみる </summary>
        void SetEntity(int[]orignIds);

        /// <summary> ブラーを有効状態にする </summary>
        void ActivateBlur();
    }
}
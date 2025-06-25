using fantec.Battle.Model;
using UniRx;
using UnityEngine;

namespace fantec.Battle.Field.Chara
{
    public interface ICharaPresenter
    {
        IBattler Battler { get; }

        void OnInitialize(IBattler battler);

        void OnUpdateUnitData(IBattler unit);

        void OnReload(IBattler unit);

        void OnOverrideCutin(OverrideSkillEntity data);

        void OnSkillAction(AffectInfo info);

        void OnOverrideEnd(Unit unit);

        void OnTakeBuff(AffectInfo info);

        void OnTakeDamage(TakeDamageInfo info);

        void OnTakeHeal(TakeHealInfo info);

        void OnDead(AffectInfo info);

        void OnRevival(AffectInfo info);

        void OnMove(Vector3 toPosition);

        void OnMoveCompleted(Unit unit);

        void OnActivateBlur(Unit unit);
    }

    public static class CharaPresenterExtentions
    {
        public static void Initialize(this ICharaPresenter @this, IBattler battler, CompositeDisposable disposables)
        {
            @this.OnInitialize(battler);
            battler.OnSetupCompleted.Subscribe(@this.OnUpdateUnitData).AddTo(disposables);
            battler.OnReload.Subscribe(@this.OnReload).AddTo(disposables);
            battler.AdventSkill.OnActivationObservable.Subscribe(@this.OnSkillAction).AddTo(disposables);
            battler.OverrideSkill.OnBlurObservable.Subscribe(@this.OnActivateBlur).AddTo(disposables);
            battler.OverrideSkill.OnCutinObservable.Subscribe(@this.OnOverrideCutin).AddTo(disposables);
            battler.OverrideSkill.OnActivationObservable.Subscribe(@this.OnSkillAction).AddTo(disposables);
            battler.OverrideSkill.OnDeactivationObservable.Subscribe(@this.OnOverrideEnd).AddTo(disposables);
            battler.State.OnTakeBuffObservable.Subscribe(@this.OnTakeBuff).AddTo(disposables);
            battler.State.Health.OnTakeDatamgeObservable.Subscribe(@this.OnTakeDamage).AddTo(disposables);
            battler.State.Health.OnTakeHealObservable.Subscribe(@this.OnTakeHeal).AddTo(disposables);
            battler.State.Health.OndeadObservable.Subscribe(@this.OnDead).AddTo(disposables);
            battler.State.Health.OnRevivalObservable.Subscribe(@this.OnRevival).AddTo(disposables);
            battler.Transform.OnMoveObservable.Subscribe(@this.OnMove).AddTo(disposables);
            battler.Transform.OnMoveCompletedObservable.Subscribe(@this.OnMoveCompleted).AddTo(disposables);
        }
    }
}
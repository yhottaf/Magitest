using fantec.Battle.Model;
using UniRx;

namespace fantec.Battle.Field.Chara
{
    public interface IStatePresenter
    {
        void OnUpdateUnitData(IBattlerParamUnit unit);
        void OnUpdateHealth(ValueChangeInfo info);
        void OnDead(AffectInfo info);
        void OnRevival(AffectInfo info);
    }

    public static class StatePresenterExtensions
    {
        public static void Initialize(this IStatePresenter @this,IBattler battler,CompositeDisposable disposables)
        {
            battler.Unit.OnUnitDataObservable                 .Subscribe(@this.OnUpdateUnitData) .AddTo(disposables);
            battler.State.Health.OnHealthChangeObservable     .Subscribe(@this.OnUpdateHealth)   .AddTo(disposables);
            battler.State.Health.OndeadObservable             .Subscribe(@this.OnDead)           .AddTo(disposables);
            battler.State.Health.OnRevivalObservable          .Subscribe(@this.OnRevival)        .AddTo(disposables);
            // TODO : HPの他に何か管理するメーターなどがあればここで管理
        }
    }
}
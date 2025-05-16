using fantec.Battle.Model;
using System.Collections.Generic;
using UniRx;

namespace fantec.Battle.Field.Chara
{

    public interface IBuffPresenter
    {
        void OnUpdateToken(List<TokenCell> cellList);
        void OnDead();
    }
    public static class BuffPresenterExtentions
    {
        public static void Initialize(this IBuffPresenter @this,IBattler battler,CompositeDisposable disposables)
        {
            battler.State.OnUpdateTokenCellList.Subscribe(@this.OnUpdateToken).AddTo(disposables);
            battler.State.Health.OndeadObservable.Subscribe(_=>@this.OnDead()).AddTo(disposables);
        }
    }
}
using System;
using UniRx;

namespace fantec.Battle.Model
{
    public interface IActionNotify
    {
        void Activation(AffectInfo info);

        IObservable<Unit> OnActionCompletedObservable {  get; } // アクションの完了通知を監視
        void ActionCompleted(); // アクションが完了したことを通知する
    }
}
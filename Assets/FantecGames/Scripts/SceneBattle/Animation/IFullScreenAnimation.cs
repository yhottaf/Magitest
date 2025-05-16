using System;
using UniRx;

namespace fantec.Battle.Animations
{
    public interface IFullScreenAnimation<T>:ICloseable
    {
        IObservable<Unit> OnEnd { get; }
        CompositeDisposable ClosedDisposables { get; }
        T OnCreate();
    }
}
using fantec.Battle.Animations;
using fantec.Battle.Utiles;
using System;
using UniRx;
using UnityEngine;

namespace fantec.Battle
{
    public interface IWinAnimation:IFullScreenAnimation<IWinAnimation>
    {

    }
}

namespace fantec.Battle.Animations
{
    public class WinAnimation : AnimationBase,IWinAnimation
    {
        [SerializeField]
        private AnimatorHashStateObservable m_HashStateObservable;
        public IObservable<Unit> OnEnd => m_HashStateObservable.Where(enabled => enabled).Select(_ => Unit.Default);
        public CompositeDisposable ClosedDisposables { get; private set; }=new CompositeDisposable();

        public IWinAnimation OnCreate()
        {
            OnEnd.DelayFrame(1).Subscribe(_ => this.Close()).AddTo(this);
            return this;
        }

        public void Close()
        {
            ClosedDisposables.Dispose();
            Destroy(this.gameObject);
        }
    }
}
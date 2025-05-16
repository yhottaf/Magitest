using fantec.Battle.Animations;
using fantec.Battle.Utiles;
using System;
using UniRx;
using UnityEngine;

namespace fantec.Battle
{
    public interface IBlackoutAnimation:IFullScreenAnimation<IBlackoutAnimation>
    {
        void Hide();
    }
}

namespace fantec.Battle.Animations
{
    public class BlackoutAnimation : AnimationBase,IBlackoutAnimation
    {
        public IObservable<Unit> OnEnd => m_HashStateObservable.Where(enabled => enabled).Select(_=>Unit.Default);
        public CompositeDisposable ClosedDisposables { get; private set; }=new CompositeDisposable();

        [SerializeField] private AnimatorHashStateObservable m_HashStateObservable;
        [SerializeField] private Animator m_Animator;

        private readonly int m_HasHideTrigger = Animator.StringToHash("HideTrigger");

        public IBlackoutAnimation OnCreate()
        {
            OnEnd.DelayFrame(1).Subscribe(_=>Close()).AddTo(this);
            return this;
        }

        public void Hide()
        {
            m_Animator.SetTrigger(m_HasHideTrigger);
        }

        public void Close()
        {
            ClosedDisposables.Dispose();
            Destroy(this.gameObject);
        }
    }
}
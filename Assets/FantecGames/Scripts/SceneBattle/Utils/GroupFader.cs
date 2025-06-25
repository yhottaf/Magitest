using DG.Tweening;
using fantec.Battle.Utiles;
using System;
using UniRx;
using UnityEngine;

namespace fantec.Battle
{
    public class GroupFader : SpeedableBehaviour
    {
        public IObservable<Unit> OnHideCompleted => m_HideCompletedSubject;
        public IObservable<Unit> OnShowCompleted => m_ShowCompletedSubject;

        [Header("Fade Setting")]
        [SerializeField] CanvasGroup m_CanvasGroup;
        [SerializeField] private float m_Duration = 0.3f;
        [SerializeField] private float m_ViewTime = 1.0f;
        [SerializeField] private bool m_IsSpeedable = true;

        SequenceStacker m_Sequence=new SequenceStacker();

        private readonly Subject<Unit> m_ShowCompletedSubject=new Subject<Unit>();
        private readonly Subject<Unit> m_HideCompletedSubject=new Subject<Unit>();

        protected override void Awake()
        {
            base.Awake();

            base.SetThroughDirectingPause(true);
        }

        public void PlayShowAndHide()
        {
            m_CanvasGroup.alpha = 0;
            m_Sequence?.Kill();
            m_Sequence.Value = DOTween.Sequence()
                .Append(m_CanvasGroup.DOFade(1, m_Duration))
                .AppendInterval(m_ViewTime)
                .Append(m_CanvasGroup.DOFade(0, m_Duration))
                .OnComplete(() => m_HideCompletedSubject.OnNext(Unit.Default))
                .SetLink(this.gameObject);
        }

        public void PlayShow()
        {
            m_CanvasGroup.alpha = 0;
            m_Sequence?.Kill();
            m_Sequence.Value=DOTween.Sequence()
                .Append(m_CanvasGroup.DOFade(1,m_Duration))
                .OnComplete(()=>m_ShowCompletedSubject.OnNext(Unit.Default))
                .SetLink(this.gameObject);
        }

        public void PlayHide()
        {
            m_Sequence?.Kill();
            m_Sequence.Value = DOTween.Sequence()
                .Append(m_CanvasGroup.DOFade(0, m_Duration))
                .OnComplete(() => m_HideCompletedSubject.OnNext(Unit.Default))
                .SetLink(this.gameObject);
        }

        protected override void OnSetCurrentTimeScale(float speed)
        {
           if(m_IsSpeedable)
            {
                m_Sequence.TimeScale= speed;
            }
        }
    }
}
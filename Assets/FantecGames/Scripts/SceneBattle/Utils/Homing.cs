using DG.Tweening;
using System;
using UniRx;
using UnityEngine;

namespace fantec.Battle.Utiles
{
    public class Homing : SpeedableBehaviour
    {
        public IObservable<Unit> OnCompleted => m_OnCompleted;

        [SerializeField] private float m_Height = 1f;
        [SerializeField] private float m_Duration = 1.0f;
        [SerializeField] private Ease m_EaseType = Ease.Linear;
        [SerializeField] private TrailRenderer m_TrailRenderer;
        private SequenceStacker m_Sequence=new SequenceStacker();
        private Subject<Unit> m_OnCompleted = new Subject<Unit>();

        protected override void Awake()
        {
            base.Awake();

            SetThroughDirectingPause(true);
        }

        protected override void OnSetCurrentTimeScale(float speed)
        {
            m_Sequence.TimeScale = speed;
        }

        public void PlayThrow(Vector3 end)
        {
            PlayThrow(end, m_Height, m_Duration);
        }

        public void PlayThrow(Vector3 start,Vector3 end)
        {
            PlayThrow(start,end,m_Height,m_Duration);
        }

        public void PlayThrow(Vector3 end,float height,float duration)
        {
            PlayThrow(this.transform.position, end, height, duration);
        }

        public void PlayThrow(Vector3 start,Vector3 end,float height,float duration)
        {
            PlayThrow(start, CalcHalf(start, end, height), end, duration);
        }

        public void PlayThrow(Vector3 start,Vector3 half,Vector3 end)
        {
            PlayThrow(start, half, end, m_Duration);
        }

        // 中間地点を設けて補間しながらパーティクルを飛ばす
        public void PlayThrow(Vector3 start,Vector3 half,Vector3 end,float duration)
        {
            if(m_TrailRenderer)
            {
                m_TrailRenderer.Clear();
                m_TrailRenderer.gameObject.SetActive(false);
            }

            m_Sequence?.Complete();
            m_Sequence.Value = DOTween.Sequence()
                .OnStart(() =>
            {
                if (m_TrailRenderer) m_TrailRenderer.gameObject.SetActive(true);
            }).Append(DOVirtual.Float(0, 1, duration, value =>
            {
                this.transform.position = CalcLerpPoint(start, half, end, value);
            }).SetEase(m_EaseType))
            .OnComplete(() =>
            {
                m_OnCompleted.OnNext(Unit.Default);
            })
            .SetLink(this.gameObject);
        }

        private Vector3 CalcHalf(Vector3 start,Vector3 end, float height)
        {
            var half = end - start * 0.5f + start;
            half.y += Vector3.up.y + height;
            return half;
        }

        private Vector3 CalcLerpPoint(Vector3 p0,Vector3 p1,Vector3 p2,float t)
        {
            var a = Vector3.Lerp(p0, p1, t);
            var b = Vector3.Lerp(p1, p2, t);
            return Vector3.Lerp(a, b, t);
        }

        // 中間補間を指定せずに真っすぐに飛ばすバージョン
        public void PlayStraight(Vector3 start,Vector3 end)
        {
            PlayStraight(start,end, m_Duration);
        }

        public void PlayStraight(Vector3 start,Vector3 end, float duration)
        {
            if (m_TrailRenderer)
            {
                m_TrailRenderer.Clear();
                m_TrailRenderer.gameObject.SetActive(false);
            }

            m_Sequence?.Complete();
            m_Sequence.Value = DOTween.Sequence()
                .OnStart(() =>
                {
                    this.transform.position = start;
                    if (m_TrailRenderer) m_TrailRenderer.gameObject.SetActive(true);
                }).Append(DOVirtual.Float(0, 1, duration, value =>
                {
                    transform.DOMove(end, duration);
                }).SetEase(m_EaseType))
                .OnComplete(() =>
                {
                    m_OnCompleted.OnNext(Unit.Default);
                })
                .SetLink(this.gameObject);
        }

    }
}
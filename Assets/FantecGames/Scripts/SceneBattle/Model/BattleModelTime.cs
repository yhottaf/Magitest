using JetBrains.Annotations;
using System;
using UniRx;
using UnityEngine;

namespace fantec.Battle.Model
{
    public interface IBattleModelTime:ILocatable,IDisposable
    {
        IReadOnlyReactiveProperty<float> OnSystemTimeScale { get; }
        IReadOnlyReactiveProperty<float> OnGameTimeScale { get; }
        IReadOnlyReactiveProperty<float> OnDirectingTimeScale { get; }
        IReadOnlyReactiveProperty<float> OnCurrentTimeScale { get; }
        IReadOnlyReactiveProperty<float> OnIgnoreTimeScale { get; }
        IReadOnlyReactiveProperty<bool> OnIsSystemPause { get; }
        IReadOnlyReactiveProperty<bool> OnIsDirectingPause { get; }

        void SetGameTimeScale(float timeScale);
        void SetGameTimeScaleX1();
        void SetGameTimeScaleX2();
        void SetGameTimeScaleX4();
        void SetDirectingTimeScaleDefault();
        void SetDirectingTimeScaleSlow();
        void SetSystemPause(bool enabled);
        void SetDirectingPause(bool enabled);
    }
    public class BattleModelTime : MonoBehaviour,IBattleModelTime,IRegistable
    {
        [SerializeField] private FloatReactiveProperty m_SystemTimeScaleReactive = new FloatReactiveProperty(1);
        [SerializeField] private FloatReactiveProperty m_GameTimeScaleReactive = new FloatReactiveProperty(1);
        [SerializeField] private FloatReactiveProperty m_DirectingTimeScaleReactive = new FloatReactiveProperty(1);
        [SerializeField] private FloatReactiveProperty m_CurrentTimeScaleReactive=new FloatReactiveProperty(1);
        [SerializeField] private FloatReactiveProperty m_IgnoreTimeScaleReactive = new FloatReactiveProperty(1);
        [SerializeField] private BoolReactiveProperty m_IsSystemPauseReactive=new BoolReactiveProperty();
        [SerializeField] private BoolReactiveProperty m_IsDirectingPauseReactive=new BoolReactiveProperty();

        public IReadOnlyReactiveProperty<float> OnSystemTimeScale => m_SystemTimeScaleReactive;
        public IReadOnlyReactiveProperty<float> OnGameTimeScale => m_GameTimeScaleReactive;
        public IReadOnlyReactiveProperty<float> OnDirectingTimeScale => m_DirectingTimeScaleReactive;
        public IReadOnlyReactiveProperty<float> OnCurrentTimeScale => m_CurrentTimeScaleReactive;
        public IReadOnlyReactiveProperty<float> OnIgnoreTimeScale => m_IgnoreTimeScaleReactive;
        public IReadOnlyReactiveProperty<bool> OnIsSystemPause => m_IsSystemPauseReactive;
        public IReadOnlyReactiveProperty<bool> OnIsDirectingPause => m_IsDirectingPauseReactive;

        public void Register()
        {
            Locator.Register<IBattleModelTime>(this);
        }

        public void Dispose()
        {
            m_SystemTimeScaleReactive.Dispose();
            m_GameTimeScaleReactive.Dispose();
            m_DirectingTimeScaleReactive.Dispose();
            m_CurrentTimeScaleReactive.Dispose();
            m_IgnoreTimeScaleReactive.Dispose();
            m_IsSystemPauseReactive.Dispose();
            m_IsDirectingPauseReactive.Dispose();
        }

        public void SetGameTimeScale(float timeScale)
        {
            m_GameTimeScaleReactive.Value = timeScale;
            UpdateCurrentTimeScale();
            UpdateIgnoreTimeScale();
        }

        public void SetGameTimeScaleX1()
        {
            SetGameTimeScale(1);
        }

        public void SetGameTimeScaleX2()
        {
            SetGameTimeScale(2);
        }

        public void SetGameTimeScaleX4()
        {
            SetGameTimeScale(4);
        }

        public void SetDirectingTimeScaleDefault()
        {
            m_DirectingTimeScaleReactive.Value = 1;
            UpdateCurrentTimeScale();
            UpdateIgnoreTimeScale();
        }

        public void SetDirectingTimeScaleSlow()
        {
            m_DirectingTimeScaleReactive.Value = 0.25f;
            UpdateCurrentTimeScale();
            UpdateIgnoreTimeScale();
        }

        public void SetSystemPause(bool enabled)
        {
            m_IsSystemPauseReactive.Value= enabled;
            m_SystemTimeScaleReactive.Value= enabled ? 0 : 1;
            UpdateCurrentTimeScale();
            UpdateIgnoreTimeScale();
        }
        public void SetDirectingPause(bool enabled)
        {
            m_IsDirectingPauseReactive.Value= enabled;
        }

        private void UpdateCurrentTimeScale()
        {
            m_CurrentTimeScaleReactive.Value =
                m_SystemTimeScaleReactive.Value *
                m_GameTimeScaleReactive.Value *
                m_DirectingTimeScaleReactive.Value;
        }

        private void UpdateIgnoreTimeScale()
        {
            m_IgnoreTimeScaleReactive.Value =
                m_SystemTimeScaleReactive.Value *
                m_GameTimeScaleReactive.Value;
        }
    }
}
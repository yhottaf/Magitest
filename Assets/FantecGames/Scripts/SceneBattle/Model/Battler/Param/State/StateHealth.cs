using System;
using UniRx;

namespace fantec.Battle.Model
{
    public class StateHealth : IStageGauge,IDisposable,IResetable
    {
        public IObservable<ValueChangeInfo> OnHealthChangeObservable => m_HealthChangeSubject;
        private readonly Subject<ValueChangeInfo> m_HealthChangeSubject = new Subject<ValueChangeInfo>();

        public IObservable<TakeDamageInfo> OnTakeDatamgeObservable => m_TakeDamageSubject;
        private readonly Subject<TakeDamageInfo>m_TakeDamageSubject= new Subject<TakeDamageInfo>();

        public IObservable<TakeHealInfo> OnTakeHealObservable => m_TakeHealSubject;
        private readonly Subject<TakeHealInfo>m_TakeHealSubject=new Subject<TakeHealInfo>();

        public IObservable<AffectInfo> OndeadObservable => m_DeadSubject;
        private readonly Subject<AffectInfo>m_DeadSubject=new Subject<AffectInfo>();

        public IObservable<AffectInfo> OnRevivalObservable => m_RevivalSubject;
        private readonly Subject<AffectInfo>m_RevivalSubject=new Subject<AffectInfo>();

        public float NormalizedHealth => (float)m_CurrentHealth / m_MaxHealth; // Š„‡
        public int MaxHealth => m_MaxHealth;
        public int CurrentHealth => m_CurrentHealth;
        public bool IsDead => m_CurrentHealth <= 0;

        private int m_CurrentHealth;
        private int m_MaxHealth;
        private bool m_IsDisable;
        private bool m_IsGuts;

        public void Dispose()
        {
            m_HealthChangeSubject.Dispose();
            m_TakeDamageSubject.Dispose();
            m_TakeHealSubject.Dispose();
            m_DeadSubject.Dispose();
            m_RevivalSubject.Dispose();
        }

        public void Reset()
        {
            m_CurrentHealth = 0;
            m_MaxHealth = 0;
            m_IsGuts = false;
            m_IsDisable = false;
        }

        public void SetDisable(bool enable)
        {
            m_IsDisable = enable;
        }

        public void SetGuts(bool enable)
        {
            m_IsGuts= enable;
        }

        public void SetMaxHealth(int value,bool isTakeover=false)
        {
            m_MaxHealth = value;
            if (isTakeover == false) m_CurrentHealth = value;
            m_HealthChangeSubject.OnNext(this.GetCurrentInfo());
        }

        public void TakeDamage(AffectInfo affectInfo)
        {
            var damageInfo = GetDamageInfo(affectInfo);

            if (affectInfo.GetIsAffectable())
            {
                m_CurrentHealth = damageInfo.valueInfo.ClampedNewValue;
                m_HealthChangeSubject.OnNext(damageInfo.valueInfo);
            }
            m_TakeDamageSubject.OnNext(damageInfo);

            if(IsDead)
            {
                m_DeadSubject.OnNext(affectInfo);
            }
        }

        public void TakeHeal(AffectInfo affectInfo)
        {
            var healInfo=GetHealInfo(affectInfo);

            if(affectInfo.GetIsAffectable())
            {
                m_CurrentHealth=healInfo.valueInfo.ClampedNewValue;
                m_HealthChangeSubject.OnNext(healInfo.valueInfo);
            }
            m_TakeHealSubject.OnNext(healInfo);
        }

        public void Kill(AffectInfo affectInfo)
        {
            var info = this.GetKillValueInfo();

            m_CurrentHealth = info.ClampedNewValue;
            m_HealthChangeSubject.OnNext(info);
            m_DeadSubject.OnNext(affectInfo);
        }

        public void Revive(AffectInfo affectInfo)
        {
            var info = this.GetReviveValueInfo();

            m_CurrentHealth= info.ClampedNewValue;
            m_HealthChangeSubject.OnNext(info);
            m_RevivalSubject.OnNext(affectInfo);
        }

        private TakeHealInfo GetHealInfo(AffectInfo affectInfo)
        {
            var valueInfo = this.GetHealValueInfo(affectInfo.CalcedAffectValue);

            return new TakeHealInfo()
            {
                affectInfo = affectInfo,
                valueInfo = valueInfo,
            };
        }

        private TakeDamageInfo GetDamageInfo(AffectInfo affectInfo)
        {
            var valueInfo = this.GetDamageValueInfo(affectInfo.CalcedAffectValue);

            if(m_IsGuts==true&&this.GetIsZero()==true||m_IsDisable==true)
            {
                m_IsGuts = false;
                valueInfo.newValue = 1;
            }

            return new TakeDamageInfo()
            {
                affectInfo = affectInfo,
                valueInfo = valueInfo,
            };
        }
    }
}
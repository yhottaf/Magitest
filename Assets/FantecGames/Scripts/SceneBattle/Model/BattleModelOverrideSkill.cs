using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

namespace fantec.Battle.Model
{
    public class BattleModelOverrideSkill : MonoBehaviour, IBattleModelOverrideSkill, IRegistable
    {
        public IObservable<OverrideSkillEntity> OnCutinObservable => m_CutinSubject;
        public IReadOnlyReactiveProperty<bool> OnIsActive => m_IsActiveReactive;
        public IReadOnlyReactiveProperty<bool> OnIsFook => m_IsFookReactive;

        [SerializeField] private BoolReactiveProperty m_IsActiveReactive = new BoolReactiveProperty();
        [SerializeField] private BoolReactiveProperty m_IsFookReactive = new BoolReactiveProperty();
        private readonly Subject<OverrideSkillEntity> m_CutinSubject = new Subject<OverrideSkillEntity>();
        private readonly List<IBattler>m_ReserveList=new List<IBattler>();

        public bool IsReserveExists => m_ReserveList.Count != 0;

        public void Register()
        {
            Locator.Register<IBattleModelOverrideSkill>(this);
        }

        public void Dispose()
        {
            m_CutinSubject.Dispose();
            m_IsActiveReactive.Dispose();
            m_IsFookReactive.Dispose();
        }

        void IResetable.Reset()
        {
            m_ReserveList.Clear();
            m_IsFookReactive.Value = false;
            m_IsActiveReactive.Value = false;
        }


        public void Reserve(IBattler affecter)
        {
            affecter.OverrideSkill.SetIsReserve(true);
            m_ReserveList.Add(affecter);

            ReserveCheck();
        }

        public void Cancell(IBattler affecter)
        {
            affecter.OverrideSkill.SetIsReserve(false);
            m_ReserveList.Remove(affecter);

            ReserveCheck();
        }

        public void Activation()
        {
            m_IsActiveReactive.Value = true;

            var battler = GetReserveHeadBattler();
            battler.OverrideSkill.Consume();
            m_CutinSubject.OnNext(battler.OverrideSkill.Entity);

            ReserveCheck();
        }

        public void Deactivate()
        {
            m_IsActiveReactive.Value = false;

            var battler = GetReserveHeadBattler();
            battler.OverrideSkill.Deactivation();
            m_ReserveList.Remove(battler);

            ReserveCheck();
        }

        public IBattler GetReserveHeadBattler()
        {
            try
            {
                return m_ReserveList[0];
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new ArgumentOutOfRangeException(nameof(m_ReserveList), "発動予約中のキャラクターがいない状態で呼ばれました");
            }
        }

        private void ReserveCheck()
        {
            if (m_ReserveList.Count == 0)          // 予約中のスキルがなければ
            {
                m_IsFookReactive.Value = false; // フックを無効化
            }
            else                                // 予約中のスキルがある状態で
            {
                if (m_IsActiveReactive.Value == false) // 発動中でなければ
                {
                    m_IsFookReactive.Value = true; // フックを有効化
                }
            }
        }

        public bool TryGetReserveHeadBattler(out IBattler battler)
        {
            if(m_ReserveList.Count>0)
            {
                battler= m_ReserveList[0];
                return true;
            }

            battler = null;
            return false;
        }
    }
}
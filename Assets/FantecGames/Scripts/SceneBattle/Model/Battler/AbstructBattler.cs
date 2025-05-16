using Cysharp.Threading.Tasks;
using fantec.Common;
using System;
using UniRx;
using UnityEngine;

namespace fantec.Battle.Model
{
    public abstract class AbstructBattler : IBattler
    {
        public IBattlerParamUnit Unit => m_Unit;
        public IBattlerParamState State => m_State;
        public IBattlerParamTransform Transform => m_Transform;
        public IBattlerParamAdventSkill AdventSkill => m_AdventSkill;
        public IBattlerParamOverrideSkill OverrideSkill => m_OverrideSkill;
        public IObservable<IBattler> OnSetupCompleted => m_OnSetupCompleted;
        public IObservable<IBattler> OnReload => m_OnReload;

        protected readonly BattlerParamUnit m_Unit=new BattlerParamUnit();
        protected readonly BattlerParamState m_State=new BattlerParamState();
        protected readonly BattlerParamAdventSkill m_AdventSkill = new BattlerParamAdventSkill();
        protected readonly BattlerParamOverrideSkill m_OverrideSkill = new BattlerParamOverrideSkill();
        protected readonly BattlerParamTransform m_Transform=new BattlerParamTransform();

        private readonly Subject<IBattler>m_OnSetupCompleted=new Subject<IBattler>();
        private readonly Subject<IBattler>m_OnReload=new Subject<IBattler>();

        public virtual void Dispose()
        {
            m_Unit.Dispose();
            m_State.Dispose();
            m_AdventSkill.Dispose();
            m_OverrideSkill.Dispose();
            m_Transform.Dispose();
        }

        public virtual void Reset()
        {
            m_Unit.Reset();
            m_State.Reset();
            m_AdventSkill.Reset();
            m_OverrideSkill.Reset();
            m_Transform.Reset();

            SetupCompleted();
        }

        public virtual void Reload()
        {
            m_State.Reload();

            m_OnReload.OnNext(this);
        }

        protected void SetupCompleted()
        {
            m_OnSetupCompleted.OnNext(this);
        }

        public abstract void SetUp(TeamData.Unit unit, bool isTakeover = false,bool leader=false);
    }
}
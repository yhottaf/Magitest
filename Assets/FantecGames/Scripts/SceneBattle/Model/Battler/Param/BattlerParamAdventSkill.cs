using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace fantec.Battle.Model
{
    public class BattlerParamAdventSkill : IBattlerParamAdventSkillPrivate
    {
        // フューチャービューの最大表示数 仮で5に設定中 TODO
        public const int RESERVE_MAX_COUNT = 5;

        public IObservable<AffectInfo> OnActivationObservable => m_ActivationSubject;
        public IObservable<Unit> OnActionCompletedObservable => m_ActionCompletedSubject;

        public ActionCapsule ReserveActionCapsule => m_ReserveActionCopsule;
        public ActionCapsule[] AdvanceActionCapsules => m_AdvanceActionCapsules;
        public AbstructSkillEntity HeadEntity => this.GetHeadEntity();
        public NormalAttackEntity NormalEntity => m_NormalAttackEntity;
        public bool IsConsumable => m_ReserveActionCopsule.IsConsumable;

        private readonly Subject<AffectInfo> m_ActivationSubject = new Subject<AffectInfo>();
        private readonly Subject<Unit> m_ActionCompletedSubject = new Subject<Unit>();

        private ActionCapsule m_ReserveActionCopsule;
        private ActionCapsule[] m_AdvanceActionCapsules;
        private NormalAttackEntity m_NormalAttackEntity;


        public void Setup(NormalAttackEntity normalAttackEntity)
        {
            m_NormalAttackEntity = normalAttackEntity;
            m_ReserveActionCopsule = new ActionCapsule();
            m_AdvanceActionCapsules = this.CreateAdvanceReserve(RESERVE_MAX_COUNT);
        }

        public void Reset()
        {

        }

        public void Dispose()
        {
            m_ActivationSubject.Dispose();
            m_ActionCompletedSubject.Dispose();
        }

        public ActionCapsule GetActionCapsule(int index)
        {
            try { return m_AdvanceActionCapsules[index];}
            catch { throw new IndexOutOfRangeException($"{index}　は予約可能な範囲を超えています。{nameof(RESERVE_MAX_COUNT)} の値を{index + 1}に更新してください。"); }
        }

        public void AddLottery()
        {
            // 通常攻撃を追加
            this.AddLottelyAdvanceReserve();

            // スキル本体を更新する
            this.UpdateReserve();
        }

        public void AddDanger()
        {
            // デンジャー用のIDリスト配列を抽選
            this.AddDangerAdvanceReserve();

            //スキル本体を更新する
            this.UpdateReserve();
        }

        public void Consume()
        {
            // 消費可能なスキルがあれば
            if(IsConsumable)
            {
                // リストの先頭から消費
                m_ReserveActionCopsule.Dequeue();
            }
        }

        public void ConsumeAll()
        {
            m_ReserveActionCopsule.Clear();
        }

        public void Activation(AffectInfo info)
        {
            m_ActivationSubject.OnNext(info);
        }

        public void ActionCompleted()
        {
            m_ActionCompletedSubject.OnNext(Unit.Default);
        }
    }
}
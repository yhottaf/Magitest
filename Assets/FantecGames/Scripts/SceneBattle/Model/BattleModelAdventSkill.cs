using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

namespace fantec.Battle.Model
{
    public class BattleModelAdventSkill : MonoBehaviour,IBattleModelAdventSkill,IRegistable
    {
        public IObservable<IBattleModelAdventSkill> OnInitCompletedObservable;

        public IObservable<IBattler[]> OnUpdateOrderObservable => m_UpdateOrderSubject;
        public IBattler ReserveHead => this.GetReserveHeadBattler();
        public IBattler ReserveLast => this.GetReserveLastBattler();
        public bool IsConsumable => this.GetIsConsumable();
        public List<IBattler> ReserveList => m_ReserveList;
        private List<IBattler> m_ReserveList = new List<IBattler>();

        private readonly AsyncSubject<IBattleModelAdventSkill> m_InitCompletedSubject = new AsyncSubject<IBattleModelAdventSkill>();
        private readonly Subject<IBattler[]> m_UpdateOrderSubject = new Subject<IBattler[]>();

        public void Register()
        {
            Locator.Register<IBattleModelAdventSkill>(this);
        }

        public void Dispose()
        {
            m_InitCompletedSubject.Dispose();
            m_UpdateOrderSubject.Dispose();
        }

        public void SortByHeadInsert()
        {
            m_ReserveList = this.GetSortByHeadInsertBattlerList();

            // 更新を通知
            m_UpdateOrderSubject.OnNext(m_ReserveList.ToArray());
        }

        public void SortByHeadInsertDefinitelyFirstPlayer()
        {
            if(m_ReserveList.GetIsPlayer()==false)
            {
                throw new Exception("プレイヤーが存際しない状態でこの関数は呼べません。");
            }
            while(m_ReserveList.First().GetIsPlayer()==false)
            {
                m_ReserveList = this.GetSortByHeadInsertBattlerList();
            }

            // 更新を通知
            m_UpdateOrderSubject.OnNext(m_ReserveList.ToArray());
        }

        public void SortBySpeed()
        {
            m_ReserveList = this.GetSortBySpeedReserveList();

            // 更新を通知
            m_UpdateOrderSubject.OnNext(m_ReserveList.ToArray());
        }
    }
}
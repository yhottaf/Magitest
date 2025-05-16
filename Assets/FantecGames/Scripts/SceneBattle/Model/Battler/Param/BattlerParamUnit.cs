using System;
using UniRx;
using UnityEngine;


namespace fantec.Battle.Model
{
    public interface IBattlerParamUnit:IDisposable,IResetable
    {
        /// <summary>
        /// ユニットの本体更新監視
        /// </summary>
        IObservable<IBattlerParamUnit> OnUnitDataObservable { get; }

        CardInfoEntity Entity { get; }

        /// <summary> ユニットの中身が存在するか否か </summary>
        bool IsExist { get; }
    }

    public class BattlerParamUnit : IBattlerParamUnit
    {
        public IObservable<IBattlerParamUnit> OnUnitDataObservable => m_UnitDataReactive.SkipLatestValueOnSubscribe();
        private readonly ReactiveProperty<IBattlerParamUnit> m_UnitDataReactive = new ReactiveProperty<IBattlerParamUnit>();

        public CardInfoEntity Entity => m_Entity;

        public bool IsExist => m_Entity != null;

        private CardInfoEntity m_Entity;

        public void Dispose()
        {
            m_UnitDataReactive.Dispose();
        }

        public void Setup(CardInfoEntity entity)
        {
            m_Entity = entity;
            m_UnitDataReactive.SetValueAndForceNotify(this);
        }

        /// <summary>
        /// ユニットデータに null を入れる
        /// </summary>
        public void Reset()
        {
            m_Entity = null;
            m_UnitDataReactive.SetValueAndForceNotify(this);
        }
    }
}
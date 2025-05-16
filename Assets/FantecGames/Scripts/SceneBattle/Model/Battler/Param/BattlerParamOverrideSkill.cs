using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace fantec.Battle.Model
{
    public class BattlerParamOverrideSkill : IBattlerParamOverrideSkill
    {
        // 保有している最大のスキル数。オーバライド～クエタオーバーライドの4種
        public const int RESEREVE_MAX_COUNT = 4;
        public IObservable<bool> OnIsReserveReactive => m_IsReserveReactive;
        public IObservable<bool> OnIsSealedReactive => m_IsSealedReactive;
        public IObservable<bool> OnIsInActivationReactive => m_IsInActivationReactive;
        public IObservable<OverrideSkillEntity> OnCutinObservable => m_CutinSubject;
        public IObservable<AffectInfo> OnActivationObservable => m_ActivationSubject;
        public IObservable<Unit> OnActionCompletedObservable => m_ActionCompletedSubject;
        public IObservable<Unit> OnDeactivationObservable => m_DeactivationSubject;
        public IObservable<Unit> OnBlurObservable => m_BlurSubject;

        private readonly ReactiveProperty<bool> m_IsReserveReactive = new ReactiveProperty<bool>();
        private readonly ReactiveProperty<bool> m_IsSealedReactive = new ReactiveProperty<bool>();
        private readonly ReactiveProperty<bool> m_IsInActivationReactive = new ReactiveProperty<bool>();
        private readonly Subject<OverrideSkillEntity> m_CutinSubject = new Subject<OverrideSkillEntity>();
        private readonly Subject<AffectInfo> m_ActivationSubject = new Subject<AffectInfo>();
        private readonly Subject<Unit> m_ActionCompletedSubject = new Subject<Unit>();
        private readonly Subject<Unit> m_DeactivationSubject = new Subject<Unit>();
        private readonly Subject<Unit> m_BlurSubject = new Subject<Unit>();

        public List<OverrideSkillEntity> EntityList => m_EntityList;
        public OverrideSkillEntity Entity => m_Entity;

        public bool IsSealed => m_IsSealedReactive.Value;
        public bool IsReserve => m_IsReserveReactive.Value;
        public bool IsActive => m_IsInActivationReactive.Value;
        public bool IsReserveable => !IsSealed && !IsReserve && !IsActive && !m_IsMute;
        public bool IsCancelable => IsReserve;


        private List<OverrideSkillEntity> m_EntityList;
        private OverrideSkillEntity m_Entity;
        private NormalAttackEntity m_NormalAttackEntity;
        private bool m_IsMute;
        public void Setup(List<OverrideSkillEntity>entityList,NormalAttackEntity normalAttackEntity)
        {
            if (entityList != null)
            {
                m_EntityList = entityList;
                m_NormalAttackEntity = normalAttackEntity;
            }
            else
            {
            }
        }

        public void Reset()
        {
            if (m_EntityList != null)
            {
                m_EntityList.Clear();
            }
            m_Entity = null;
        }

        public void Dispose()
        {
            m_BlurSubject.Dispose();
            m_CutinSubject.Dispose();
            m_IsReserveReactive.Dispose();
            m_IsSealedReactive.Dispose();
            m_IsInActivationReactive.Dispose();
        }


        public void Consume()
        {
            m_IsReserveReactive.Value = false;          // 予約状態解除

            m_CutinSubject.OnNext(m_Entity);
        }

        public void Activation(AffectInfo info)
        {
            m_IsInActivationReactive.Value = true;
            m_ActivationSubject.OnNext(info);
        }

        public void Deactivation()
        {
            m_IsMute = false;
            m_IsInActivationReactive.Value = false;
            m_DeactivationSubject.OnNext(Unit.Default);
        }


        public void SetIsReserve(bool enable)
        {
            m_IsReserveReactive.Value = enable;
        }

        public void SetIsSealed(bool enable)
        {
            m_IsSealedReactive.Value = enable;
        }

        public void SetMute(bool enable)
        {
            m_IsMute = enable;
        }

        /// <summary>
        /// オーバードライブの予約
        /// </summary>
        /// <param name="originIds"> 移動先にいるユニットの固有IDが入ってくる </param>
        public void SetEntity(int[]originIds)
        {
            m_Entity = null;
            if (m_EntityList!=null)
            {
                foreach(var entity in m_EntityList)
                {
                    // 発動に必要なリストが移動予定にいるユニットIDと完全に一致していたら、
                    // そのオーバードライブを使用する
                    var originSet = new HashSet<int>(originIds);
                    var triggerSet = new HashSet<int>(entity.triggerCardOriginId);

                    if (originSet.SetEquals(triggerSet)) // 完全一致（順不同）
                    {
                        m_Entity = entity;
                        break;
                    }
                }

                // 特定の種類のユニット同士が重ならなかった場合
                if (m_Entity==null) 
                {
                    if (originIds.Length.Equals(2))// 2枚以上重なったものが検出されたなら
                    {
                        m_Entity = m_EntityList[0];// 発動予定をオーバーライドに (作りを変える必要あり) 
                    }
                    else if(originIds.Length.Equals(3)) // カードが被らず3枚重なっていたら
                    {
                        m_Entity = m_EntityList[2]; // 発動予定をゼタに設定
                    }
                }
            }
        }

        public void ActionCompleted()
        {
            m_ActionCompletedSubject.OnNext(Unit.Default);
        }

        public void ActivateBlur()
        {
            m_BlurSubject.OnNext(Unit.Default);
        }
    }
}
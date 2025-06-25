using UnityEngine;
using UniRx;
using System;
using fantec.Battle.Utiles;

namespace fantec.Battle.Manager.Pool
{
    [RequireComponent(typeof(Homing))]
    public class IndexingHomingParticleObject : AbstractIndexingParticleObject<IndexingHomingParticleObject>
    {
        public IObservable<Unit>OnCompleted=>m_Homing.OnCompleted;

        private Homing m_Homing;

        protected override void Awake()
        {
            m_Homing = GetComponent<Homing>();

            base.Awake();
        }

        public override void Return()
        {
            Locator.Resolve<IBattlePoolManager>().Return(this);
        }

        public IndexingHomingParticleObject Setup(Vector3 start,Vector3 end)
        {
            base.SetupBase();

            // ホーミング再生
            this.m_Homing.PlayThrow(start, end);

            // 描画順を HUD より手前に
            this.m_SortingGroup.sortingLayerName = BD.SortingLayer.NAME_HUD;

            return this;
        }

        // 中間地点を設定したホーミング射出処理
        public IndexingHomingParticleObject Setup(Vector3 start,Vector3 half,Vector3 end)
        {
            base.SetupBase();

            // ホーミング再生
            this.m_Homing.PlayThrow(start, half, end);

            // 描画順を HUD より手前に
            this.m_SortingGroup.sortingLayerName= BD.SortingLayer.NAME_HUD;

            return this;
        }

        // 対象まで真っすぐにパーティクルを飛ばす処理
        public IndexingHomingParticleObject SetupStraight(Vector3 start,Vector3 end)
        {
            base.SetupBase();

            // ホーミング再生
            this.m_Homing.PlayStraight(start,end);

            // 描画順を HUD より手前に
            this.m_SortingGroup.sortingLayerName = BD.SortingLayer.NAME_HUD;

            return this;
        }
    }
}
using DG.Tweening;
using UnityEngine;

namespace fantec.Battle.Manager.Pool
{
    public abstract class AbstractIndexingParticleObject<T>:AbstractIndexingBaseObject<T> where T : Object
    {
        protected ParticleSystem[] m_Particles;

        protected override void Awake()
        {
            m_Particles=GetComponentsInChildren<ParticleSystem>();
            m_SortingGroup.sortingLayerName = BD.SortingLayer.NAME_FIELD_DEFAULT;
            m_SortingGroup.sortingOrder = (int)BD.SortingLayer.FieldOrder.Particle;
            base.Awake();
        }

        protected virtual void OnDestroy()
        {
            m_Sequence.Kill();
        }

        protected override void OnSetCurrentTimeScale(float speed)
        {
            foreach(var particle in m_Particles)
            {
                var main = particle.main;
                main.simulationSpeed=speed;
            }

            base.m_Sequence.TimeScale = speed;
        }

        public void SetSortingGroup(string layerName)
        {
            this.m_SortingGroup.sortingLayerName=layerName;
        }

        protected void SetupBase()
        {
            // サイズ調整
            this.transform.GetChild(0).localScale = Vector3.one * base.m_Size;

            // 特殊演出中に再生された場合
            if(base.m_IsDirectingPause)
            {
                // 素通りする設定に
                base.SetThroughDirectingPause(true);
                this.m_SortingGroup.sortingLayerName = BD.SortingLayer.NAME_FIELD_BLACKOUT;
            }
            else
            {
                // 設定を戻す
                base.SetThroughDirectingPause(false);
                this.m_SortingGroup.sortingLayerName = BD.SortingLayer.NAME_FIELD_DEFAULT;
            }


            // 演出再生
            base.m_Sequence.Value = DOTween.Sequence()
                .AppendInterval(m_LifeTime)
                .SetLink(this.gameObject)
                .OnComplete(() =>
                {
                    // 完了、返却
                    Return();
                });
        }
    }
}
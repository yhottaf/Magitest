using fantec.Battle.Manager.Pool;
using UnityEngine;

namespace fantec.Battle.Manager
{
    public interface IBattlePoolManager:IRegistable
    {
        IndexingSpotParticleObject Rent(PoolableSpotParticle.Index index);
        IndexingHomingParticleObject Rent(PoolableHomingParticle.Index index);
        IndexingPopNumeralObject Rent(PoolableNumeral.Index index);
        void Return(IndexingSpotParticleObject poolObject);
        void Return(IndexingHomingParticleObject poolObject);
        void Return(IndexingPopNumeralObject poolObject);
    }

    public class BattlePoolManager : MonoBehaviour,IBattlePoolManager
    {
        [Header("Parent")]
        [SerializeField] private Transform m_ScreenSpace;
        [SerializeField] private Transform m_WorldSpace;

        [Header("Pool")]
        [SerializeField] private IndexingPoolContainer<IndexingSpotParticleObject> m_SpotParticleContainer;
        [SerializeField] private IndexingPoolContainer<IndexingHomingParticleObject> m_HomingParticleContainer;
        [SerializeField] private IndexingPoolContainer<IndexingPopNumeralObject> m_PopNumeralContainer;

        public void Register()
        {
            Locator.Register<IBattlePoolManager>(this);
        }

        protected void Awake()
        {
            m_SpotParticleContainer.Initialize(m_WorldSpace);
            m_HomingParticleContainer.Initialize(m_WorldSpace);
            m_PopNumeralContainer.Initialize(m_WorldSpace);
        }

        public IndexingSpotParticleObject Rent(PoolableSpotParticle.Index index) => m_SpotParticleContainer.Rent(index.ToStringQuickly());
        public IndexingHomingParticleObject Rent(PoolableHomingParticle.Index index) => m_HomingParticleContainer.Rent(index.ToStringQuickly());
        public IndexingPopNumeralObject Rent(PoolableNumeral.Index index) => m_PopNumeralContainer.Rent(index.ToStringQuickly());

        public void Return(IndexingSpotParticleObject poolObject) => m_SpotParticleContainer.Return(poolObject);
        public void Return(IndexingHomingParticleObject poolObject) => m_HomingParticleContainer.Return(poolObject);
        public void Return(IndexingPopNumeralObject poolObject)=>m_PopNumeralContainer.Return(poolObject);
    }
}
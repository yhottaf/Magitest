using UnityEngine;

namespace fantec.Battle.Manager.Pool
{
    public class IndexingSpotParticleObject : AbstractIndexingParticleObject<IndexingSpotParticleObject>
    {
        public override void Return()
        {
            Locator.Resolve<IBattlePoolManager>().Return(this);
        }

        public IndexingSpotParticleObject Setup()
        {
            base.SetupBase();

            return this;
        }
    }
}
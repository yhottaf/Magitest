using fantec.Common;
using UnityEngine;

namespace fantec.Battle.Model
{
    public interface IBattleModelBridginData : ILocatable
    {
        BridgingData Data { get; }
    }
    public class BattleModelBridginData : MonoBehaviour, IBattleModelBridginData,IRegistable
    {
        [SerializeField] private BridgingData m_Data;

        public BridgingData Data => m_Data;

        public void Register()
        {
            Locator.Register<IBattleModelBridginData>(this);
        }

        private void OnApplicationQuit()
        {
            m_Data.ResetData();
        }
    }
}
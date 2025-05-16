using UnityEngine;
using UnityEngine.SceneManagement;

namespace fantec.Common
{
    public class BridgingDataProvider : MonoBehaviour
    {
        [SerializeField, ReadOnly] private BridgingData m_Data;

        public static BridgingData Get
        {
            get { return ExSceneManager.GetRootComponent<BridgingDataProvider>().m_Data; }
        }
        public static BridgingData GetData(Scene scene)
        {
            return ExSceneManager.GetRootComponent<BridgingDataProvider>(scene).m_Data;
        }

        public static BridgingData GetData(string sceneName)
        {
            return ExSceneManager.GetRootComponent<BridgingDataProvider>(sceneName).m_Data;
        }

        private void OnApplicationQuit()
        {
            m_Data.ResetData();
        }
    }
}
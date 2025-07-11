using System.Collections;
using UnityEngine;
using fantec.Common;

namespace fantec.Adventure
{
    public class AdventureEngineStarter : MonoBehaviour
    {
        [SerializeField] private AdvEngineStarter m_Starter;
        [SerializeField] private AdvEngine m_Engine;

        private void Awake()
        {
            StartCoroutine(LoadAsync());
        }

        private IEnumerator LoadAsync()
        {
            yield return m_Starter.LoadEngineAsync(() => m_Starter.IsLoadErrorOnAwake = true);
            Loading.Hide(1.0f, () =>
            {
                m_Engine.StartGame(BridgingDataProvider.Get.GetAdvSeetName());

                // プレイヤー名のパラメータ変更
                string playerName = PlayFabClient.PlayerProfileManager.UserDisplayName;
                if (m_Engine.Param.CheckSetParameter("user_name", playerName))
                {
                    m_Engine.Param.SetParameterString("user_name", playerName);
                }
            });
        }
    }
}

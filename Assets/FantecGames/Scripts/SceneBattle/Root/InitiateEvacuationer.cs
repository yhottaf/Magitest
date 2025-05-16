using UnityEngine;
using System.Threading;
using fantec.Common;
using Cysharp.Threading.Tasks;

namespace fantec.Battle.Root
{
    public class InitiateEvacuationer : MonoBehaviour,IRootInitiater
    {
        [SerializeField] BridgingData m_bridgingData;
        [SerializeField] SceneIndex m_SceneIndex;
        public async UniTask InitializeAsync(CancellationTokenSource cts)
        {
            if(string.IsNullOrEmpty(m_bridgingData.GetStageData().stageName))
            {
                ExSceneManager.Instance.LoadScene(m_SceneIndex);
                cts.Cancel();
            }

            await UniTask.CompletedTask;
        }
    }
}
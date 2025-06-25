using Cysharp.Threading.Tasks;
using fantec.Common;
using UnityEngine;

namespace fantec.Debugger.BattleSetup
{
    [DefaultExecutionOrder(-1)]
    public class DebugBattleSetup : MonoBehaviour
    {
        [SerializeField] private GameObject m_UiRoot;

        private void Awake()
        {
            UniTask.Void(async () =>
            {
                m_UiRoot.SetActive(false);
                await MasterDataManager.Instance.LoadMasterDataForLocalAsync(this.GetCancellationTokenOnDestroy());
                m_UiRoot.SetActive(true);
                Fade.FadeIn(0.5f);
            });
        }
    }
}
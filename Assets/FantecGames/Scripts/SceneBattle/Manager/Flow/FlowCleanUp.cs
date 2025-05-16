using Cysharp.Threading.Tasks;
using fantec.Common;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace fantec.Battle.Manager
{
    public partial class BattleFlowManager
    {
        public class FlowCleanUp : FlowBase
        {
            public override void OnEnter(BattleFlowManager manager, FlowBase prevFlow)
            {
                UniTask.Void(async () =>
                {
                    Loading.Show(0.5f);
                    await Locator.Resolve<IBattleSoundManager>().UnloadSeAsync(manager.m_OnDestroyCancellationToken);

                    Locator.Resolve<IBattleSoundManager>().StopBgm();

                    if(PlayFabClient.UserDataManager.User!=null)
                    {
                        // ユーザーが存在していたらメニュー画面に戻るようにする
                        await ExSceneManager.Instance.LoadSceneAsync(SceneIndex.MENU, LoadSceneMode.Single);
                    }
                    else
                    {
                        // ユーザーが存在していなかったらデバッグモードのバトルのセットアップ画面に戻る
                        await ExSceneManager.Instance.LoadSceneAsync(SceneIndex.DEBUG_BATTLE_SETUP, LoadSceneMode.Single);
                    }

                    Modal.Clear();     // モーダルを全て閉じる
                    Locator.Dispose(); // ロケート中のものをDispose() メモリ解放
                    Locator.Clear();   // ロケーターをクリア

                    Loading.Hide(0.5f);
                });
            }
        }
    }
}
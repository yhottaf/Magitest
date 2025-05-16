using fantec.Common;
using fantec.Menu.Manager;
using fantec.PlayFabClient;
using UnityEngine;
using UnityEngine.UI;

namespace fantec.Menu.Home.Presenter
{
    public class HomePresenter : MonoBehaviour
    {
        [SerializeField]
        private Image m_Background;
        private async void Start()
        {
            try
            {
            // ホーム画面のBGM再生
                await BGMManager.Instance.PlayAsync(BGMManager.Type.Home, true);


                // 現在ロード中のアドレス一覧を確認(デバッグ)
                var loading = AssetManager.Instance.GetLoadedAudioClips();
                foreach (var addr in loading)
                {
                    Debug.Log($"現在キャッシュに残っているAudio: {addr}");
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"BGM 再生失敗: {ex.Message}");
            }

            //名前が未入力の場合モーダル表示
            if (string.IsNullOrEmpty(PlayerProfileManager.UserDisplayName))
            {
                MenuWindowManager.Instance.Create(MenuWindowManager.CreateType.PlayerNameEdit);
            }
            else // 仮で入力済ならお知らせ表示させようとする
            {
                MenuWindowManager.Instance.Create(MenuWindowManager.CreateType.Notice);
            }
        }
    }
}
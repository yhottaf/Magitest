using Cysharp.Threading.Tasks;
using fantec.Common;
using fantec.Master;
using fantec.PlayfabCilent;
using fantec.PlayFabClient;
using PlayFab;
using PlayFab.ClientModels;
using Spine.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UniRx;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace fantec.Title
{
    public class TitlePresenter : MonoBehaviour
    {
        [SerializeField]
        private TitleView m_View;

        private List<string> AllVersions = new List<string>();

        /// <summary>
        /// タップを受け付けるかどうか
        /// </summary>
        private bool isTapable = true;

        private bool isCanSceneChange = false;

        // アセットのロード確認から何秒でフェードが始まるか(2秒指定)
        private readonly int AwaitSecond = 2000; 

        void Start()
        {
            Fade.FadeIn(0.5f);
            m_View.OnClickTapButtonObservable.Where(_ => isTapable).Subscribe(OnClickTapButton).AddTo(this);                      //全体画面をタップしたとき

            //m_View.OnClickRetryButtonObservable
            //    .Subscribe(async _ =>
            //    {
            //        try
            //        {
            //            await DownLoadInitialAsset();
            //        }
            //        catch (Exception ex)
            //        {
            //            Debug.LogError($"RetryボタンでのDL失敗: {ex.Message}");
            //            m_View.ShowRetryButton();
            //        }
            //    }).AddTo(this);

            // m_View.OnClickDeleteUserDataBtnObservable.Where(_=>isTapable).Subscribe(OnClickDeleteUserData).AddTo(this);           // Test:ユーザーデータの削除ボタン
            string versionStr = string.Empty;
#if UNITY_EDITOR
            versionStr = $"Ver.{PlayerSettings.bundleVersion} ({System.Environment.OSVersion.VersionString})";
#elif UNITY_ANDROID
            versionStr = $"Ver.{Application.version} ({AndroidVersion.GetVersionCode()})";
#elif UNITY_IOS
            versionStr = $"Ver.{Application.version} ({IOSVersion.GetBuildNumber()})";
#endif

            m_View.UpdateVersionText(versionStr);

            m_View.UpdatePlayerIdText("ID: " + PlayerPrefsManager.UserId);



            // BGM再生関数をここに呼び出す タイトル画面のBGM
            BGMManager.Instance.Play(m_View.m_audiosource, true);
        }

        /// <summary>
        /// タップエリア押下時
        /// </summary>
        /// <param name="unit"></param>
        private void OnClickTapButton(Unit unit)
        {
            // SE再生
            SEManager.Instance.Play(SEClipName.SystemLoginPop);

            Login().Forget();
        }

        /// <summary>
        /// ユーザーデータの削除機能の呼び出し
        /// </summary>
        /// <param name="unit"></param>
        private void OnClickDeleteUserData(Unit unit)
        {
            UserDataDelete().Forget();
        }

        /// <summary>
        /// ログイン処理
        /// </summary>
        /// <returns></returns>
        private async UniTask Login()
        {
            isTapable = false;

            //ここにマスターデータの読み込み処理を書く
            await MasterDataManager.Instance.LoadMasterDataForLocalAsync(new CancellationToken());

            //PlayFabログイン
            Debug.Log("PlayFabログイン開始");
            await LoginManager.LoginAndUpdateLocalCacheAsync();
            Debug.Log("PlayFabログイン完了");

            // TODO デバッグでローカルに保存されているリソースデータを削除したいときコメントアウトを外す
            //PlayerPrefsManager.AssetBundleVersion = "";
            //bool success = Caching.ClearCache();
            //Debug.Log(success ? "キャッシュ削除成功" : "キャッシュ削除失敗");


            try //バージョン確認と、それに応じダウンロードする必要があるリソースデータがあるならダウンロード処理
            {
                AllVersions = await FetchAllVersionsFromPlayFab();
                Debug.Log("取得したバージョン: " + string.Join(", ", AllVersions));

                await DownLoadInitialAsset();
            }
            catch (Exception ex)
            {
                Debug.LogError("バージョン情報取得エラー : " + ex.Message);
            }

            await UniTask.Delay(AwaitSecond);// 2秒待つ

            if (isCanSceneChange)// 更新データの確認が終わり次第シーン遷移開始
            {
                Loading.Show();

                PlayFabSettings.staticSettings.TitleId = "E23B7";
                PlayFabSettings.staticSettings.DeveloperSecretKey = "KXYWIP4D4FG5SOTJ8WCJW1FX8UBK3H69FYWP5UICQG8F3SQ8RC";
                UserDataManager.User.TutorialDictionary[TutorialId.InitialPresent] = false;
                //初回ログインの場合
                if (UserDataManager.User.TutorialDictionary[TutorialId.InitialPresent] == false)
                {
                    await StoreManager.PurchaseItemAsync(StoreId.DummyStore, "Initial-Present", VirtualCurrencyNames.FS.Code);
                    await UserDataManager.User.UpdateTutorialFlag(TutorialId.InitialPresent);
                    await StoreManager.PurchaseItemAsync(StoreId.DummyStore, "MagiDarkPresent", VirtualCurrencyNames.FS.Code);
                }

                // Spineのスケルトンデータの読み込みは重いのでここで事前に全て読み込んでキャッシュに保存しておく
                await LoadSkeletonData();
                await LoadLive2DData();

                ExSceneManager.Instance.LoadScene(SceneIndex.MENU);
            }
        }

        private async UniTask UserDataDelete()
        {
            await UserDataManager.DeleteUserData();
        }

        // 外部サーバーからリソースをダウンロードする (ローカルからのダウンロードも可)
        private async UniTask DownLoadInitialAsset(bool isRetry = false)
        {
            string installedVer = PlayerPrefsManager.AssetBundleVersion;

            List<string> needDownloadLabels = AllVersions
                .Where(v => string.Compare(v, installedVer) > 0)
                .Select(v => $"InitialAssets-{v}")
                .ToList();

            if (needDownloadLabels.Count == 0)
            {
                Debug.Log("ダウンロード対象のアセットはありません");
                isCanSceneChange = true;
                return;
            }
            Debug.Log(isRetry ? "再試行モード: キャッシュを削除して再ダウンロード中..." : "初期アセットのダウンロード開始");
            // ダウンロードするリソースが存在するなら、進捗バーの表示
            m_View.SetLoadingViewObj(true);

            bool allSucceeded = true;
            long totalBytes = 0;
            long downloadedBytes = 0;
            List<long> labelSizes = new();

            // まず全ラベルのサイズを取得
            foreach (var label in needDownloadLabels)
            {
                Debug.Log($"試すラベル: {label}");
                var sizeHandle = Addressables.GetDownloadSizeAsync(label);
                await sizeHandle.ToUniTask();

                if (sizeHandle.Status == AsyncOperationStatus.Succeeded)
                {
                    labelSizes.Add(sizeHandle.Result);
                    totalBytes += sizeHandle.Result;
                }
                else
                {
                    Debug.LogError($"{label} のダウンロードサイズ取得に失敗しました");
                    allSucceeded = false;
                    return;
                }

            }

            // バックグラウンドでもダウンロードが行われるようにする
            Application.runInBackground = true; 

            for (int i = 0; i < needDownloadLabels.Count; i++)
            {
                string label = needDownloadLabels[i];
                long labelSize = labelSizes[i];

                if (labelSize == 0)
                {
                    Debug.Log($"{label} はキャッシュ済みです");
                    downloadedBytes += labelSize;
                    continue;
                }

                try
                {
                    var downloadHandle = Addressables.DownloadDependenciesAsync(label, true);

                    // プログレス表示ループ
                    while (!downloadHandle.IsDone)
                    {
                        float labelProgress = downloadHandle.PercentComplete;
                        long currentDownloaded = downloadedBytes + (long)(labelProgress * labelSize);
                        float progressRate = totalBytes > 0 ? (float)currentDownloaded / totalBytes : 1f;
                        float currentMB = currentDownloaded / (1024f * 1024f);
                        float totalMB = totalBytes / (1024f * 1024f);

                        m_View.UpdateProgressText(progressRate, $"{currentMB:F2} MB / {totalMB:F2} MB ({progressRate * 100:F1}%)");

                        await UniTask.Yield();
                        // エラーチェック (ループ中に失敗が発生した場合も対応)
                        if (progressRate != 1.0f)
                        {
                            if (downloadHandle.Status == AsyncOperationStatus.Failed)
                            {
                                Debug.LogError($"ダウンロード中にエラーが発生しました: {label}");
                                isCanSceneChange = false;
                                throw new Exception($"ダウンロードに失敗しました: {label} - {downloadHandle.OperationException?.Message}");
                            }
                        }
                    }

                    Debug.Log($"{label} のダウンロード完了！");
                }
                catch (Exception ex)
                {
                    Debug.LogError($"ダウンロード中に例外発生: {label} → {ex.Message}");

                    // RemoteProviderException の場合、isCanSceneChange を false にする
                    if (ex.Message.Contains("RemoteProviderException") || ex.Message.Contains("ConnectionError"))
                    {
                        Debug.LogError("リモートプロバイダへの接続に失敗しました。シーン遷移を許可しません。");
                        isCanSceneChange = false;
                    }

                    allSucceeded = false;
                    break;
                }
            }

            // ダウンロード終了時にバックグラウンド動作を無効にする　
            Application.runInBackground = false; 

            if (allSucceeded)
            {
                // 成功時のみ現在のアプリのバージョンを端末に保存しておく
                PlayerPrefsManager.AssetBundleVersion = Application.version;
                Debug.Log("初期アセットのチェック完了。ログイン処理開始");
                isCanSceneChange = true;

            }
            else if (!isRetry)
            {
                Debug.LogWarning("初回ダウンロード失敗 → キャッシュ削除して再試行します");

                bool cleared = Caching.ClearCache();
                PlayerPrefsManager.AssetBundleVersion = "";
                Debug.Log(cleared ? "キャッシュ削除成功" : "キャッシュ削除失敗");

                await DownLoadInitialAsset(isRetry: true);
            }
            else
            {
                Debug.LogError("キャッシュを削除してもダウンロード失敗。手動リトライを許可");
                // TODO: 手動でリトライを開始するボタンの表示
                //     m_View.ShowRetryButton();
            }
        }

        // サーバーからアプリの現在配信済みバージョン情報を取得する
        // この情報を元にAssetBandleの読み込みを行う。 →例えば1.1.0まで配信していた場合、1.0.0～1.1.0分のリソースを取得させる
        public static async UniTask<List<string>> FetchAllVersionsFromPlayFab()
        {
            try
            {
                var response = await PlayFabClientAPI.GetTitleDataAsync(new GetTitleDataRequest());

                if (response.Result.Data != null &&
                    response.Result.Data.TryGetValue("AssetVersions", out string versionCsv))
                {
                    List<string> versions = versionCsv
                        .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(v => v.Trim())
                        .ToList();

                    return versions;
                }
                else
                {
                    throw new Exception("TitleData に 'AssetVersions' キーが存在しません");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"PlayFab TitleData取得失敗: {ex.Message}");
                throw;
            }
        }

        // 最新のアプリバージョンであるかどうかを調べ
        // 更新データがあればAppStoreかGooglePlayConsoleへのリンクを表示させる
        private async UniTask VersionChecker()
        {
            string currentVersion = string.Empty;
#if UNITY_EDITOR
            currentVersion =PlayerSettings.bundleVersion;
#elif UNITY_ANDROID||UNITY_IOS
            currentVersion = Application.version;
#endif

            // 現在のバージョンより新しいものがあるかどうかをチェック
            var NewVersion = AllVersions
                .Where(v => string.Compare(v, currentVersion) > 0)
                .ToList();

            if (NewVersion.Count > 0)
            {
                Debug.Log("新しいバージョンがあります: " + string.Join(",", NewVersion));
                ShowUpdateLink();
            }
            else
            {
                Debug.Log("最新のバージョンです");
            }
        }

        private void ShowUpdateLink()
        {
            m_View.SetUpdateViewObj(true); // アップデートを促すUIの表示

#if UNITY_ANDROID
            m_View.OnClickUpdateButtonObservable.Subscribe(_ =>Application.OpenURL(m_View.GooglePlayUrl)).AddTo(this);
#elif UNITY_IOS
         m_View.OnClickUpdateButtonObservable.Subscribe(_ =>Application.OpenURL(m_View.AppStoreUrl)).AddTo(this);
#endif
        }

        /// <summary>
        /// プレイヤーの手持ちのSpineのスケルトンデータの読み込み
        /// </summary>
        /// <returns></returns>
        private async UniTask LoadSkeletonData()
        {
            try
            {
                for (int i = 0; i < Define.PARTY_NUM; i++)
                {
                    PartyData partyData = UserDataManager.PartyList[i];

                    // 一時保存用
                    for (int j = 0; j < partyData.MemberList.Count; j++)
                    {
                        int cardId = partyData.MemberList[j];

                        if (cardId != -1)
                        {
                            CardData carddata = CardManager.GetCardData(cardId);
                            int originId = carddata.CardMasterData().originId;

                            string address = AssetPath.GetCharacterSpinePath(originId);
                            await AssetManager.Instance.LoadAssetAsync<SkeletonDataAsset>(address, CancellationToken.None);
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"スケルトンデータの読み込み中に例外が発生しました{ex}");
            }
        }

        /// <summary>
        /// Live2DDataのロード
        /// </summary>
        /// <returns></returns>
        private async UniTask LoadLive2DData()
        {
            CardData carddata = CardManager.GetCardData(UserDataManager.User.ProfileCardId);
            int originId = carddata.CardMasterData().originId;

            // モデルのロード
            await AssetManager.Instance.LoadAssetAsync<GameObject>(AssetPath.GetLive2DPath(originId));
        }
    }
}
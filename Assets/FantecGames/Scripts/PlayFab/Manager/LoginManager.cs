using Cysharp.Threading.Tasks;
using fantec.PlayFabClient;
using PlayFab;
using PlayFab.ClientModels;
using System;

namespace fantec.PlayfabCilent
{
    /// <summary>
    /// PlayFabへのログインを管理する
    /// </summary>
    public class LoginManager
    {
        /// <summary>
        /// ここでログインと同時に取得する情報の設定をする
        /// </summary>
        public static GetPlayerCombinedInfoRequestParams CombinedInfoRequestparams { get; }
        = new GetPlayerCombinedInfoRequestParams
        {
            GetUserAccountInfo = true,
            GetPlayerProfile = true,
            GetTitleData = true,
            GetUserData=true,
            GetUserInventory = true,
            GetUserVirtualCurrency = true,
            GetPlayerStatistics = true,
        };

        /// <summary>
        /// コンストラクタ
        /// </summary>
        static LoginManager()
        {
            //TODO:環境先切替の実装
            //PlayFabSettings.staticSettings.TitleId = "";  // 移行前開発
            PlayFabSettings.staticSettings.TitleId = "E23B7";  // 開発
            //PlayFabSettings.staticSettings.TitleId = "";  // ステージング
            //PlayFabSettings.staticSettings.TitleId = "";  // 本番
        }

        /// <summary>
        /// ユーザーデータとタイトルデータを初期化する。
        /// </summary>
        /// <returns></returns>
        public static async UniTask LoginAndUpdateLocalCacheAsync()
        {
            //UserIdがなければユーザーを新規作成し、UserIdがあれば既存ユーザーでログインする
            var userId = PlayerPrefsManager.UserId;
            var loginResult = string.IsNullOrEmpty(userId)
                ? await CreateNewUserAsync()
                : await LoadUserAsync(userId);

            await UpdateLocalCacheAsync(loginResult);
        }


        /// <summary>
        /// ログイン時に取得したデータをキャッシュする
        /// </summary>
        /// <param name="loginResult"></param>
        /// <returns></returns>
        public static async UniTask UpdateLocalCacheAsync(LoginResult loginResult)
        {
            //カタログは他のインスタンスの初期化にも必要なので最初に行うこと
            await UniTask.WhenAll(
                CatalogManager.SyncPlayFabToClientAsync(),
                StoreManager.SyncPlayFabToClientAsync()
                );

            //プレイヤープロフィール、手持ちカード、インベントリーなどの更新もここに追記
            PlayerProfileManager.SyncPlayFabToClient(loginResult.InfoResultPayload.PlayerProfile, loginResult.InfoResultPayload.PlayerStatistics);
            CardManager.SyncPlayFabToClient(loginResult.InfoResultPayload.UserInventory);
            InventoryManager.SyncPlayFabToClient(loginResult.InfoResultPayload.UserInventory);
            VirtualCurrencyManager.SyncPlayFabToClient(loginResult.InfoResultPayload.UserVirtualCurrency);
            UserDataManager.SyncPlayFabToClient(loginResult.InfoResultPayload.UserData);
            
            //ユーザー情報の更新
            await UserDataManager.UpdatePlayFab();
            //ログインボーナス獲得処理
        }

        /// <summary>
        /// 最終ログイン日時から日付が変わっていればログインボーナスを獲得する
        /// </summary>
        /// <param name="loginResult"></param>
        /// <returns></returns>
        private static async UniTask CheckAndAddLoginBonusAsync(LoginResult loginResult)
        {
            var loginDateTime = loginResult.InfoResultPayload.AccountInfo.TitleInfo.LastLogin;
            var lastLoginDataTime = loginResult.LastLoginTime;

            if(loginDateTime is null || lastLoginDataTime is null)
            {
                return;
            }

            var loginDate = (loginDateTime + TimeSpan.FromHours(9))?.Date;
            var lastlLoginDate = (lastLoginDataTime + TimeSpan.FromHours(9))?.Date;

            if(loginDate==lastlLoginDate)
            {
                return;
            }

            // TODO: ログインボーナスをプレゼントボックスに付与する処理などを書く

            // PlayerPrefsにログインボーナスを獲得したことを記録しておく
            PlayerPrefsManager.HasLoginBonus = true;
        }

        private static async UniTask<LoginResult>CreateNewUserAsync()
        {
            while(true)
            {
                // UserIdを採番する
                // PlayFabのCunstomIdとして使うならGuid.NewGuid().ToString()で十分
                // ただし今回はこれをメールアドレス連携するときのUserIdにも使いまわしたいため、記号は使用せず、文字数を20文字以内にしておく
                string newUserId = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 20);

                //ログインリクエストを作成する
                LoginWithCustomIDRequest request = new LoginWithCustomIDRequest
                {
                    CustomId = newUserId,
                    CreateAccount = true,
                    InfoRequestParameters = CombinedInfoRequestparams
                };

                //ログインする
                var response = await PlayFabClientAPI.LoginWithCustomIDAsync(request);
                if (response.Error!=null)
                {
                    throw new PlayFabErrorException(response.Error);
                }

                //もし LastLoginTime に値が入っている場合は採番した ID が既存ユーザーと重複しているのでリトライする
                if(response.Result.LastLoginTime.HasValue)
                {
                    continue;
                }

                // PlayerPrefsにUserIdを記録する
                PlayerPrefsManager.UserId = newUserId;

                return response.Result;
            }
        }

        /// <summary>
        /// ログインしてユーザーデータをロードする
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        private static async UniTask<LoginResult>LoadUserAsync(string userId)
        {
            //ログインリクエストを作成する
            LoginWithCustomIDRequest request = new LoginWithCustomIDRequest
            {
                CustomId = userId,
                CreateAccount = false,
                InfoRequestParameters = CombinedInfoRequestparams
            };

            //ログインする
            var response = await PlayFabClientAPI.LoginWithCustomIDAsync(request);
            if(response.Error!=null)
            {
                throw new PlayFabErrorException(response.Error);
            }

            return response.Result;
        }


    }
}
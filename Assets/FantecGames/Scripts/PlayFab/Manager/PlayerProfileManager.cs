using Cysharp.Threading.Tasks;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;
using System.Linq;

namespace fantec.PlayFabClient
{
    /// <summary>
    /// PlayerProfileを管理する
    /// </summary>
    public static class PlayerProfileManager
    {
        /// <summary>
        /// PlayFabId.
        /// </summary>
        /// <remarks>フレンドコードとしても使用する</remarks>
        public static string PlayFabId=>Profile.PlayerId;

        /// <summary>
        /// ユーザー名
        /// </summary>
        public static string UserDisplayName => Profile.DisplayName != null ? Profile.DisplayName : string.Empty;

        /// <summary>
        /// 統計情報
        /// </summary>
        public static List<StatisticValue> Statistics { get; private set; }

        /// <summary>
        /// タグ
        /// </summary>
        public static List<TagModel> Tags => Profile.Tags;

        /// <summary>
        /// プレイヤープロフィール
        /// </summary>
        private static PlayerProfileModel Profile { get; set; }

        /// <summary>
        /// PlayFabからClientへデータを同期する。
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="statistics"></param>
        public static void SyncPlayFabToClient(PlayerProfileModel profile,List<StatisticValue>statistics)
        {
            //初回ログイン時は null なので new しておく
            Profile = profile ?? new PlayerProfileModel();
            Statistics=statistics; 
        }

        /// <summary>
        /// ユーザー名を更新する
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static async UniTask<(bool isSuccess,string errorMessage)>UpdateUserDisplayNameAsync(string name)
        {
            var request = new UpdateUserTitleDisplayNameRequest
            { 
                DisplayName = name
            };

            var response = await PlayFabClientAPI.UpdateUserTitleDisplayNameAsync(request);
            if (response.Error!=null)
            {
                // ドキュメントを参考にエラーハンドリングする。
                // https://docs.microsoft.com/en-us/rest/api/playfab/client/account-management/updateusertitledisplayname?view=playfab-rest
                switch (response.Error.Error)
                {
                    case PlayFabErrorCode.InvalidParams:
                        return (false, "名前は3～12文字以内で入力してください。");
                    case PlayFabErrorCode.ProfaneDisplayName:
                    case PlayFabErrorCode.NameNotAvailable:
                        return (false, "この名前は使用できません。");

                    //想定外のエラーなので例外として処理する。
                    default:
                        throw new PlayFabErrorException(response.Error);
                }
            }

            //ローカルのデータを更新する。
            Profile.DisplayName = name;

            return (true,string.Empty);
        }

        /// <summary>
        /// ユーザーレベルを更新する
        /// </summary>
        /// <param name="afterLevel"></param>
        /// <returns></returns>
        public static async UniTask UpdateUserLevelAysnc(int afterLevel)
        {
            await SyncClientToPlayFabAsync(("Level",afterLevel));
        }

        /// <summary>
        /// 統計情報を更新する
        /// </summary>
        /// <returns></returns>
        private static async UniTask SyncClientToPlayFabAsync(params(string name,int value)[] statistics)
        {
            var request = new UpdatePlayerStatisticsRequest
            {
                Statistics = statistics.Select(x => new StatisticUpdate
                {
                    StatisticName = x.name,
                    Value = x.value
                }).ToList()
            };

            var response=await PlayFabClientAPI.UpdatePlayerStatisticsAsync(request);
            if (response.Error!=null)
            {
                throw new PlayFabErrorException(response.Error);
            }
        }
    }
}
using Cysharp.Threading.Tasks;
using fantec.Common;
using System.Collections.Generic;
using Newtonsoft.Json;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

namespace fantec.PlayFabClient
{
    public static class UserDataManager
    {
        public static int Level=>CurrentLevelInfo.level;
        public static int MaxStamina => CurrentLevelInfo.stamina;

        //パーティー情報などのリストなどがあればここに記載しておく
        public static List<PartyData> PartyList => User.PartyList;

        private static Master.UserRankData CurrentLevelInfo { get; set; }
        public static Master.UserRankData NextLevelInfo { get; private set; }

        public static User User { get; set; }

        /// <summary>
        /// PlayFabから最新のデータを取得してローカルにキャッシュする
        /// </summary>
        /// <param name="userData"></param>
        public  static void SyncPlayFabToClient(Dictionary<string,UserDataRecord>userData)
        {
            int exp = VirtualCurrencyManager.Exp;
            CurrentLevelInfo = MasterDataManager.Instance.UserRankMaster.GetDataByExp(exp);
            NextLevelInfo = MasterDataManager.Instance.UserRankMaster.GetDataByLevel(CurrentLevelInfo.level + 1);

            User = userData.TryGetValue("User", out var user)
                ? JsonConvert.DeserializeObject<User>(user.Value)
                : User.Create();
            if(User.PartyList==null)
            {
                User.PartyList = new List<PartyData>()
                { 
                    new PartyData(new List<int>(Define.PARTY_CAPACITY)),
                    new PartyData(new List<int>(Define.PARTY_CAPACITY)),
                    new PartyData(new List<int>(Define.PARTY_CAPACITY)),
                    new PartyData(new List<int>(Define.PARTY_CAPACITY)),
                    new PartyData(new List<int>(Define.PARTY_CAPACITY)),
                };
                // ※パーティにいれるための初期キャラ3体をあらかじめ設定しておく
                List<int> cardIds = new List<int>() { 10001100, 10001101, 10001102 };


                // プロフィールカードの設定
                User.ProfileCardId = cardIds[0];

                // お気に入りカードにも設定 (この子をトップ画面にLive2Dで出すようにする ) 
                LocalDataManager.Instance.LocalData.AddFavoriteCard(cardIds[0]);

                // 初期パーティの設定
                for (int i = 0; i < User.PartyList.Count; i++) // 編成枠全てを同じ編成で埋める
                {
                    for (int k = 0; k < cardIds.Count; k++)
                    {
                        User.PartyList[i].MemberList.Add(cardIds[k]);
                    }
                }
            }
            if(User.ClearQuestIdList==null)
            {
                User.ClearQuestIdList=new List<int>();
            }

            if(User.ClaimedNoticeDictionary==null)
            {
                User.ClaimedNoticeDictionary = new Dictionary<string, bool>();
            }
        }

        /// <summary>
        /// PlayFabのユーザーデータを更新する。
        /// </summary>
        /// <returns></returns>
        public static async UniTask<(bool isSuccess, string errorMessage)>UpdatePlayFab()
        {
            string userJson=JsonConvert.SerializeObject(User);
            var request = new UpdateUserDataRequest { Data = new Dictionary<string, string> { { "User", userJson } } };

            var response = await PlayFabClientAPI.UpdateUserDataAsync(request);
            if(response.Error!=null)
            {
                throw new PlayFabErrorException(response.Error);
            }

            return (true, string.Empty);
        }

        /// <summary>
        /// クリア済みのクエストIDを追加する
        /// </summary>
        /// <param name="questId"></param>
        public static void AddClearQuestId(int questId)
        {
            if(User.ClearQuestIdList.Contains(questId))
            {
                return;
            }

            User.ClearQuestIdList.Add(questId);
        }

        /// <summary>
        /// クエストのクリア状態を返します。
        /// </summary>
        /// <param name="questId"></param>
        /// <returns></returns>
        public static bool IsQuestClear(int questId)
        {
            return User.ClearQuestIdList.Contains(questId);
        }

        /// <summary>
        /// ゲームのユーザーデータを削除する
        /// </summary>
        /// <returns></returns>
        public static async UniTask<(bool isSuccess, string errorMessage)> DeleteUserData()
        {
            var request = new UpdateUserDataRequest { Data = new Dictionary<string, string> { { "User", null } } };

            var response = await PlayFabClientAPI.UpdateUserDataAsync(request);
            if (response.Error != null)
            {
                throw new PlayFabErrorException(response.Error);
            }
            PlayerPrefs.DeleteAll();
            return (true, string.Empty);
        }
    }
}
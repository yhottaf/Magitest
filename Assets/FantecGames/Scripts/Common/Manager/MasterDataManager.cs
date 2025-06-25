using Cysharp.Threading.Tasks;
using fantec.Common.Master;
using fantec.Master;
using fantec.Utilities;
using Newtonsoft.Json;
using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace fantec.Common
{
    public class MasterDataManager : PersistentSingleton<MasterDataManager>
    {
        public StageMaster StageMaster { get; private set; }
        public PlayerCardMaster PlayerCardMaster { get; private set; }
        public EnemyCardMaster EnemyCardMaster { get; private set; }
        public OverrideSkillMaster OverrideSkillMaster { get; private set; }
        public AdventSkillMaster AdventSkillMaster { get; private set; }
        public NoticeMaster NoticeMaster { get; private set; }
        public OtherSkillMaster OtherSkillMaster { get; private set; }
        public ConsumeItemMaster ConsumeItemMaster { get; private set; }
        public UserRankMaster UserRankMaster { get; private set; }
        public InitialRewardStageMaster InitialRewardStageMaster { get; private set; }
        public TestSkillMaster TestSkillMaster { get; private set; }

        #region TODO:サーバーで参照するようにするかも？
        public RewardStageMaster RewardStageMaster { get; private set; }
        #endregion

        private List<WaveMaster> m_WaveMasterList = new List<WaveMaster>();
        private List<ExpMaster> m_ExpMasterList = new List<ExpMaster>();
        private List<GrowthMaster> m_GrowthMasterList = new List<GrowthMaster>();

        /// <summary>
        /// カタログのリストを保存
        /// </summary>
        [NonSerialized]
        public PlayFabResult<GetCatalogItemsResult> Catalogs;

        public T GetMaster<T>(string masterName) where T : class
        {
            try
            {
                if (typeof(T) == typeof(WaveMaster)) return m_WaveMasterList.Where(master => master.name == masterName).First() as T;
                if (typeof(T) == typeof(ExpMaster)) return m_ExpMasterList.Where(master => master.name == masterName).First() as T;
                if (typeof(T) == typeof(GrowthMaster)) return m_GrowthMasterList.Where(master => master.name == masterName).First() as T;
                throw new System.Exception();
            }
            catch
            {
                throw new System.InvalidOperationException($"{masterName} が見つかりません。");
            }
        }

        /// <summary>
        /// ローカルでマスターデータを読み込む
        /// </summary>
        public async UniTask LoadMasterDataForLocalAsync(CancellationToken cts)
        {
            await ExSceneManager.Instance.LoadSceneAsync(SceneIndex.DEBUG_LOCAL_MASTER, UnityEngine.SceneManagement.LoadSceneMode.Additive);

            var provider = ExSceneManager.GetRootComponent<LocalMasterProvider>(SceneIndex.DEBUG_LOCAL_MASTER);

            StageMaster = provider.StageMaster;
            PlayerCardMaster = provider.PlayerCardMaster;
            EnemyCardMaster = provider.EnemyCardMaster;
            NoticeMaster=provider.NoticeMaster;
            OverrideSkillMaster = provider.OverrideSkillMaster;
            AdventSkillMaster = provider.AdventSkillMaster;
            OtherSkillMaster = provider.OtherSkillMaster;
            ConsumeItemMaster = provider.ConsumeItemMaster;
            UserRankMaster = provider.UserRankMaster;
            InitialRewardStageMaster = provider.InitialRewardMaster;
            TestSkillMaster = provider.TestSkillMaster;
            #region TODO:サーバーでデータを参照するようにするかも？
            RewardStageMaster = provider.RewardStageMaster;
            #endregion

            m_WaveMasterList = provider.WaveMasterList;
            m_ExpMasterList = provider.ExpMasterList;
            m_GrowthMasterList = provider.GrowthMasterList;


            await ExSceneManager.Instance.UnloadSceneAsync(SceneIndex.DEBUG_LOCAL_MASTER);
        }

        /// <summary>
        /// サーバーからマスターデータを読み込む
        /// </summary>
        /// <param name="titleData"></param>
        public void LoadMasterDataForServer(Dictionary<string, string> titleData)
        {
            PlayerCardMaster.dataList = ConvertTitleDataToDataList<PlayerCardData>(titleData, "PlayerCardMaster");
            EnemyCardMaster.dataList = ConvertTitleDataToDataList<EnemyCardData>(titleData, "EnemyCardMaster");
        }

        private List<T> ConvertTitleDataToDataList<T>(Dictionary<string, string> titleData, string dataName)
        {
            if (!titleData.TryGetValue(dataName, out var json))
            {
                Debug.LogWarning($"TitleData に '{dataName}' が存在しません。");
                return new List<T>();
            }

            try
            {
                NoticeMaster.dataList.Clear();
                return JsonConvert.DeserializeObject<List<T>>(json); // ← これだけでOK
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"JSONのデシリアライズ失敗: {ex.Message}");
                return new List<T>();
            }
        }

        public async UniTask LoadNoticeMasterDataFromServerAsync()
        {
            var titleData = await LoadTitleDataAsync();

            if (titleData == null)
            {
                Debug.LogError("TitleDataの取得に失敗したため、NoticeMasterは更新されません。");
                return;
            }

            // TitleData をデシリアライズ
            var rawList = ConvertTitleDataToDataList<NoticeData>(titleData, "NoticeMaster");

            // ScheduledStartDate が null または 空でないものだけを抽出 ※掲載日があるものだけをお知らせに表示する
            NoticeMaster.dataList = rawList
                .Where(n => !string.IsNullOrEmpty(n.ScheduledStartDate))
                .ToList();
        }

        private async UniTask<Dictionary<string, string>> LoadTitleDataAsync()
        {
            var result = await PlayFabClientAPI.GetTitleDataAsync(new GetTitleDataRequest());

            if (result.Error != null)
            {
                Debug.LogError("TitleData取得失敗: " + result.Error.GenerateErrorReport());
                return null;
            }

            Debug.Log("TitleData取得成功");

            if (result.Result.Data.TryGetValue("NoticeMaster", out var json))
            {
                Debug.Log("NoticeMasterデータ: " + json);
            }

            return result.Result.Data;
        }
    }
}
using Cysharp.Threading.Tasks;
using fantec.Common.Master;
using fantec.Master;
using fantec.Utilities;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace fantec.Common
{
    public class MasterDataManager : PersistentSingleton<MasterDataManager>
    {
        public StageMaster StageMaster { get; private set; }
        public PlayerCardMaster PlayerCardMaster { get; private set; }
        public EnemyCardMaster EnemyCardMaster { get; private set; }
        public OverrideSkillMaster OverrideSkillMaster { get; private set; }
        public AdventSkillMaster AdventSkillMaster { get; private set; }
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
            OverrideSkillMaster = provider.OverrideSkillMaster;
            AdventSkillMaster = provider.AdventSkillMaster;
            OtherSkillMaster = provider.OtherSkillMaster;
            ConsumeItemMaster=provider.ConsumeItemMaster;
            UserRankMaster = provider.UserRankMaster;
            InitialRewardStageMaster = provider.InitialRewardMaster;
            TestSkillMaster= provider.TestSkillMaster;

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
            return JsonConvert.DeserializeObject<T[]>(titleData[dataName]).ToDictionary(x => x.ToString()).Values.ToList();
        }
    }
}
using UnityEngine;
using fantec.Master;
using fantec.Common;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace fantec.Battle.Manager
{
    public interface IBattleMasterManager:IRegistable
    {
        StageMaster StageMaster { get; }
        PlayerCardMaster PlayerCardMaster { get; }
        EnemyCardMaster EnemyCardMaster { get; }
        OverrideSkillMaster OverrideSkillMaster { get; }
        AdventSkillMaster AdventSkillMaster { get; }
        OtherSkillMaster OtherSkillMaster { get; }
        UserRankMaster UserRankMaster { get; }
        TestSkillMaster TestSkillMaster { get; }
        UniTask LoadAsync(CancellationToken cts);
    }

    public class BattleMasterManager : MonoBehaviour, IBattleMasterManager
    {
        public StageMaster StageMaster => MasterDataManager.Instance.StageMaster;
        public PlayerCardMaster PlayerCardMaster => MasterDataManager.Instance.PlayerCardMaster;
        public EnemyCardMaster EnemyCardMaster => MasterDataManager.Instance.EnemyCardMaster;
        public OverrideSkillMaster OverrideSkillMaster => MasterDataManager.Instance.OverrideSkillMaster;
        public AdventSkillMaster AdventSkillMaster => MasterDataManager.Instance.AdventSkillMaster;
        public OtherSkillMaster OtherSkillMaster => MasterDataManager.Instance.OtherSkillMaster;
        public UserRankMaster UserRankMaster => MasterDataManager.Instance.UserRankMaster;
        public TestSkillMaster TestSkillMaster => MasterDataManager.Instance.TestSkillMaster;
        public void Register()
        {
            Locator.Register<IBattleMasterManager>(this);
        }

        public async UniTask LoadAsync(CancellationToken cts)
        {
            // TODO : サーバーから読み込みとの分岐
            await MasterDataManager.Instance.LoadMasterDataForLocalAsync(cts);
        }
    }
}
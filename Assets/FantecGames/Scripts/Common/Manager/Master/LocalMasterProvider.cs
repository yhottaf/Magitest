using UnityEngine;
using fantec.Master;
using System.Collections.Generic;

namespace fantec.Common.Master
{
    public class LocalMasterProvider : MonoBehaviour
    {
        [SerializeField] private UserRankMaster m_UserRankMaster; public UserRankMaster UserRankMaster => m_UserRankMaster;
        [SerializeField] private StageMaster m_StageMaster;       public StageMaster StageMaster => m_StageMaster;
        [SerializeField] private ConsumeItemMaster m_ConsumeItemMaster; public ConsumeItemMaster ConsumeItemMaster => m_ConsumeItemMaster;
        [SerializeField] private PlayerCardMaster m_PlayerCardMaster; public PlayerCardMaster PlayerCardMaster => m_PlayerCardMaster;
        [SerializeField] private EnemyCardMaster m_EnemyCardMaster; public EnemyCardMaster EnemyCardMaster=> m_EnemyCardMaster;
        [SerializeField] private OverrideSkillMaster m_OverrideSkillMaster; public OverrideSkillMaster OverrideSkillMaster => m_OverrideSkillMaster;
        [SerializeField] private AdventSkillMaster m_AdventSkillMaster; public AdventSkillMaster AdventSkillMaster=> m_AdventSkillMaster;
        [SerializeField] private OtherSkillMaster m_OtherSkillMaster;  public OtherSkillMaster OtherSkillMaster => m_OtherSkillMaster;
        [SerializeField] private InitialRewardStageMaster m_InitialRewardMaster; public InitialRewardStageMaster InitialRewardMaster => m_InitialRewardMaster;
        [SerializeField] private List<WaveMaster> m_WaveMasterList; public List<WaveMaster> WaveMasterList => m_WaveMasterList;
        [SerializeField] private List<ExpMaster> m_ExpMasterList; public List<ExpMaster> ExpMasterList => m_ExpMasterList;
        [SerializeField] private List<GrowthMaster> m_GrowthMasterList; public List<GrowthMaster> GrowthMasterList => m_GrowthMasterList;
        [SerializeField] private RewardStageMaster m_RewardStageMaster; public RewardStageMaster RewardStageMaster => m_RewardStageMaster;
        [SerializeField] private TestSkillMaster m_TestSkillMaster; public TestSkillMaster TestSkillMaster => m_TestSkillMaster;
    }
}
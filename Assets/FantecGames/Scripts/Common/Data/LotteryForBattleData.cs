using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace fantec
{
    /// <summary>
    /// バトルへ開始時に抽選されるための結果のデータ
    /// </summary>
    [System.Serializable]
    public class LotteryForBattleData
    {
        [SerializeField, ReadOnly] bool m_IsSuccess;
        [SerializeField, ReadOnly] int m_InitialRewardTableId; // 初回報酬のテーブルIDを保存
        [SerializeField, ReadOnly] int m_StageRewardTableId;   // ステージの報酬テーブル
        [SerializeField, ReadOnly] int m_RewardExp;            // クリア経験値

        public bool IsSuccess => m_IsSuccess;
        public int InitialRewardTableId => m_InitialRewardTableId;
        public int StageRewardTableId => m_StageRewardTableId;
        public int rewardExp => m_RewardExp;

        public LotteryForBattleData(bool isSuccess, int initialRewardTableId, int stageRewardTableId, int rewardExp)
        {
            m_IsSuccess = isSuccess;
            m_InitialRewardTableId = initialRewardTableId;
            m_StageRewardTableId = stageRewardTableId;
            m_RewardExp = rewardExp;
        }
    }
}
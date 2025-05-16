using UnityEngine;
using System;
using System.Linq;
using fantec.Master;
using UniRx;

namespace fantec.Common
{
    /// <summary>
    /// シーン間で値を受け渡す為に使用する
    /// 受け渡しに必要な変数、Get Set 関数のみ記載
    /// </summary>
    [CreateAssetMenu(fileName = "BridgingData", menuName = "Data/BridgingData", order = 1)]
    public class BridgingData : ScriptableObject
    {
        [Header("Battle")]
        [Tooltip("味方のチームのリスト")]
        [SerializeField] private TeamData m_TeamData;
        [Tooltip("バトルに使用されるステージデータ")]
        [SerializeField] private StageData m_StageData;
        [Tooltip("サーバーの抽選結果")]
        [SerializeField] private LotteryForBattleData m_LotteryData;

        [Header("ADV")]
        [SerializeField] private string m_AdvSeetName;


        public void SetData(BridgingData data)
        {
            m_TeamData = data.m_TeamData;
            m_StageData = data.m_StageData;
            m_LotteryData = data.m_LotteryData;
            m_AdvSeetName= data.m_AdvSeetName;
        }
        public void ResetData()
        {
            m_TeamData = null;
            m_StageData = null;
            m_LotteryData = null;
            m_AdvSeetName= null;
        }

        public void SetTeamData(TeamData data) => this.m_TeamData = data;
        public void SetStageData(StageData data) => this.m_StageData = data;
        public void SetAdvSeetName(string name)=>this.m_AdvSeetName = name;
        public void SetLotteryData(LotteryForBattleData data) => this.m_LotteryData = data;

        public bool GetIsActive()
        {
            return m_TeamData != null && m_StageData != null && m_LotteryData != null;
        }

        public StageData GetStageData()
        {
            return m_StageData ?? throw new ArgumentNullException("ステージデータが設定されていません。");
        }

        public TeamData GetTeamData()
        {
            return m_TeamData ?? throw new ArgumentNullException("チームメンバーが設定されていません。");
        }

        public LotteryForBattleData GetLotteryData()
        {
            return m_LotteryData ?? throw new ArgumentNullException("サーバーの抽選結果が入っていません。");
        }

        public TeamData GetCleanTeamData()
        {
            var result = m_TeamData.UnitList.Where(unit=>unit.cardId !=-1).ToList();
            if(result.Count > 0)
            {
                return new TeamData(result);
            }
            else { throw new Exception("メンバーを1名以上設定する必要があります。"); }
        }

        public string GetAdvSeetName()
        {
            return string.IsNullOrEmpty(m_AdvSeetName) ? "Sample" : m_AdvSeetName;
        }
    }
}
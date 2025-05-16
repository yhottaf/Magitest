using fantec.Common;
using fantec.Master;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace fantec.Debugger.BattleSetup
{
    public class DebugModel : MonoBehaviour
    {
        [SerializeField]private List<TeamData.Unit>m_UnitList=new List<TeamData.Unit>();
        [SerializeField] private StageData m_StageData;

        public List<TeamData.Unit> UnitList { get { return m_UnitList; } set { m_UnitList = value; } }
        public StageData StageData { get{ return m_StageData; } set { m_StageData = value; } }

        private void Awake()
        {
            m_UnitList.Clear();
            for(int i=0;i<Define.PARTY_CAPACITY;i++)
            {
                var unit = new TeamData.Unit(CardRarityType.R1, 0, -1, 1, 1, 1, 1, 1);
                m_UnitList.Add(unit);
            }
        }

        public TeamData GetTeamData()
        {
            return new TeamData(m_UnitList);
        }

        public bool GetIsMemberExist()
        {
            return m_UnitList.Any(x => x.cardId != -1);
        }
    }
}
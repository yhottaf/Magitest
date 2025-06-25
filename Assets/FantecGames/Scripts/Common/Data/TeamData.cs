using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace fantec.Common
{
    [System.Serializable]
    public class TeamData
    {
        [System.Serializable]
        public class  Unit
        {
            // ユニットに必要な情報は都度追加していく
            public CardRarityType rarityType;
            public int cardId;
            public int cardLevel;
            public int positionIndex;
            public int OverrideLevel;            // オーバーライド
            public int ExsaOverrideLevel;        // エクサオーバーライド
            public int SectaOverrideLevel;       // ゼタオーバーライド
            public int QuetaOverrideLevel;       // クエタオーバーライド

            public Unit(CardRarityType rarityType,int cardId,int cardLevel,int positionIndex,int OverrideLevel,int ExsaOverrideLevel,int SectaOverrideLevel,int QuetaOverrideLevel)
            {
                this.rarityType = rarityType;
                this.cardId = cardId;
                this.cardLevel = cardLevel;
                this.positionIndex = positionIndex;
                this.OverrideLevel = OverrideLevel;
                this.ExsaOverrideLevel = ExsaOverrideLevel;
                this.SectaOverrideLevel = SectaOverrideLevel;
                this.QuetaOverrideLevel = QuetaOverrideLevel;
            }
        }
        [SerializeField] private List<Unit> m_UnitList = new List<Unit>(Define.PARTY_CAPACITY);

        public List<Unit> UnitList => m_UnitList;

        public TeamData(List<Unit>unitList)
        {
            m_UnitList= unitList;
        }

        public IEnumerable<int>GetCardIds()
        {
            return m_UnitList.Select(unit=>unit.cardId);
        }

        public void ChangeMemberForIndex(int index,Unit unit)
        {
            m_UnitList[index] = unit;
        }

        public void ChangeMenberForPositionIndex(int positionIndex,Unit unit)
        {
            for(int i=0;i<m_UnitList.Count;i++)
            {
                if (m_UnitList[i].positionIndex==unit.positionIndex)
                {
                    m_UnitList[i] = unit;
                }
            }
        }
    }
}

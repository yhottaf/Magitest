using System.Collections.Generic;
using fantec.Common;
using fantec.Menu.Manager;

namespace fantec.PlayFabClient
{
    public class PartyData
    {
        private List<int> memberList = new List<int>(Define.PARTY_CAPACITY);

        public List<int> MemberList => memberList;

        public PartyData(List<int>memberList)
        {
            this.memberList = memberList;
        }

        public void ChangeMemberForIndex(int index,int cardId)
        {
            memberList[index] = cardId;
        }

        public TeamData GetTeamData()
        {
            List<TeamData.Unit> unitList = new List<TeamData.Unit>();

            for(int i=0;i<Define.PARTY_CAPACITY;i++)
            {
                int cardId = memberList[i];
                // cardId が 0 または -1 の場合はスキップ(ダミーで編成データなし)
                if (cardId == 0 || cardId == -1) continue;

                CardData cardData=CardManager.GetCardData(cardId);
                TeamData.Unit unit = new TeamData.Unit(cardData.rarityType, 
                    cardId, 
                    cardData.CardMasterData().GetLevelByExp(cardData.totalExp), // カードの現在のレベル取得
                    cardData.positionIndex[PlayerPrefsManager.SelectPartyIndex], // カードの配置箇所
                    cardData.OverrideSkillLevel,cardData.OverrideSkillLevel,cardData.OverrideSkillLevel,cardData.OverrideSkillLevel //オーバーライドスキルレベル 
                    );


                unitList.Add(unit);
            }

            return new TeamData(unitList);
        }

        /// <summary>
        /// パーティの合計Spec(戦闘力)を返します
        /// </summary>
        /// <returns></returns>
        public int GetTotalSpec()
        {
            int totalSpec = 0;
            for(int i=0;i<Define.PARTY_CAPACITY;i++)
            {
                // カード情報
                int cardId = memberList[i];
                CardData cardData =CardManager.GetCardData(cardId);

                totalSpec += cardData.CardMasterData().GetSpecByExp(cardData.totalExp, cardData.rarityType);
            }
            return totalSpec;
        }
    }
}
using fantec.Common;
using fantec.PlayFabClient;
using UnityEngine;

namespace fantec.Menu.Card.View
{
    public class CardDetailContentView : MonoBehaviour
    {
        public void Setup(int cardId)
        {
            CardData cardData=CardManager.GetCardData(cardId);
            UnitSetUp(cardData);
            CommonSetup(cardId);
        }

        /// <summary>
        /// 共通反映処理
        /// </summary>
        /// <param name="cardId"></param>
        private void CommonSetup(int cardId)
        {
            Master.PlayerCardData masterData=MasterDataManager.Instance.PlayerCardMaster.GetData(cardId);

            // キャラクター名 反映

            
        }

        /// <summary>
        /// 所持ユニット用のセットアップ
        /// </summary>
        /// <param name="data"></param>
        private void UnitSetUp(CardData data)
        {
            if(data!=null)
            {
                Master.PlayerCardData masterData = MasterDataManager.Instance.PlayerCardMaster.GetData(data.cardId);

                int level = masterData.GetLevelByExp(data.totalExp);         // 現在のレベル
                int maxLevel = EnumExtentions.GetMaxLevel(data.rarityType);  // 現在の限界レベル

                int nNowExperience = data.totalExp;     // 現在の経験値
                int nNowExpTotal;                       // 現在のレベルまでの経験値
                int nNextExpTotal;                      // 次のレベルまでの経験値

                // 経験値
                if(level!=maxLevel)
                {
                    nNowExpTotal=masterData.GetExpByLevel(level);
                    nNextExpTotal = masterData.GetExpByLevel(level + 1);
                }
                else // 最大レベルの場合
                {
                    nNowExpTotal = masterData.GetExpByLevel(level - 1);
                    nNextExpTotal = masterData.GetExpByLevel(level);
                }

                int nextLevel = masterData.GetLevelByExp(data.totalExp) + 1; // 次のレベル
                if(nextLevel>=EnumExtentions.GetMaxLevel(data.rarityType))
                {
                    nextLevel=EnumExtentions.GetMaxLevel(data.rarityType);
                }

                //　現在の経験値 / 次のレベルまでに必要な経験値　を描画
            }
        }
    }
}
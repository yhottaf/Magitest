using System.Xml.Linq;
using fantec.Common;
using fantec.Master;
using fantec.PlayFabClient;
using UnityEngine;
using UnityEngine.UI;

namespace fantec.Menu.Card.View
{
    public class CardDetailContentView : MonoBehaviour
    {
        [SerializeField] private Text characterNameText; // キャラクター名を表示するテキスト　
        [SerializeField] private Text hpText; // キャラクターのHPを表示するテキスト　
        [SerializeField] private Text atkText; // キャラクタの攻撃力を表示するテキスト　
        [SerializeField] private Text spdText; // キャラクターのスピードを表示するテキスト　
        [SerializeField] private Text specText; // キャラクターのスペックを表示するテキスト　

        public void Setup(int cardId)
        {
            CardData cardData = CardManager.GetCardData(cardId);
            UnitSetUp(cardData);
            CommonSetup(cardId);
        }

        /// <summary>
        /// 共通反映処理
        /// </summary>
        /// <param name="cardId"></param>
        private void CommonSetup(int cardId)
        {
            Master.PlayerCardData masterData = MasterDataManager.Instance.PlayerCardMaster.GetData(cardId);
            // キャラクター名 反映
            characterNameText.text = masterData.charaName;
        }

        [SerializeField] private Text levelText; // キャラクターのレベルを表示するテキスト
        [SerializeField] private Slider expSlider; // 経験値のスライダー
        [SerializeField] private Text nextexpText; // キャラクターの現在レベル中の経験値 / 次のレベルまでの経験値を表示するテキスト

        /// <summary>
        /// 所持ユニット用のセットアップ
        /// </summary>
        /// <param name="data"></param>
        private void UnitSetUp(CardData data)
        {

            if (data != null)
            {
                Master.PlayerCardData masterData = MasterDataManager.Instance.PlayerCardMaster.GetData(data.cardId);

                int level = masterData.GetLevelByExp(data.totalExp);         // 現在のレベル
                int maxLevel = EnumExtentions.GetMaxLevel(data.rarityType);  // 現在の限界レベル

                //現在のレベルを表示
                levelText.text = $"Lv.{level}　/  {maxLevel}(MAX)";

                int nNowExperience = data.totalExp;     // 現在の経験値
                int nNowExpTotal;                       // 現在のレベルまでの経験値
                int nNextExpTotal;                      // 次のレベルまでの経験値


                // ステータス取得
                int currentHp = data.GetHp();           //現在のHP
                int currentAtk = data.GetAtk();　       //現在の攻撃力
                int currentSpd = data.GetSpd();　       //現在のスピード
                int currentSpec = data.GetSpec();       //現在のスペック



                //ステータス表示
                hpText.text = $"HP: {currentHp}";
                atkText.text = $"ATK: {currentAtk}";
                spdText.text = $"SPD: {currentSpd}";
                specText.text = $"レアリティ: {currentSpec}";

                // 経験値
                if (level != maxLevel)
                {
                    nNowExpTotal = masterData.GetExpByLevel(level);
                    nNextExpTotal = masterData.GetExpByLevel(level + 1);
                }
                else // 最大レベルの場合
                {
                    nNowExpTotal = masterData.GetExpByLevel(level - 1);
                    nNextExpTotal = masterData.GetExpByLevel(level);
                }

                int nextLevel = masterData.GetLevelByExp(data.totalExp) + 1; // 次のレベル
                if (nextLevel >= EnumExtentions.GetMaxLevel(data.rarityType))
                {
                    nextLevel = EnumExtentions.GetMaxLevel(data.rarityType);

                }

                // 現在のレベル中の経験値 / 次のレベルまでに必要な経験値　を描画
                //expmasterからキャラクターに対応する経験値テーブルを取得
                var expMaster = MasterDataManager.Instance.GetMaster<ExpMaster>(masterData.expTableKey);

                //現在のレベルの中での経験値を取得
                int currentLevelExp = expMaster.GetCurrentTableExp(data.totalExp);
                // 次のレベルまでの経験値を取得
                int nextExp = expMaster.GetNextLevelExp(data.totalExp);

                // スライダー表示
                expSlider.value = Mathf.Clamp01((float)currentLevelExp / nextExp);

　　　　　　　　// 現在のレベル中の経験値 / 次のレベルまでに必要な経験値を表示
                nextexpText.text = $"現在の経験値：{currentLevelExp} 次のレベルまで：{nextExp}";

                
            }
        }
    }
}

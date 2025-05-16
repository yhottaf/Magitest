using fantec.Common;
using fantec.Master;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

namespace fantec.Debugger.BattleSetup
{
    public class DebugMemberCharacterView : MonoBehaviour
    {
        [SerializeField] private Dropdown m_CardDropDown;
        [SerializeField] private Dropdown m_RarityDropDown;
        [SerializeField] private Dropdown m_OverLimitDropDwon;
        [SerializeField] private Dropdown m_positioinIndexDropDown;
        [SerializeField] private InputField m_cardIdField;
        [SerializeField] private Text m_TagNameText;
        [SerializeField] private Text m_TitleText;
        [SerializeField] private int m_PositionIndex;
        [SerializeField] private InputField m_CardLevelField;

        private Dictionary<int, PlayerCardData> m_CardDataDic = new Dictionary<int, PlayerCardData>();

        private void Awake()
        {
            // インデックス番号を使用し名前設定
            m_TitleText.text = $"Card / Index : {m_PositionIndex}";

            var model = ExSceneManager.GetRootComponent<DebugModel>();
            var unit = model.UnitList[m_PositionIndex];

            // ----------------------------------------------------------------------------------------------------
            // カードドロップダウン
            // ----------------------------------------------------------------------------------------------------

            {
                // 空用ダミーの作成
                var optionList = new List<string>();
                optionList.Add("NONE");
                m_CardDataDic.Add(0, null);

                // マスターからドロップダウンオプションに入れ込む
                var playerCardDataList = MasterDataManager.Instance.PlayerCardMaster.dataList;
                for (int i = 0; i < playerCardDataList.Count; i++)
                {
                    var data = playerCardDataList[i];
                    m_CardDataDic.Add(i + 1, data);
                    optionList.Add($"{data.charaName}");
                }
                m_CardDropDown.ClearOptions();
                m_CardDropDown.AddOptions(optionList);

                // ドロップダウンで値が変化した際にメンバーを変更するように
                m_CardDropDown.OnValueChangedAsObservable()
                    .Select(index => m_CardDataDic[index])
                    .Subscribe(data =>
                    {
                        if (data != null)
                        {
                            m_TagNameText.text = string.Join(" ", data.tagNames);
                            m_cardIdField.text = data.cardId.ToString();
                            unit.cardId = data.cardId;
                        }
                        else
                        {
                            m_cardIdField.text = "-1";
                            m_TagNameText.text = "";
                            unit.cardId = -1;
                        }
                    })
                    .AddTo(this);

                // 配列の0番目は、何も選んでいないことを示す「NONE」　
                // なので+1した値からユニットデータとなるので+1した値から見る
                m_CardDropDown.value = m_PositionIndex + 1; 
            }

            // ----------------------------------------------------------------------------------------------------
            // レアリティドロップダウン
            // ----------------------------------------------------------------------------------------------------

            {
                var optionList = new List<string>() { "R1", "R2", "R3", "R4", "R5", "R6" };
                var rarityList = new List<CardRarityType>() { CardRarityType.R1, CardRarityType.R2, CardRarityType.R3, CardRarityType.R4, CardRarityType.R5, CardRarityType.R6 };
                m_RarityDropDown.ClearOptions();
                m_RarityDropDown.AddOptions(optionList);
                m_RarityDropDown.OnValueChangedAsObservable()
                    .Select(index => rarityList[index])
                    .Subscribe(rarityType =>
                    {
                        m_CardLevelField.text = rarityType.GetMinLevel().ToString();
                        unit.cardLevel = rarityType.GetMinLevel();
                        unit.rarityType = rarityType;
                    })
                    .AddTo(this);
            }

            // ----------------------------------------------------------------------------------------------------
            // 限凸ドロップダウン
            // ----------------------------------------------------------------------------------------------------

            //{
            //    var optionList = new List<string>() { "凸0", "凸1", "凸2", "凸3", "凸4", "凸5", "凸6" };
            //    m_OverLimitDropDwon.ClearOptions();
            //    m_OverLimitDropDwon.AddOptions(optionList);
            //    m_OverLimitDropDwon.OnValueChangedAsObservable()
            //        .Subscribe(index =>
            //        {
            //            unit.overLimitCount = index;
            //        })
            //        .AddTo(this);
            //}

            // ----------------------------------------------------------------------------------------------------
            // 配置ドロップダウン
            // ----------------------------------------------------------------------------------------------------

            {
                var optionList = new List<string>() { "1", "2", "3", "4", "5", "6","7","8","9" };
                m_positioinIndexDropDown.ClearOptions();
                m_positioinIndexDropDown.AddOptions(optionList);
                m_positioinIndexDropDown.OnValueChangedAsObservable()
                    .Subscribe(index =>
                    {
                        unit.positionIndex = index;
                    })
                    .AddTo(this);
                m_positioinIndexDropDown.value = m_PositionIndex;
            }

            // ----------------------------------------------------------------------------------------------------
            // レベル
            // ----------------------------------------------------------------------------------------------------

            // カードレベルレベル購読
            m_CardLevelField.text = "1";
            m_CardLevelField.OnEndEditAsObservable()
                .Where(value => string.IsNullOrEmpty(value) == false)
                .Select(value => int.Parse(value))
                .Subscribe(value => unit.cardLevel = value)
                .AddTo(this);
        }
    }
}
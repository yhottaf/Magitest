using fantec.Common;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace fantec.Debugger.BattleSetup
{
    public class DebugStageSelectView : MonoBehaviour
    {
        [SerializeField] private Dropdown m_DropDown;
        [SerializeField] private Text m_InfoText;

        private Dictionary<int,Master.StageData>m_StagedataDic=new Dictionary<int,Master.StageData>();


        private void Awake()
        {
            var optionList=new List<string>();
            var stageDataList = MasterDataManager.Instance.StageMaster.dataList.Where(data => data.playType == StagePlayType.Battle).ToList();
            for(int i=0;i<stageDataList.Count;i++)
            {
                var data = stageDataList[i];
                m_StagedataDic.Add(i, data);
                optionList.Add($"{data.stageId}:{data.stageName}");
            }

            m_DropDown.ClearOptions();
            m_DropDown.AddOptions(optionList);

            var model = ExSceneManager.GetRootComponent<DebugModel>();
            m_DropDown.OnValueChangedAsObservable()
                .Subscribe(index =>
                {
                    var data = m_StagedataDic[index];
                    model.StageData = data;
                    m_InfoText.text =
                    $"説明:{data.description}\n 消費スタミナ:{data.stamina}\n 難易度: {data.difficultyType} / 推奨レベル:{data.recommendedLevel} / 推奨レアリティ: {data.recommendedRarityType}";

                }).AddTo(this);
        }
    }
}
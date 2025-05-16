using fantec.Common;
using fantec.Master;
using UnityEngine;

namespace fantec.Master
{
    [System.Serializable]
    public class AdventSkillData : AbstructSkillData
    {
        public int triggerProbility; // 発生確率
        public int iconId;           // スキルアイコンなどを設定するなら
    }

    [ExcelAsset(AssetPath =AssetPath.MasterLocalDataFolderPath), CreateAssetMenu(fileName = "AdventSkillMaster", menuName = "ScriptableObjects/AdventSkillMaster")]
    public class AdventSkillMaster:AbstructSkillMaster<AdventSkillData>
    {

    }
}
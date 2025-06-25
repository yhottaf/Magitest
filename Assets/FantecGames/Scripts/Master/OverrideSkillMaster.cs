using fantec.Common;
using UnityEngine;

namespace fantec.Master
{
    [System.Serializable]
    public class OverrideSkillData : AbstructSkillData
    {
        public int iconId;     // スキルアイコンID
        public int[] triggerCardOriginId; // 発動に必要なキャラの個別ID
        public AffectOverrideType overrideType;
    }

    [ExcelAsset(AssetPath =AssetPath.MasterLocalDataFolderPath), CreateAssetMenu(fileName = "OverrideSkillMaster", menuName = "ScriptableObjects/OverrideSkillMaster")]
    public class OverrideSkillMaster:AbstructSkillMaster<OverrideSkillData>
    {

    }
}
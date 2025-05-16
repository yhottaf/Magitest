using fantec.Common;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace fantec.Master
{
    [System.Serializable]
    public class OtherSkillData:AbstructSkillData
    {
        public string[] addtionalCommands = new string[0];
        public int successRate;
        public int conditionValue;
    }

    [ExcelAsset(AssetPath = AssetPath.MasterLocalDataFolderPath), CreateAssetMenu(fileName = "OtherSkillMaster", menuName = "ScriptableObjects/OtherSkillMaster")]
    public class OtherSkillMaster : AbstructSkillMaster<OtherSkillData>
    {
        private readonly Dictionary<int, int> m_CategoryToDataIdDic = new Dictionary<int, int>()
        {
            { (int)AffectCategoryType.Poison, 20000 },
            { (int)AffectCategoryType.Burn, 20100 },
            { (int)AffectCategoryType.Frost, 20200 },
            { (int)AffectCategoryType.Blind, 20300 },
        };

        public OtherSkillData GetData(AffectCategoryType categoryType,AffectEfficacyType efficacyType)
        {
            try { return GetData(m_CategoryToDataIdDic[(int)categoryType] + (int)efficacyType); }
            catch (InvalidOperationException) { throw new InvalidOperationException($"[{categoryType} / {efficacyType}] \n[微、小、中...]などを付与し忘れている可能性があります。"); }
        }
    }
}
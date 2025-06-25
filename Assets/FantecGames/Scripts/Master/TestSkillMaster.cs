using fantec.Common;
using System;
using UnityEngine;

namespace fantec.Master
{
    [Serializable]
    public class TestSkillData:AbstructSkillData
    { 
    }
    [ExcelAsset(AssetPath =AssetPath.MasterLocalDataFolderPath), CreateAssetMenu(fileName = "TestSkillMaster", menuName = "ScriptableObjects/TestSkillMaster")]
    public class TestSkillMaster : AbstructSkillMaster<TestSkillData>
    {

    }
}
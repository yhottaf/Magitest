using fantec.Common;
using UnityEngine;

namespace fantec.Master
{
    [System.Serializable]
    public class EnemyCardData : AbstructCardData
    {

    }

    [ExcelAsset(AssetPath =AssetPath.MasterLocalDataFolderPath), CreateAssetMenu(fileName = "EnemyCardMaster", menuName = "ScriptableObjects/EnemyCardMaster")]
    public class EnemyCardMaster:AbsturctCardMaster<EnemyCardData>
    {

    }
}
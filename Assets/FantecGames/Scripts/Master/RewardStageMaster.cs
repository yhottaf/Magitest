using fantec.Common;
using System;
using System.Linq;
using UnityEngine;

namespace fantec.Master
{
    [Serializable]
    public class RewardStageData:IData
    {
        public string tableId; // テーブルID
        public string itemId1; // アイテムID1
        public int weight1;    // アイテムID1の重み
        public string itemId2; // アイテムID2
        public int weight2;    // アイテムID2の重み
        public string itemId3; // アイテムID3
        public int weight3;    // アイテムID3の重み
        public int minLottery; // 最小抽選回数
        public int maxLottery; // 最大抽選回数
    }
    [ExcelAsset(AssetPath=AssetPath.MasterLocalDataFolderPath), CreateAssetMenu(fileName = "RewardStageMaster", menuName = "ScriptableObjects/RewardStageMaster")]
    public class RewardStageMaster : MasterBase<RewardStageData>
    {
        public RewardStageData GetDataLocal(int tableId)
        {
            var tableIdStr = tableId.ToString();
            try { return dataList.First(x => x.tableId ==tableIdStr); }
            catch { throw new InvalidOperationException($"[tableId : {tableId}]"); }
        }
    }
}
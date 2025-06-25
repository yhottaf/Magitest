using fantec.Common;
using System;
using System.Linq;
using UnityEngine;

namespace fantec.Master
{
    [Serializable]
    public class InitialRewardStageData : IData
    {
        public string tableId;
        public string itemId1;
        public int quantity1;  // itemId1の入手できる数
        public string itemId2;
        public int quantity2;  // itemId2の入手できる数
        public string itemId3;
        public int quantity3;  // itemId3の入手できる数
        public int VC;         // 入手できる無償石の数
        public int[] cardId;   //入手できるカードのID
    }
    [ExcelAsset(AssetPath =AssetPath.MasterLocalDataFolderPath),CreateAssetMenu(fileName ="InitialRewardStageMaster",menuName ="ScriptableObjects/InitialRewardStageMaster")]
    public class InitialRewardStageMaster : MasterBase<InitialRewardStageData>
    {
        public InitialRewardStageData GetData(int tableId)
        {
            try
            {
                return dataList.First(x => x.tableId == tableId.ToString());
            }
            catch
            {
                throw new InvalidOperationException($"[TableId:{tableId}] は存在しません。");
            }
        }
    }
}
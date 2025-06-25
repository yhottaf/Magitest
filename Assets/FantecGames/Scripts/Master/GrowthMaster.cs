using fantec.Common;
using System;
using UnityEngine;

namespace fantec.Master
{
    [System.Serializable]
    public class GrowthData : IData // ステータスを改良したい場合はこの項目に増やす
    {
        public int level;
        public float HP;
        public float ATK;
        public float DEF;
        public float SPD;
    }

    [ExcelAsset(AssetPath =AssetPath.MasterLocalDataGrowthFolderPath), CreateAssetMenu(fileName = "GrowthMaster", menuName = "ScriptableObjects/GrowthMaster")]
    public class GrowthMaster:MasterBase<GrowthData>
    {
        public GrowthData GetData(int level)
        {
            try { return dataList[level - 1]; }
            catch { throw new IndexOutOfRangeException($"[level : {level}] はデータの範囲外です。"); }
        }
    }
}
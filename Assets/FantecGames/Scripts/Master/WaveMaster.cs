using fantec.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace fantec.Master
{
    [System.Serializable]
    public class WaveData : IData
    {
        public CardRarityType rarityType; // 敵ユニットのレアリティ
        public int cardId;                // 敵ユニットのID
        public int cardLevel;             // 敵ユニットのレベル
        public int positionIndex;         // 配置インデックス
        public int waveIndex;             // ウェーブのインデックス
        public bool isBoss;               // ボスであるか否か

        public TeamData.Unit ToTeamUnitData()
        {
            var unitData = new TeamData.Unit(rarityType, cardId, cardLevel, positionIndex, 1, 1, 1, 1);
            return unitData;
        }
    }

    public static class WaveDataExtensions
    {
        public static IEnumerable<WaveData> GetDatas(IEnumerable<WaveData> waveDatas, int waveIndex)
        {
            try { return waveDatas.Where(x => x.waveIndex == waveIndex) ?? throw new Exception(); }
            catch { throw new Exception($"[waveIndex : {waveIndex}] のキャラは存在しません。"); }
        }

        public static int GetMaxWaveIndex(this IEnumerable<WaveData> @this) => @this.Max(x => x.waveIndex);
        public static int GetMaxWave(this IEnumerable<WaveData> @this) => @this.GetMaxWaveIndex() + 1;

        public static TeamData ToTeamData(this IEnumerable<WaveData> waveDataList)
        {
            var teamUnitDataList = new List<TeamData.Unit>();
            foreach (var waveData in waveDataList) teamUnitDataList.Add(waveData.ToTeamUnitData());
            return new TeamData(teamUnitDataList);
        }

        public static (TeamData teamData, bool isBoss)[] ToWaveAndIsBossData(this IEnumerable<WaveData> waveDatas)
        {
            var maxWave = waveDatas.GetMaxWave();
            var result = new (TeamData teamData, bool isBoss)[maxWave];
            for (int i = 0; i < maxWave; i++)
            {
                var targetWaveDatas = waveDatas.Where(x => x.waveIndex == i);
                result[i].teamData = targetWaveDatas.ToTeamData();
                result[i].isBoss = targetWaveDatas.Any(x => x.isBoss);
            }
            return result;
        }
    }

    [ExcelAsset(AssetPath = AssetPath.MasterLocalDataWaveFolderPath), CreateAssetMenu(fileName = "WaveMaster", menuName = "ScriptableObjects/WaveMaster")]
    public class WaveMaster : MasterBase<WaveData>
    {
        public List<WaveData> GetDataList()
        {
            try
            {
                if (dataList == null) throw new Exception("waveData が null です。");
                if (dataList.Count == 0) throw new Exception("waveData に要素が存在しません。");
                return dataList;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
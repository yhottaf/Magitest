using fantec.Common;
using fantec.Master;
using UnityEngine;

namespace fantec.Battle
{
    public class StageEntity
    {
        public readonly (TeamData teamData, bool isBoss)[] waveDatas;
        public readonly StageDifficultyType difficultyType; // 難易度種別
        public readonly int stageId;                        // ステージID
        public readonly int stamina;                        // 消費スタミナ
        public readonly string stageName;                   // ステージ名
        public readonly string normalBgm;                   // 通常BGM
        public readonly string bossBgm;                     // ボス戦時のBGM
        public readonly string normalBg;                    // 通常背景
        public readonly string bossBg;                      // ボス戦背景
        public readonly string FieldImg;                    // フィールドの画像
        public readonly int maxWave;                        // 最大ウェーブ数
        public readonly int maxWaveIndex;                   // 最大ウェーブインデックス数
        public readonly int rewardTableId;
        public readonly int rewardExp;
        
        public StageEntity(StageData data)
        {
            this.difficultyType = data.difficultyType;
            this.stageId = data.stageId;
            this.stamina = data.stamina;
            this.stageName = data.stageName;
            this.normalBgm = data.normalBgm;
            this.bossBgm= data.bossBgm;
            this.normalBg = data.normalBg;
            this.bossBg = data.bossBg;
            this.rewardTableId = data.rewardTableId;
            this.rewardExp = data.rewardExp;
            this.FieldImg = data.fieldImg;
            // ステージの割り当て
            var waveDataList = MasterDataManager.Instance.GetMaster<WaveMaster>(data.waveMasterKey).GetDataList();
            maxWaveIndex = waveDataList.GetMaxWaveIndex();
            maxWave=waveDataList.GetMaxWave();
            waveDatas = waveDataList.ToWaveAndIsBossData();
        }
    }

    public static class StageEntityExtensions
    {
        public static StageEntity ToEntity(this StageData data)
        {
            return new StageEntity(data);
        }
    }
}
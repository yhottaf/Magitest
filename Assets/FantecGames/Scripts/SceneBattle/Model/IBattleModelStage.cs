using System;
using UnityEngine;
using fantec.Common;

namespace fantec.Battle.Model
{
    public interface IBattleModelStage :ILocatable,IDisposable,IResetable
    {
        /// <summary> ステージデータの情報更新監視 </summary>
        IObservable<IBattleModelStage> OnUpdateStageDataObservable { get; }
        ///<summary> 現在のウェーブ数更新監視 </summary>
        IObservable<int> OnCurrentWaveIndexReactive { get; }
        ///<summary> 現在のターン経過更新監視 </summary>
        IObservable<int> OnCurrentTurnIndexReactive { get; }
        /// <summary> ターンの最後に行動する者 </summary>
        IObservable<IBattler> OnLastActingUnit { get; }

        /// <summary> 次のウェーブへ </summary>
        void NextWave();
        /// <summary> 次のターンへ </summary>
        void NextTurn();
        /// <summary> 最初のターンへ </summary>
        void ResetTurn();
        ///<summary> ターンを直接設定 </summary>
        void SetTurn(int turnIndex);
        ///<summary> ステージデータを設定する </summary>
        void SetStageEntity(StageEntity stageEntity);
        ///<summary> ウェーブを直接設定 </summary>
        void SetWave(int waveIndex);
        ///<summary> そのターンの最後に行動するユニットを直接設定 </summary>
        void SetLastActingUnit(IBattler battler);
        ///<summary> 敵メンバーを更新 </summary>
        void UpdateMember();
        ///<summary> BGM名の取得 </summary>
        string GetBattleBgmName();
        ///<summary> 背景名の取得 </summary>
        string GetBattleBgName();

        string GetFieldImgName();


        ///<summary> ステージデータ </summary>
        StageEntity Entity { get; }
        ///<summary> 現在のウェーブの敵パーティーデータ </summary>
        TeamData CurrentWaveTeamData { get; }
        ///<summary> 現在のウェーブ数 </summary>
        int CurrentWave { get; }
        ///<summary> 現在のウェーブインデックス </summary>
        int CurrentWaveIndex { get; }
        ///<summary> 最大ウェーブに到達しているか否か </summary>
        bool IsMaxWave { get; }
        ///<summary> 最初のウェーブであるか否か </summary>
        bool IsFirstWave { get; }
        ///<summary> ボスウェーブであるか否か </summary>
        bool IsBossWave { get; }
        ///<summary> 現在のターン数 </summary>
        int CurrentTurn { get; }
        ///<summary> 現在のターンインデックス </summary>
        int CurrentTurnIndex { get; }
        ///<summary> 最大ターンに到達しているか否か </summary>
        bool IsMaxTurn { get;}
        ///<summary> 最初のターンであるか否か </summary>
        bool IsFirstTurn { get; }
        ///<summary> ターンの最後に行動するユニット </summary>
        IBattler LastActingUnit { get; }
    }
}
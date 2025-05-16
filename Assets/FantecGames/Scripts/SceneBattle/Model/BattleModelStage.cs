using fantec.Common;
using System;
using UniRx;
using UnityEngine;

namespace fantec.Battle.Model
{
    public class BattleModelStage : MonoBehaviour, IBattleModelStage, IRegistable
    {
        public IObservable<IBattleModelStage> OnInitCompletedObservable => m_SetupCompletedSubject;
        private readonly AsyncSubject<IBattleModelStage> m_SetupCompletedSubject = new AsyncSubject<IBattleModelStage>();

        public IObservable<IBattleModelStage> OnUpdateStageDataObservable => m_UpdateStageDataReactive.SkipLatestValueOnSubscribe();
        private readonly ReactiveProperty<IBattleModelStage> m_UpdateStageDataReactive = new ReactiveProperty<IBattleModelStage>();

        // ウェーブ数の監視
        public IObservable<int> OnCurrentWaveIndexReactive => m_CurrentWaveIndexReactive;
        private readonly ReactiveProperty<int> m_CurrentWaveIndexReactive=new ReactiveProperty<int>();

        // ターン数の監視
        public IObservable<int> OnCurrentTurnIndexReactive => m_CurrentTurnIndexReactive;
        private readonly ReactiveProperty<int>m_CurrentTurnIndexReactive=new ReactiveProperty<int>();

        // 最後に行動するユニットの監視
        public IObservable<IBattler> OnLastActingUnit => m_LastActingUnitReactive;
        private readonly ReactiveProperty<IBattler> m_LastActingUnitReactive = new ReactiveProperty<IBattler>();

        public StageEntity Entity => m_Entity;                    // ステージデータ
        public TeamData CurrentWaveTeamData => m_CurrentTeamData; // 現在のウェーブの敵パーティーデータ
        public int CurrentWave => m_CurrentWaveIndexReactive.Value + 1; // 現在のウェーブ数
        public int CurrentWaveIndex => m_CurrentWaveIndexReactive.Value; 
        public bool IsMaxWave => CurrentWave == m_Entity.maxWave;
        public bool IsFirstWave => CurrentWaveIndex == 0;
        public bool IsBossWave => m_Entity.waveDatas[CurrentWaveIndex].isBoss;

        public int CurrentTurn => m_CurrentTurnIndexReactive.Value + 1; // 現在のターン数
        public int CurrentTurnIndex => m_CurrentTurnIndexReactive.Value;

        public bool IsMaxTurn => CurrentTurn == 31; // 最大ターンは30ターンまで( MEMO : 30ターンのターンの終わりで勝敗結果を決めるので正確には31の変数で終了させる)
        public bool IsFirstTurn => CurrentTurn == 1;

        public IBattler LastActingUnit => m_LastActingUnitReactive.Value;



        private StageEntity m_Entity;
        private TeamData m_CurrentTeamData;

        public void Register()
        {
            Locator.Register<IBattleModelStage>(this);
        }

        public void Dispose()
        {
            m_SetupCompletedSubject.Dispose();
            m_CurrentWaveIndexReactive.Dispose();
            m_UpdateStageDataReactive.Dispose();
            m_CurrentTurnIndexReactive.Dispose();
            m_LastActingUnitReactive.Dispose();
        }

        public void Reset()
        {
            m_CurrentWaveIndexReactive.Value = 0;
            m_CurrentTurnIndexReactive.Value = 0;
            m_LastActingUnitReactive.Value = null;
        }

        /// <summary>
        /// ステージデータを設定する
        /// </summary>
        public void SetStageEntity(StageEntity entity)
        {
            m_Entity=entity;
            m_UpdateStageDataReactive.SetValueAndForceNotify(this);
        }

        public void SetLastActingUnit(IBattler battler)
        {
            m_LastActingUnitReactive.Value = battler;
        }

        public void NextWave()
        {
            // 最大ウェーブでなければ
            if(IsMaxWave==false)
            {
                m_CurrentWaveIndexReactive.Value++;
            }
            else
            {
                Debug.LogError($"最大ウェーブです wave : {CurrentWave}");
            }
        }

        public void NextTurn()
        {
            if (IsMaxTurn == false)
            {
                m_CurrentTurnIndexReactive.Value++;
                if (IsMaxTurn)
                {
                    Debug.LogError($"ラストターンに到達しました ターン : {CurrentTurn}");
                }
                else
                {
                    Debug.Log($"ターン : {CurrentTurn}");
                }
            }
            else
            {
                Debug.LogError($"ラストターンに到達しました ターン : {CurrentTurn}");
            }
        }

        // 主に次ウェーブに移った際の新しい敵の更新に使用する
        public void UpdateMember()
        {
            m_CurrentTurnIndexReactive.Value = 0; // ターンの初期化をしておく(ウェーブ切り替え時はターン初期化にする？)
            m_CurrentTeamData = m_Entity.waveDatas[CurrentWaveIndex].teamData;
        }

        /// <summary>
        /// ウェーブを直接設定
        /// </summary>
        public void SetWave(int waveIndex)
        {
            if(m_Entity.maxWave<=waveIndex)
            {
                m_CurrentWaveIndexReactive.Value = m_Entity.maxWaveIndex;

                Debug.LogError($"最大ウェーブを超えています。 ターゲットウェーブ : {waveIndex + 1} 最大ウェーブ : {m_Entity.maxWave}");
            }
            else
            {
                m_CurrentWaveIndexReactive.Value = waveIndex;
            }
        }

        public void SetTurn(int turnIndex)
        {
            if(31<=turnIndex)
            {
                m_CurrentTurnIndexReactive.Value = 31;

                Debug.LogError($"最大ターン数を超えています。ターゲットターン : {turnIndex + 1} 最大ターン : {30}");
            }
            else
            {
                m_CurrentTurnIndexReactive.Value = turnIndex;
            }
        }

        // ターン数を最初のターンに戻す
        public void ResetTurn()
        {
            m_CurrentTurnIndexReactive.Value = 0;
        }

        // 現在のウェーブのBGMネームを取得する
        public string GetBattleBgmName()
        {
            return IsBossWave ? Entity.bossBgm : Entity.normalBgm;
        }

        // 現在のウェーブの背景画像ネームを取得する
        public string GetBattleBgName()
        {
            return IsBossWave ? Entity.bossBg : Entity.normalBg;
        }

        // 現在のウェーブのフィールドの画像ネームを取得する
        public string GetFieldImgName()
        {
            return Entity.FieldImg;
        }
    }
}
using Cysharp.Threading.Tasks;
using fantec.Battle.Utiles;
using System.Threading;
using UniRx;
using UnityEngine;

namespace fantec.Battle.Manager
{
    public interface IBattleFlowManager:ILocatable
    {
        void ChangeFlow<T>() where T : FlowBase, new();
        void ChangeFlowReserve<T>()where T:FlowBase, new();
        void Exit();
    }

    /// <summary>
    /// バトルの流れを管理
    /// </summary>
    /// 
    // Boot -> Init -> Admission -> Gimmick -> WaveSetup -> Move -> Combat -> Judge -> WaveSetup へ戻る 
    //                                                                              -> Win
    //                                                                              -> Lose
    public partial class BattleFlowManager : SpeedableBehaviour,IBattleFlowManager,IRegistable
    {
        /// <summary> 現在のフロー (初期値は Boot) </summary>
        private FlowBase m_CurrentFlow=new FlowBoot();

        /// <summary> 遷移待ちのフロー </summary>
        private FlowBase m_ReserveFlow = null;

        /// <summary> CompositeDisposableは適切にDispose()を呼び出し、不要なリソースを解放する。  
        /// メモリリークの原因を排除する
        /// フローが切り替わったタイミングで Clearされる
        /// </summary>
        private CompositeDisposable m_OnChangeFlowDisposables=new CompositeDisposable();

        /// <summary> オブジェクトが破棄されたタイミングで Dispose()される メモリ解放 </summary>
        private CompositeDisposable m_OnDestroyDisposables=new CompositeDisposable();

        /// <summary> 破棄されたタイミングでキャンセレーション </summary>
        private CancellationToken m_OnDestroyCancellationToken;

        /// <summary> 速度可変に対応した Sequence </summary>
        private SaveableSequence m_Sequence=new SaveableSequence();

        private bool m_IsQuitting = false;

        #region IRegistable
        public void Register()
        {
            Locator.Register<IBattleFlowManager>(this);
        }
        #endregion

        #region SpeedableBehavior
        protected override void OnSetCurrentTimeScale(float speed)
        {
            m_Sequence.TimeScale= speed;
        }
        #endregion

        protected override void Awake()
        {
            base.Awake();

            m_OnDestroyCancellationToken = this.GetCancellationTokenOnDestroy();

            m_CurrentFlow.OnEnter(this, null);

            // 演出では停止しないように
            SetThroughDirectingPause(true);
        }

        private void OnDestroy()
        {
#if UNITY_EDITOR
            // エディターが終了していたら
            if (m_IsQuitting) return;
#endif
            m_CurrentFlow.OnExit(this,null);
            m_Sequence.Kill();
            m_OnChangeFlowDisposables.Dispose();
            m_OnDestroyDisposables.Dispose();
        }

        private void Update()
        {
            m_CurrentFlow.OnUpdate(this);
        }

        private void OnApplicationQuit()
        {
            m_IsQuitting= true;
        }

        /// <summary>
        /// フローをタイプ指定で変更する
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void ChangeFlow<T>() where T : FlowBase,new()
        {
            ChangeFlow(new T());
        }

        /// <summary>
        /// フローをインスタンス指定で変更する
        /// </summary>
        private void ChangeFlow(FlowBase flow)
        {
            var nextFlow = flow;              // 次のフロー保持
            var prevFlow = m_CurrentFlow;     // 現在のフローを前回のフローとして保持

            if(m_ReserveFlow!=null)           // 予約中のフローがある時
            {
                if(m_ReserveFlow!=flow)       // 遷移予定のフローと違っていれば
                {
                    nextFlow = m_ReserveFlow; // 次のフローを予約中のフローに変更
                }
                m_ReserveFlow = null;         // 予約を空にする
            }

            m_OnChangeFlowDisposables.Clear();     // フロー内での購読クリア
            prevFlow.OnExit(this,nextFlow);        // 前回のフローの終了通知
            m_CurrentFlow = nextFlow;              // 次のフローへ変更
            m_CurrentFlow.OnEnter(this, prevFlow); // 次のフロー開始通知
        }

        /// <summary>
        /// 遷移予約する
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void ChangeFlowReserve<T>() where T:FlowBase,new()
        {
            m_ReserveFlow = new T();
        }

        /// <summary>
        /// バトルを終了する際に呼ぶ
        /// </summary>
        public void Exit()
        {
            this.ChangeFlow<FlowExit>();
        }
    }
}
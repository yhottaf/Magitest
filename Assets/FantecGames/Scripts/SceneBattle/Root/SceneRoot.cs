using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

namespace fantec.Battle.Root
{
    // シーンのロード完了を待ったり、ネットワーク接続や設定のロードが終わるまでバトルを開始させない。
    // 複数の初期化処理が終わるまでUIを表示させない
    // ゲーム開始時の GameManager や UIManager の準備が完了するのを待機する

    // Unityの非同期処理を効率的に管理するクラス


    [DefaultExecutionOrder(-1)] // 他のMonoBehaviorよりも先に実行するように指定
    public class SceneRoot : MonoBehaviour
    {
        UniTaskCompletionSource<bool> _loadCompletedSource;

        /// <summary>
        /// シーンの初期化を非同期で実行
        /// </summary>
        async UniTaskVoid Awake()
        {
            var cts = CancellationTokenSource.CreateLinkedTokenSource(this.GetCancellationTokenOnDestroy());
            
            // シーンの初期化が完了するまで、非同期処理で待機。
            // オブジェクトが破棄されたときに初期化処理をキャンセルできる
            await SceneInitializeAsync(cts);
        }

        /// <summary>
        /// 非同期でシーンの初期化を実行。IRootInitiater インターフェースを実装したコンポーネントの全ての初期化が完了すると
        /// _loadCompletedSource.TrySetResult(true)を呼び出し、初期化完了を通知する
        /// </summary>
        /// <param name="cts"></param>
        async UniTask SceneInitializeAsync(CancellationTokenSource cts)
        {
            // ゲームオブジェクトを一時的に非アクティブにして、初期化が終わるまで表示しない
            this.gameObject.SetActive(false);

            // 非同期タスクの完了を待機するための変数
            _loadCompletedSource = new UniTaskCompletionSource<bool>();

            foreach(var initializer in GetComponents<IRootInitiater>())
            {
                await initializer.InitializeAsync(cts);

                // もしキャンセルが発生したら処理を中断
                cts.Token.ThrowIfCancellationRequested();
            }

            // Unload check
            if(this!=null)
            {
                this.gameObject.SetActive(true);

                _loadCompletedSource.TrySetResult(true);
            }
        }

        // 外部から InitCompletedCheckAsync()を呼び出して初期化が完了したかどうかを返す
        public UniTask<bool>InitCompletedCheckAsync()
        {
            return _loadCompletedSource.Task;
        }
    }
}
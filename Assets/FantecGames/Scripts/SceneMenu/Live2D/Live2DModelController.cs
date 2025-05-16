using Cysharp.Threading.Tasks;
using fantec.Common;
using fantec.Menu.Manager;
using fantec.PlayFabClient;
using Live2D.Cubism.Framework.Motion;
using Live2D.Cubism.Rendering;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

namespace fantec.Menu
{
    public class Live2DModelController : MonoBehaviour
    {
        [SerializeField] 
        private AnimationClip IdleMotion; // アイドルモーション

        // Live2Dのモーションをまとめたリスト
        [SerializeField]
        private List<AnimationClip> anim=new List<AnimationClip>();

        // インターバル（秒）
        private float interval = 8.0f;         
        // 最大値でリセットするか
        private bool loop = true;              
        // 現在の値
        private int currentValue = 0;         
        // インターバル用のタイマー
        private float timer = 0.0f;       
        // 初期化完了フラグ
        private bool isInitialized = false;

        // アニメーションの終了を監視
        private readonly Subject<Unit> onMotionFinished = new Subject<Unit>();

        // Live2Dモデルについてるモーションコントローラー
        private CubismMotionController m_MotionController;

        // Live2Dモデルについているレンダーコントローラー
        private CubismRenderController m_RenderController;

        // 現在のアニメーションの再生時間の長さ
        private float CurrentAnimationLength = 0f;


        // Live2Dモデルのプレハブ
        private GameObject model;

        void Start()
        {
            // キャッシュが存在するかどうかの確認変数
            bool isValidLive2DData = false;

            { // まずはキャッシュが存在するかどうか確認する処理
            
                // 現在ロード中のアドレス一覧を確認(デバッグ)
                var loading = AssetManager.Instance.GetLoadedLive2DModels();
                if (loading.Count.Equals(0))
                {
                    Debug.Log("ロードされているLive2Dデータがないため新規ロードを開始します。");
                }
                foreach (var addr in loading)
                {
                    Debug.Log($"現在キャッシュに残っているLive2Dデータ: {addr}");
                    // キャッシュが存在したらtrueに変える。
                    isValidLive2DData = true;
                }
            }

            SetInitLive2D().Forget(); // 非同期で呼び出し

            onMotionFinished.
                Subscribe(_ =>
                {
                    PlayIdleMotion();
                }).AddTo(this);

            MenuManager.Instance.onLive2DState.
                Subscribe(_=>
                {
                    // モデルの参照があるなら消す。
                    if (model!=null)
                    {
                        isInitialized = false;
                        Destroy(model);
                    }
                    else
                    {
                        // モデルがnullなら再度Live2Dオブジェクトを作り直す
                        SetInitLive2D().Forget();
                    }
                }).AddTo(this);

            
            // キャッシュが存在した場合はチェックする必要がない
            if (!isValidLive2DData)
            {
                // Home画面読み込み時にLive2Dのキャッシュデータがなかった場合は
                // Load処理を挟んだ後にロードされているか確認。
                MenuManager.Instance.LoadedCheckLive2DData();
            }
        }

        void Update()
        {
            // 初期化完了後のみ実行
            if (!isInitialized) return;

            if (model != null)
            {
                timer += Time.deltaTime;

                if (timer >= interval+CurrentAnimationLength)
                {
                    timer = 0.0f;

                    // パラメータの更新
                    currentValue++;
                    PlayMotion(anim[currentValue]);

                    // 最大値に達したらリセット
                    if (currentValue >= anim.Count-1)
                    {
                        if (loop)
                        {
                            currentValue = 0;
                        }
                        else
                        {
                            enabled = false;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Live2D　モデルの初期化
        /// </summary>
        /// <returns></returns>
        private async UniTask SetInitLive2D()
        {
            // プロフィールカードをLive2Dで表示させる
            CardData carddata = CardManager.GetCardData(UserDataManager.User.ProfileCardId);
            int originId = carddata.CardMasterData().originId;
            List<string> AnimList = new List<string>(carddata.CardMasterData().AnimationClip.ToList());
            if (AnimList.Count.Equals(0))
            {
                Debug.Log($"{carddata.CardMasterData().readCharaname}　のマスターデータにアニメーションが１つも設定されていません。");
            }

            // モデルのロード
            model = await AssetManager.Instance.LoadAssetAsync<GameObject>(AssetPath.GetLive2DPath(originId));
            model.SetActive(false);
            IdleMotion = await AssetManager.Instance.LoadAssetAsync<AnimationClip>(AssetPath.GetIdleMotion(originId));

            model = Instantiate(model);
            // モデルを子オブジェクトとして追加
            model.transform.SetParent(this.transform);

            // ローカル座標をマスターデータの位置に調整しなおす
            model.transform.localPosition = carddata.CardMasterData().Live2DlocalPosition;
            model.transform.localRotation = Quaternion.identity;
            model.transform.localScale = carddata.CardMasterData().Live2DlocalScale;

            m_MotionController = model.GetComponent<CubismMotionController>();
            m_RenderController = model.GetComponent<CubismRenderController>();

         
            await LoadAnimationsAsync(originId, AnimList);

            Debug.Log($"Live2Dモデル{carddata.CardMasterData().readCharaname}の初期化が完了しました。");



            //  **1秒後** にフェードインする
            Observable
                .Timer(System.TimeSpan.FromSeconds(1)) // 1秒後に発火
                .Subscribe(_ =>
                {
                    if (model != null)
                    {
                        model.SetActive(true);
                        PlayIdleMotion();
                        Debug.Log("フェードインを開始します。");
                        StartCoroutine(FadeIn());
                    }
                }).AddTo(this);
#if Test
            model = Instantiate(m_TestModel);
           //  モデルを子オブジェクトとして追加
            model.transform.SetParent(this.transform);
        //    animator = model.GetComponent<Animator>();

            // ローカル座標をマスターデータの位置に調整しなおす
            model.transform.localPosition = carddata.CardMasterData().Live2DlocalPosition;
            model.transform.localRotation = Quaternion.identity;
            model.transform.localScale = carddata.CardMasterData().Live2DlocalScale;

            m_MotionController=model.GetComponent<CubismMotionController>();
            
            PlayIdleMotion();
#endif

        }

        private void PlayMotion(AnimationClip animation)
        {
            if ((m_MotionController == null) || (animation == null))
            {
                return;
            }

            // モーションを再生
            m_MotionController.PlayAnimation(animation, isLoop: false,priority: CubismMotionPriority.PriorityForce);

            // 再生するモーションの長さを変数に格納
            CurrentAnimationLength = animation.length;

            Observable
                .Timer(System.TimeSpan.FromSeconds(animation.length))
                .Subscribe(_ =>
                {
                    onMotionFinished.OnNext(Unit.Default); // モーションが終了したことを通知
                }).AddTo(this);
        }

        /// <summary>
        /// 待機モーションを再生させる
        /// </summary>
        private void PlayIdleMotion()
        {
            if(IdleMotion!=null)
            {
                m_MotionController.PlayAnimation(IdleMotion,priority: CubismMotionPriority.PriorityForce, isLoop:true);
            }
        }

        /// <summary>
        /// 非同期で複数のアニメーションファイルをロード
        /// </summary>
        /// <param name="animationPaths">ロードするアニメーションのパスリスト</param>
        private async UniTask LoadAnimationsAsync(int originId,List<string> animationPaths)
        {
            anim.Clear();
            var tasks = new List<UniTask<AnimationClip>>();

            // 全てのパスについてロードを開始
            foreach (var path in animationPaths)
            {
                Debug.Log($"アニメーションロード開始: {path}");
                var task = AssetManager.Instance.LoadAssetAsync<AnimationClip>(AssetPath.GetLive2DMotion(originId, path));
                tasks.Add(task);
            }

            // 全てのロードが完了するのを待つ
            AnimationClip[] loadedClips = await UniTask.WhenAll(tasks);

            // 成功したアニメーションをリストに追加
            foreach (var clip in loadedClips)
            {
                if (clip != null)
                {
                    anim.Add(clip);
                    Debug.Log($"ロード成功: {clip.name}");
                }
                else
                {
                    Debug.LogWarning("アニメーションのロードに失敗しました。");
                }
            }

            CurrentAnimationLength = anim[0].length;
            Debug.Log($"全てのアニメーションロード完了: {anim.Count}個 読み込みました。");
        }

        private IEnumerator FadeIn()
        {
            float duration = 0.5f; // フェードインの時間（秒）
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                m_RenderController.Opacity = Mathf.Clamp01(elapsedTime / duration);
                yield return null;
            }

            m_RenderController.Opacity = 1f; // 最後に1に設定
                                             
            isInitialized = true;// 初期化完了フラグをオン
            Debug.Log("フェードイン完了しました。");
        }
    }
}
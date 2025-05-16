using fantec.Utilities;
using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;

namespace fantec.Common
{
    public enum SceneIndex
    {
        EMPTY,
        FIREST_LOAD,
        CAUTION,
        LOGO,
        TITLE,
        MENU,
        BATTLE,    // インゲーム (戦闘ステージ)
        ADVENTURE, // 会話パート

        DEBUG_TASK_KILL,
        DEBUG_PLAYFAB,
        DEBUG_LOCAL_MASTER,
        DEBUG_BATTLE_SETUP,
    }
    public class ExSceneManager : PersistentSingleton<ExSceneManager>
    {
        private static readonly Dictionary<SceneIndex, string> m_SceneNameDictionary = new Dictionary<SceneIndex, string>()
        {
            {SceneIndex.EMPTY,"Empty" },
            {SceneIndex.FIREST_LOAD,"FirestLoad" },
            {SceneIndex.LOGO,"Logo" },
            { SceneIndex.CAUTION,"Caution"},
            {SceneIndex.TITLE,"Title" },
            {SceneIndex.MENU,"Menu" },
            {SceneIndex.BATTLE,"Battle" },
            {SceneIndex.ADVENTURE,"Adventure" },

            {SceneIndex.DEBUG_TASK_KILL,"Debug_TaskKill" },
            {SceneIndex.DEBUG_PLAYFAB,"Debug_PlayFab" },
            {SceneIndex.DEBUG_LOCAL_MASTER, "Debug_LocalMaster" },
            {SceneIndex.DEBUG_BATTLE_SETUP, "Debug_BattleSetup" },
        };

        public IObservable<Unit> OnLoadCompleted => m_LoadCompletedSubject;
        private readonly Subject<Unit>m_LoadCompletedSubject= new Subject<Unit>();

        protected override void Awake()
        {
            base.Awake();

            SceneManager.sceneLoaded +=OnScreenLoaded;
        }

        void OnScreenLoaded(Scene scene,LoadSceneMode mode)
        {
            m_LoadCompletedSubject.OnNext(Unit.Default);
        }

        /// <summary>
        /// シーンをロードする
        /// </summary>
        public void LoadScene(SceneIndex scene,LoadSceneMode mode=LoadSceneMode.Single)
        {
            LoadScene(scene).Forget();
        }
        public async UniTask LoadSceneAsync(SceneIndex scene,LoadSceneMode mode)
        {
            await SceneManager.LoadSceneAsync(m_SceneNameDictionary[scene],mode);
        }

        /// <summary>
        /// 空のシーンを読み込み、シーンをロードする
        /// </summary>
        /// <param name="scene"></param>
        /// <returns></returns>
        private async UniTask LoadScene(SceneIndex scene)
        {
            SceneIndex prevScene =GetCurrentScene();

            //空のシーンを読み込む
            await LoadSceneAsync(SceneIndex.EMPTY,LoadSceneMode.Additive);

            //遷移前のシーンを破棄
            await UnloadSceneAsync(prevScene);

            //遷移先のシーンを読み込む
            await LoadSceneAsync(scene,LoadSceneMode.Single);
        }

        /// <summary>
        /// シーンをアンロードする
        /// </summary>
        public async UniTask UnloadSceneAsync(SceneIndex scene)
        {
            await SceneManager.UnloadSceneAsync(m_SceneNameDictionary[scene]);
        }

        /// <summary>
        /// 現在のシーンを取得する
        /// </summary>
        public static SceneIndex  GetCurrentScene()
        {
            foreach(SceneIndex scene in m_SceneNameDictionary.Keys)
            {
                if (m_SceneNameDictionary[scene]==SceneManager.GetActiveScene().name)
                {
                    return scene;
                }
            }

            Debug.LogError(SceneManager.GetActiveScene().name+" : シーン名が一致しませんでした。");
            return SceneIndex.TITLE;
        }

        public static T GetRootComponent<T>()=>GetRootComponent<T>(GetCurrentScene());
        public static T GetRootComponent<T>(SceneIndex sceneIndex) => GetRootComponent<T>(m_SceneNameDictionary[sceneIndex]);
        public static T GetRootComponent<T>(string sceneName) => GetRootComponent<T>(SceneManager.GetSceneByName(sceneName));
        public static T GetRootComponent<T>(Scene scene)
        {
            if(scene.IsValid()==true)
            {
                foreach(var rootObject in scene.GetRootGameObjects())
                {
                    var target=rootObject.GetComponent<T>();
                    if(target !=null)
                    {
                        return target;
                    }
                }
                Debug.LogError($"[scene:{scene.name}] に [component : {typeof(T).FullName}] をアタッチしたオブジェクトが存在しません。");
                return default;
            }
            Debug.LogError($"[scene : {scene.name}] が読み込まれていません。");
            return default;
        }
    }
}
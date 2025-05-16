using Cysharp.Threading.Tasks;
using fantec.Common;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace fantec.Debugger
{
    public partial class DebuggerPresenter : MonoBehaviour
    {
        [SerializeField] private DebuggerItemPresenter m_ItemPresenter;
        [SerializeField] private DebuggerView m_View;
        private Dictionary<string, DebuggerItemPresenter> m_CreateList=new Dictionary<string, DebuggerItemPresenter>();

        private void Start()
        {
            this.gameObject.SetActive(false);

            m_ItemPresenter.gameObject.SetActive(false);

            ExSceneManager.Instance.OnLoadCompleted.Subscribe(_ =>
            {
                LoadItem();    
            }).AddTo(this);
            LoadItem();
            LoadItemCommon();
        }

        private void LoadItem()
        {
            foreach(var item in m_CreateList.Values)
            {
                if (item) Destroy(item.gameObject);
            }
            m_CreateList = new Dictionary<string, DebuggerItemPresenter>();

            // シーンに応じたメニューの生成
            switch(ExSceneManager.GetCurrentScene())
            {
                case SceneIndex.FIREST_LOAD:
                    LoadItemFirestLoad();
                    break;
                case SceneIndex.TITLE:
                    LoadItemTitle();
                    break;
                case SceneIndex.MENU:
                    break;
                case SceneIndex.BATTLE:
                    LoadItemBattle();
                    break;
            }
        }

        private void Create(string title,DebuggerItemData data)
        {
            // 削除済みであれば破棄
            if(m_CreateList.ContainsKey(title))
            {
                Destroy(m_CreateList[title].gameObject);
                m_CreateList.Remove(title);
            }

            // 新しく作成
            data.titleText = title;
            var item=m_ItemPresenter.Create(data) as DebuggerItemPresenter;
            m_CreateList.Add(title, item);
        }

        private void LoadItemCommon()
        {
            m_ItemPresenter.Create(new DebuggerItemData()
            {
                titleText="シーン変更",
                inItemDatas=new DebuggerInItemData[]
                {
                    new DebuggerInItemData(){titleText="タイトルシーン",onClick=()=>ExSceneManager.Instance.LoadScene(SceneIndex.TITLE)},
                    new DebuggerInItemData(){titleText="メニューシーン",onClick=()=>ExSceneManager.Instance.LoadScene(SceneIndex.MENU)},
                    new DebuggerInItemData(){titleText="バトルセットアップ",onClick=()=>ExSceneManager.Instance.LoadScene(SceneIndex.DEBUG_BATTLE_SETUP)},
                 //   new DebuggerInItemData(){titleText="DebugPlayFab",onClick=()=>ExSceneManager.Instance.LoadScene(SceneIndex.TITLE)},
                  //  new DebuggerInItemData(){titleText="シナリオサンプル",onClick=()=>ExSceneManager.Instance.LoadScene(SceneIndex.TITLE)},
                }
            });

            m_ItemPresenter.Create(new DebuggerItemData()
            {
                titleText="システム",
                inItemDatas=new DebuggerInItemData[]
                {
                    new DebuggerInItemData(){titleText="データ初期化",onClick=()=>
                    {
                        PlayerPrefs.DeleteAll();
                        ExSceneManager.Instance.LoadScene(SceneIndex.DEBUG_TASK_KILL);
                    }},
                }
            });

            m_ItemPresenter.Create(new DebuggerItemData()
            {
                titleText="SEテスト",
                inItemDatas=new DebuggerInItemData[]
                {
                    new DebuggerInItemData(){titleText="Layerd",onClick=()=>
                    {
                        SEManager.Instance.Play(SEClipName.SystemTapScreen,SEPlayType.LAYERED);
                    }},

                    new DebuggerInItemData(){titleText="Override",onClick=()=>
                    {
                        SEManager.Instance.Play(SEClipName.SystemTapScreen,SEPlayType.OVERRIDE);
                    }},

                    new DebuggerInItemData(){titleText="Canceld",onClick=()=>
                    {
                        SEManager.Instance.Play(SEClipName.SystemTapScreen,SEPlayType.CANCELD);
                    }},
                }
            });
        }
    }
}
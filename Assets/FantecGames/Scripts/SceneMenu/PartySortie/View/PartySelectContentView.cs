using Cysharp.Threading.Tasks;
using fantec.Menu.Manager;
using fantec.Menu.PartySortie.Presenter;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEngine.UI;
using System.Threading;

namespace fantec.Menu.PartySelect.View
{
    public class PartySelectContentView : MonoBehaviour
    {
        [SerializeField]
        private ScrollRect m_EnemyScrollView;

        [SerializeField]
        private ScrollRect m_RewardScrollView;

        [SerializeField]
        private ScrollRect m_ItemScrollView;

        [SerializeField]
        private EnemyInfoCell m_EnemyInfoCell;
        
        [SerializeField]
        private RewardInfoCell m_RewardInfoCell;

        [SerializeField]
        private ItemInfoCell m_ItemInfoCell;


        /// <summary>
        /// 表示内容を一括で変更する
        /// </summary>
        private void ChangeContentView(PartySelectPresenter.InfoType type)
        {
            m_EnemyScrollView.gameObject.SetActive(type == PartySelectPresenter.InfoType.Enemy);
            m_RewardScrollView.gameObject.SetActive(type == PartySelectPresenter.InfoType.Reward);
            m_ItemScrollView.gameObject.SetActive(type == PartySelectPresenter.InfoType.Item);
        }

        // 初期化時のセットアップ
        public async UniTask Setup(Dictionary<int,List<int>>enemyList,List<int>dropRewardList,List<ItemInfoCell.Data>itemList)
        {
            // 敵情報
            foreach (int key in enemyList.Keys)
            {
                var prefab = Instantiate(m_EnemyInfoCell, m_EnemyScrollView.content);

                EnemyInfoCell.Data data = new EnemyInfoCell.Data($"WAVE {key + 1}", enemyList[key]);

                // 初期化時にしか呼ばれないため、キャンセル制御は不要なのでNoneを引数に渡す
                await prefab.Setup(data, CancellationToken.None);
            }

            // 報酬 
            foreach(var itemId in dropRewardList)
            {
                RewardInfoCell prefab=Instantiate(m_RewardInfoCell, m_RewardScrollView.content);
                await prefab.Setup(itemId, CancellationToken.None);
                prefab.OnClickDetailButtonObservable.Subscribe(_=>OnClickItemDetailButton(itemId)).AddTo(this);
            }

            // アイテム
            foreach(var itemData in itemList)
            {
                ItemInfoCell prefab = Instantiate(m_ItemInfoCell, m_ItemScrollView.content);
                await prefab.Setup(itemData, CancellationToken.None);
                int itemId = itemData.itemId;
                prefab.OnClickItemDetailButtonObservable.Subscribe(_=>OnClickStaminaItemDetailButton(itemId)).AddTo(this);
            }
        }

        /// <summary>
        /// 敵情報を表示する
        /// </summary>
        public void ShowEnemyInfoContentView()
        {
            ChangeContentView(PartySelectPresenter.InfoType.Enemy);
        }

        /// <summary>
        /// 報酬情報を表示する
        /// </summary>
        public void ShowRewardInfoContentView()
        {
            ChangeContentView(PartySelectPresenter.InfoType.Reward);
        }

        /// <summary>
        /// アイテム情報を表示する
        /// </summary>
        public void ShowItemInfoContentView()
        {
            ChangeContentView(PartySelectPresenter.InfoType.Item);
        }

        /// <summary>
        /// アイテム詳細情報ボタン押下時
        /// </summary>
        /// <param name="itemId"></param>
        private void OnClickItemDetailButton(int itemId)
        {
            MenuManager.Instance.ItemDetailId = itemId;
           // MenuWindowManager.Instance.Create(MenuWindowManager.CreateType.ItemDetail);
            MenuWindowManager.Instance.Create(MenuWindowManager.CreateType.NoContents);
        }

        /// <summary>
        /// スタミナ回復アイテムの詳細情報ボタン押下時
        /// </summary>
        /// <param name="itemId"></param>
        private void OnClickStaminaItemDetailButton(int itemId)
        {
            MenuManager.Instance.ItemDetailId = itemId;
            MenuWindowManager.Instance.Create(MenuWindowManager.CreateType.StaminaItemDetail);
        }
    }
}
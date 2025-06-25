using Cysharp.Threading.Tasks;
using fantec.Common;
using fantec.Master;
using fantec.Menu.Manager;
using fantec.Notice.View;
using fantec.PlayFabClient;
using System;
using System.Linq;
using System.Threading;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace fantec.Menu.Notice
{
    public class NoticeEntryCell : MonoBehaviour
    {
        [SerializeField]
        private Text m_TitleText;                     // セルに表示させるタイトル見出し
        [SerializeField]
        private Text m_TitleType;                     // イベント：　緊急：　運営：　など決めれる
        [SerializeField]
        private Text m_TitleMainText;                 // お知らせを表示したヘッダータイトル
        [SerializeField]
        private Text m_BodyText;                      // お知らせ本文
        [SerializeField]
        private Text m_TitleHeaderText;               // お知らせの見出し
        [SerializeField]
        private Text m_ScheduleStartText;             // お知らせの掲載日

        [SerializeField]
        private Button m_ReceiveButton;               // プレゼントがあった場合の受け取りボタン

        [SerializeField]
        private Text m_BtnText;                       // 受け取り：受け取り済み　と記載するテキスト

        [SerializeField]
        private Button m_CellButton;                  // セルを押したときに動作するボタン
        [SerializeField]
        private RewardNoticeCell m_RewardNoticeCell;  // 受け取れるアイテムを表示するプレハブ
        [SerializeField]
        private ScrollRect m_RewardScrollView;        // プレハブの親にするScrollRect

        public IObservable<Unit> OnClickCellButtonObservable => m_CellButton.OnClickAsObservable();
        public IObservable<Unit>OnClickReceiveButtonObservable=>m_ReceiveButton.OnClickAsObservable();
        public async UniTask Setup(NoticeData data, Text m_Body, Button receive,
            Text Header,Text Schedule,RewardNoticeCell Prefab,ScrollRect view,Text btnText)
        {
            m_TitleText.text = data.title;
            // お知らせ本文GameObjectに参照を紐づける
            m_BodyText = m_Body;            // お知らせ本文
            m_ReceiveButton = receive;      // 受け取りボタン
            m_TitleHeaderText = Header;     // タイトルの見出し
            m_ScheduleStartText = Schedule; // お知らせの掲載日
            m_RewardNoticeCell = Prefab;    // アイテム表示のプレハブ
            m_RewardScrollView = view;      // プレハブの親にするScrollRect
            m_BtnText = btnText;            // 受け取りボタンのテキスト


            // セル自体が押されたらそのお知らせ内容を表示させる
            OnClickCellButtonObservable.Subscribe(async _ =>
            {
                await ShowNotice(data);
            }).AddTo(this);

            // 受け取りボタンが押されたら、そのアイテムの取得処理を走らせる
            OnClickReceiveButtonObservable.Subscribe(async _ =>
            {
                await StoreManager.ClaimNoticeRewardAsync(data.key);
            }).AddTo(this);
        }

        public async UniTask ShowNotice(NoticeData notice)
        {
            // 本文を表示
            m_BodyText.text = notice.body;
            // 本文の見出し
            m_TitleHeaderText.text=notice.title;
            // 掲載日
            m_ScheduleStartText.text = MenuManager.Instance.GetJSTScheduleTime(notice.ScheduledStartDate);

            // アイテム画像を表示（複数表示させる場合はバンドル登録させておき、rewardItemIdsにはバンドルIDを入れる）
            if (notice.rewardItemIds != null && notice.rewardItemIds.Count > 0)
            {
                string itemId = notice.rewardItemIds[0];

                var catalog = MasterDataManager.Instance.Catalogs;

                var item = catalog.Result.Catalog.FirstOrDefault(i => i.ItemId == itemId);

                if (item != null)
                {
                    // 数値IDに変換可能かをチェック
                    if (int.TryParse(itemId, out int parsedItemId))
                    {
                        // Instantiateの作成処理
                        RewardNoticeCell prefab = Instantiate(m_RewardNoticeCell, m_RewardScrollView.content);
                        await prefab.Setup(item, parsedItemId,CancellationToken.None);
                        prefab.OnClickDetailButtonObservable.Subscribe(_=>OnClickItemDetailButton(parsedItemId)).AddTo(this);
                    }
                    else
                    {
                        if (item.ItemClass == "Bundle")
                        {      
                            var catalogItems = MasterDataManager.Instance.Catalogs.Result.Catalog;
 
                            foreach (var itemData in item.Bundle.BundledItems) // バンドルの中のアイテムがカタログに存在しているか見る
                            {
                                var catalogItem = catalogItems.FirstOrDefault(ci => ci.ItemId == itemData);

                                if (catalogItem != null)
                                {
                                    if (int.TryParse(catalogItem.ItemId, out int parsedItemDataId))
                                    {
                                        // カタログの中に該当アイテムが存在し、int型にパースできたらprefabの作成
                                        RewardNoticeCell prefab = Instantiate(m_RewardNoticeCell, m_RewardScrollView.content);
                                        await prefab.Setup(catalogItem, parsedItemDataId, CancellationToken.None);
                                        prefab.OnClickDetailButtonObservable.Subscribe(_=>OnClickItemDetailButton(parsedItemDataId)).AddTo(this);
                                    }
                                }
                            }
                        }
                        else
                        {
                            Debug.LogWarning($"ItemId '{itemId}' は数値IDではないため、画像読み込みをスキップします。");
                        }
                    }
                }
            }
            else
            {
                // 画像を消す処理
                m_ReceiveButton.gameObject.SetActive(false);
            }

            // ボタン設定
            m_ReceiveButton.interactable=!UserDataManager.User.ClaimedNoticeDictionary.ContainsKey(notice.key);
            if (!m_ReceiveButton.interactable)
            {
                m_BtnText.text = "受け取り済み";
            }
            else
            {
                m_BtnText.text = "受け取り";
            }
            m_ReceiveButton.onClick.RemoveAllListeners();
            m_ReceiveButton.onClick.AddListener(async () =>
            {
                 await StoreManager.ClaimNoticeRewardAsync(notice.key);
                m_ReceiveButton.interactable = false;
                m_BtnText.text = "受け取り済み";
            });
        }

        // アイテムセルを押した時の挙動
        private void OnClickItemDetailButton(int itemId)
        {
            MenuManager.Instance.ItemDetailId = itemId;
            MenuWindowManager.Instance.Create(MenuWindowManager.CreateType.NoContents);
        }
    }
}
using Cysharp.Threading.Tasks;
using fantec.Common;
using fantec.PlayFabClient;
using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace fantec.Menu.Notice
{
    public class NoticeEntryCell : MonoBehaviour
    {
        [SerializeField]
        private Text m_TitleText; // セルに表示させるタイトル見出し
        [SerializeField]
        private Text m_TitleType; // イベント：　緊急：　運営：　など決めれる
        [SerializeField]
        private Text m_TitleMainText; // お知らせを表示したヘッダータイトル
        [SerializeField]
        private Text m_BodyText; // お知らせ本文
        [SerializeField]
        private Sprite m_ItemSprite; // お知らせにプレゼント付きであった場合、そのプレゼント内容の表示

        [SerializeField]
        private Button m_ReceiveButton; // プレゼントがあった場合の受け取りボタン

        [SerializeField]
        private Button m_CellButton; // セルを押したときに動作するボタン

        public IObservable<Unit> OnClickCellButtonObservable => m_CellButton.OnClickAsObservable();
        public IObservable<Unit>OnClickReceiveButtonObservable=>m_ReceiveButton.OnClickAsObservable();
        public async UniTask Setup(NoticeData data,Text m_Body,Button receive)
        {
            m_TitleText.text = data.title;
            // お知らせ本文GameObjectに参照を紐づける
            m_BodyText = m_Body;
            m_ReceiveButton = receive;

            // セル自体が押されたらそのお知らせ内容を表示させる
            OnClickCellButtonObservable.Subscribe(async _=>
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

            // アイテム画像を表示（例: 1個目だけ）
            if (notice.rewardItemIds != null && notice.rewardItemIds.Length > 0)
            {
                string itemId = notice.rewardItemIds[0];
                var catalog = await PlayFabClientAPI.GetCatalogItemsAsync(new GetCatalogItemsRequest { CatalogVersion = "Main" });
                var item = catalog.Result.Catalog.FirstOrDefault(i => i.ItemId == itemId);
                int GetitemId = int.Parse(notice.rewardItemIds[0]);


                if (item != null)
                {
                    // ItemClass が "Card" の場合は専用の画像取得処理
                    if (item.ItemClass == "Card")
                    {
                        Debug.Log("これはカードです: " + item.ItemId);

                        if (!string.IsNullOrEmpty(item.ItemImageUrl))
                        {
                            m_ItemSprite = await AssetManager.Instance.LoadAssetAsync<Sprite>(AssetPath.GetCharacterSpriteSpherePath(GetitemId), System.Threading.CancellationToken.None);
                        }
                    }
                    else
                    {
                        // それ以外のItemClassの場合は普通のアイテム画像取得処理
                        Debug.Log("カード以外のアイテム: " + item.ItemId + " / ItemClass: " + item.ItemClass);
                        m_ItemSprite = await AssetManager.Instance.LoadAssetAsync<Sprite>(AssetPath.GetSpriteItemIcon(GetitemId), System.Threading.CancellationToken.None);
                    }
                }
            }
            else
            {
                // 画像を消す処理
            }

            // ボタン設定
            m_ReceiveButton.gameObject.SetActive(notice.canReceive);
            m_ReceiveButton.onClick.RemoveAllListeners();
            m_ReceiveButton.onClick.AddListener(async () =>
            {
                var res = await StoreManager.ClaimNoticeRewardAsync(notice.key);
                Debug.Log($"報酬の状態{res?.status}");// 受け取ったか、受け取っていないか
            });
        }
    }
}
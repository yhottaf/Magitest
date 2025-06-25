using Cysharp.Threading.Tasks;
using fantec.Common;
using System.Threading;
using System;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using PlayFab.ClientModels;

namespace fantec.Notice.View
{
    public class RewardNoticeCell : MonoBehaviour
    {
        [SerializeField]
        private Image m_ItemImage;

        [SerializeField]
        private Button m_DetailButton;

        public IObservable<Unit> OnClickDetailButtonObservable => m_DetailButton.OnClickAsObservable();

        public async UniTask Setup(CatalogItem item,int itemId ,CancellationToken cts)
        {
            if (itemId >= 0)
            {
                if (item.ItemClass == "Card")
                {
                    int originId = MasterDataManager.Instance.PlayerCardMaster.GetData(itemId).originId;

                    m_ItemImage.sprite = await AssetManager.Instance.LoadAssetAsync<Sprite>(AssetPath.GetCharacterSpriteSpherePath(originId), cts);
                }
                else
                {
                    m_ItemImage.sprite = await AssetManager.Instance.LoadAssetAsync<Sprite>(AssetPath.GetSpriteItemIcon(itemId), cts);
                }

                gameObject.SetActive(true);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }
}
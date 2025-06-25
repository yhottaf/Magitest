using Cysharp.Threading.Tasks;
using fantec.Common;
using System;
using System.Threading;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace fantec.Menu.PartySelect.View
{
    public class RewardInfoCell : MonoBehaviour
    {
        [SerializeField]
        private Image m_ItemImage;

        [SerializeField]
        private Button m_DetailButton;

        public IObservable<Unit> OnClickDetailButtonObservable => m_DetailButton.OnClickAsObservable();

        public async UniTask Setup(int itemId, CancellationToken cts)
        {
            if(itemId>=0)
            {
                m_ItemImage.sprite = await AssetManager.Instance.LoadAssetAsync<Sprite>(AssetPath.GetSpriteItemIcon(itemId),cts);
                gameObject.SetActive(true);
            }
            else
            {
                gameObject.SetActive(false);
            }

        }
    }
}
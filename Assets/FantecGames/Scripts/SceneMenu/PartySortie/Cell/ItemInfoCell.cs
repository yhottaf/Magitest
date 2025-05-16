using UnityEngine;
using UniRx;
using fantec.Common;
using UnityEngine.UI;
using System;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace fantec.Menu.PartySelect.View
{
    public class ItemInfoCell : MonoBehaviour
    {
        [SerializeField]
        private Image m_ItemImage;

        [SerializeField]
        private Text m_ItemNumText;

        [SerializeField]
        private Button m_DetailButton;

        public IObservable<Unit> OnClickItemDetailButtonObservable => m_DetailButton.OnClickAsObservable();

        public async UniTask Setup(Data data, CancellationToken cts)
        {
            m_ItemImage.sprite = await AssetManager.Instance.LoadAssetAsync<Sprite>(AssetPath.GetSpriteItemIcon(data.itemId),cts);
            m_ItemNumText.text = data.itemNum.ToString();
        }

        public struct Data
        {
            public int itemId;
            public int itemNum;
        }
    }
}
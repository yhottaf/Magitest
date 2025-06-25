using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace fantec.Battle.Ui.Window
{
    public class WinResultDropItemCell : MonoBehaviour
    {
        [SerializeField] private Image m_IconImage;
        [SerializeField] private Image m_FlashImage;
        [SerializeField] private Image m_LabelImage;
        [SerializeField] private Text m_CountText;

        public void SetIconImage(Sprite sprite)
        {
            m_IconImage.sprite = sprite;
        }

        public void SetCountText(string text)
        {
            m_CountText.text = text;
        }

        public void SetLabelSprite(Sprite sprite)
        {
            m_LabelImage.gameObject.SetActive(true);
            m_LabelImage.sprite = sprite;
        }

        public void PlayFlash()
        {
            m_FlashImage.DOFade(0, 0.3f);
        }
    }
}
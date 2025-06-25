using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace fantec.Common
{
    public class LoadingView : OverlayObject
    {
        //[SerializeField]
        //private Animator m_Animator;

        [SerializeField]
        private CanvasGroup m_CanvasGroup;

        [SerializeField]
        private Image m_BackgroundImage;

        [SerializeField]
        private Image m_CharacterImage1;

        [SerializeField]
        private Image m_CharacterImage2;

        [SerializeField]
        private CharacterSprite[] m_CharacterSprites;

        public void Show(float duration, float alpha, System.Action onFinish)
        {
            SetChracterSprite();

            //m_Animator.Play("Loading");

            m_BackgroundImage.DOFade(alpha, 0f);

            gameObject.transform.SetAsLastSibling();
            m_CanvasGroup.alpha = 0f;
            m_CanvasGroup.gameObject.SetActive(true);
            m_CanvasGroup.DOFade(1f, duration).OnComplete(() => { onFinish?.Invoke(); });
        }

        public void Hide(float duration, System.Action onFinish)
        {
            gameObject.transform.SetAsLastSibling();
            m_CanvasGroup.alpha = 1f;
            m_CanvasGroup.DOFade(0f, duration).OnComplete(() =>
            {
                gameObject.SetActive(false);
                //m_Animator.StopPlayback();
                onFinish?.Invoke();
            });
        }

        /// <summary>
        /// ランダムでキャラクター画像をセットする
        /// </summary>
        private void SetChracterSprite()
        {
            int index = Random.Range(0, m_CharacterSprites.Length);
            m_CharacterImage1.sprite = m_CharacterSprites[index].m_Sprite1;
            m_CharacterImage2.sprite = m_CharacterSprites[index].m_Sprite2;
        }

        [System.Serializable]
        private class CharacterSprite
        {
            public Sprite m_Sprite1;
            public Sprite m_Sprite2;
        }
    }
}
using System;
using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;

namespace fantec.Common
{
    public class FadeView:OverlayObject
    {
        [SerializeField] private CanvasGroup m_CanvasGroup;

        /// <summary>
        /// 徐々に画像を透過させていく
        /// </summary>
        /// <param name="duration"></param>
        /// <param name="onFinish"></param>
        public void FadeIn(float duration = 1,Action onFinish=null)
        {
            this.transform.SetAsLastSibling();
            m_CanvasGroup.alpha = 1;
            m_CanvasGroup.DOFade(0, duration).OnComplete(() =>
            {
                m_CanvasGroup.gameObject.SetActive(false);
                onFinish?.Invoke();
            });
        }

        /// <summary>
        /// 非同期でフェードイン
        /// </summary>
        public async UniTask FadeInAsync(float duration = 1)
        {
            this.transform.SetAsFirstSibling();
            m_CanvasGroup.alpha = 1;
            await m_CanvasGroup.DOFade(0, duration).AsyncWaitForCompletion();
            m_CanvasGroup.gameObject.SetActive(false);
        }

        /// <summary>
        /// 徐々に画像を画像で覆わせる
        /// </summary>
        /// <param name="duration">フェードにかける時間</param>
        /// <param name="onFinish">フェード完了後のアクション</param>
        public void FadeOut(float duration=1,Action onFinish=null)
        {
            this.transform.SetAsLastSibling();
            m_CanvasGroup.alpha = 0;
            m_CanvasGroup.gameObject.SetActive(true);
            m_CanvasGroup.DOFade(1, duration).OnComplete(()=>onFinish?.Invoke());
        }

        /// <summary>
        /// 非同期でフェードアウト
        /// </summary>
        public async UniTask FadeOutAsync(float duration=1)
        {
            this.transform.SetAsLastSibling();
            m_CanvasGroup.alpha = 0;
            m_CanvasGroup.gameObject.SetActive(true);
            await m_CanvasGroup.DOFade(1, duration).AsyncWaitForCompletion();
        }
    }
}
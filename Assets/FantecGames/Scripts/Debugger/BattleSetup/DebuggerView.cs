using DG.Tweening;
using UnityEngine;

namespace fantec.Debugger
{
    public class DebuggerView : MonoBehaviour
    {
        [SerializeField] private CanvasGroup m_CanvasGroup;

        public void PlayShowTime(float showTime,System.Action onShow)
        {
            DOTween.Sequence()
                .OnStart(() => m_CanvasGroup.interactable = false)
                .Append(m_CanvasGroup.DOFade(0.0f, 0.2f))
                .AppendCallback(() => onShow())
                .AppendInterval(showTime)
                .Append(m_CanvasGroup.DOFade(1.0f, 0.2f))
                .OnComplete(() => m_CanvasGroup.interactable = true)
                .SetLink(this.gameObject);
        }
    }
}
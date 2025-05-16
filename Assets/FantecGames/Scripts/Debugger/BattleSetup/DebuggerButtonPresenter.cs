using Cysharp.Threading.Tasks;
using UnityEngine;
using UniRx;

namespace fantec.Debugger
{
    public class DebuggerButtonPresenter : MonoBehaviour
    {
        [SerializeField] private DebuggerButtonView m_View;
        [SerializeField] private RectTransform m_ContentRect;

        private bool isEnable;

        private void Awake()
        {
            m_View.PlayHide();

            m_View.OnClickObservable.Subscribe(_ =>
            {
                isEnable = !isEnable;
                if(isEnable)
                {
                    m_View.PlayShow();
                    m_ContentRect.gameObject.SetActive(true);
                }
                else
                {
                    m_View.PlayHide();
                    m_ContentRect.gameObject.SetActive(false);
                }
            }).AddTo(this);
        }
    }
}
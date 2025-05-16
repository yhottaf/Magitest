using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace fantec.Debugger
{
    public class DebuggerInItemView : MonoBehaviour
    {
        [SerializeField] private Text m_TitleText;
        [SerializeField] private Button m_Button;

        public IObservable<Unit> OnClickObservable => m_Button.OnClickAsObservable();

        public void SetTitleText(string text)
        {
            m_TitleText.text = text;
        }
    }
}
using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace fantec.Debugger
{
    public class DebuggerButtonView : MonoBehaviour
    {
        [SerializeField] private Button m_Button;
        [SerializeField]private Text m_Text;

        public IObservable<Unit> OnClickObservable => m_Button.OnClickAsObservable();

        public void PlayHide()
        {
            m_Text.text = "DEBUG";
        }

        public void PlayShow()
        {
            m_Text.text = "CLOSE";
        }
    }
}
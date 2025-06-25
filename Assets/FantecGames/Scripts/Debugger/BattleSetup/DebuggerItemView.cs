using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace fantec.Debugger
{
    public class DebuggerItemView : MonoBehaviour
    {
        [SerializeField] private Text m_TitleText;
        [SerializeField] private Toggle m_Toggle;

        public IObservable<bool> OnValueChangedToggleObservable => m_Toggle.OnValueChangedAsObservable();

        public void SetTitleText(string titleText)
        {
            m_TitleText.text = titleText;
        }
    }
}
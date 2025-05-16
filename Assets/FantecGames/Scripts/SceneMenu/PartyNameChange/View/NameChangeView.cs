using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace fantec.Menu.NameChange.View
{
    public class NameChangeView : MonoBehaviour
    {
        [SerializeField]
        private Button m_CloseButton;

        [SerializeField]
        private Button m_SubmitButton;

        [SerializeField]
        private InputField m_KeywordInput;

        [SerializeField]
        private GameObject m_InfoText;

        [SerializeField]
        private Text m_PlaceHolderText;

        public IObservable<Unit> OnClickCloseButtonObservable => m_CloseButton.OnClickAsObservable();
        public IObservable<Unit>OnClickSubmitButtonObservable=>m_SubmitButton.OnClickAsObservable();

        public IObservable<string> OnClickKeywordInputObservable => m_KeywordInput.OnEndEditAsObservable();

        public void SetInfoText(bool value)
        {
            m_InfoText.SetActive(value);
        }

        public void SetPlaceholder(string name)
        {
            m_PlaceHolderText.text = $"{name}";
        }
    }
}
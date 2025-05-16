using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace fantec.Battle.Ui.Window
{
    public class RankUpPopupTextView : MonoBehaviour
    {
        [SerializeField] private Text m_OldRankText;
        [SerializeField] private Text m_NewRankText;
        [SerializeField] private Text m_OldStaminaText;
        [SerializeField] private Text m_NewStaminaText;
        [SerializeField] private Button m_CloseButton;

        public IObservable<Unit> OnClickCloseButton => m_CloseButton.OnClickAsObservable();

        public void SetRankText(int oldValue,int newValue)
        {
            m_OldRankText.text = oldValue.ToString();
            m_NewRankText.text = newValue.ToString();
        }

        public void SetStaminaText(int oldValue,int newValue)
        {
            m_OldStaminaText.text = oldValue.ToString();
            m_NewStaminaText.text = newValue.ToString();
        }
    }
}
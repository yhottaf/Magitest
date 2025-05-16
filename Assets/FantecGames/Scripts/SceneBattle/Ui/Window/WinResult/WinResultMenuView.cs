using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace fantec.Battle.Ui.Window
{
    public class WinResultMenuView : MonoBehaviour
    {
        [SerializeField] private Button m_RetryButton;
        [SerializeField] private Button m_NextButton;
        [SerializeField] private Text m_StaminaText;

        public IObservable<Unit> OnClickRematchObservable => m_RetryButton.OnClickAsObservable();

        public IObservable<Unit>OnClickNextObservable=>m_NextButton.OnClickAsObservable();

        public void SetInteractableRetryButton(bool enabled)=>m_RetryButton.interactable = enabled;
        public void SetInteractableNextButton(bool enabled)=>m_NextButton.interactable= enabled;

        public void SetStaminaText(int consumeStamina,int currentStamina)
        {
            var colorStr = consumeStamina > currentStamina ? "C90000" : "FFFFFF";
            m_StaminaText.text = $"<color=#{colorStr}>{consumeStamina}</color>/{currentStamina}";
        }
    }
}
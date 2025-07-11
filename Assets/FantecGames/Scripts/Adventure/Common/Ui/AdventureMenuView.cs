using System;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;

namespace fantec.Adventure
{
    public class AdventureMenuView : MonoBehaviour
    {
        public IObservable<bool> OnClickSkipObserveble => m_SkipToggle.OnValueChangedAsObservable();
        public IObservable<bool> OnClickAutoObservable => m_AutoToggle.OnValueChangedAsObservable();
        public IObservable<bool> OnClickLogObservable => m_LogToggle.OnValueChangedAsObservable();
        public IObservable<bool> OnClickMuteObservable => m_MuteToggle.OnValueChangedAsObservable();
        public IObservable<Unit> OnReleaseButtonObservable => m_PressingButton.OnPointerUpAsObservable()
                .TakeUntil(m_PressingButton.OnPointerExitAsObservable())
            .RepeatUntilDestroy(m_PressingButton)
            .AsUnitObservable();

        public IObservable<Unit> OnPressingButtonObservable => m_PressingButton.OnPressingAsObservable(0.5f);

        [SerializeField] private Toggle m_SkipToggle;
        [SerializeField] private Toggle m_AutoToggle;
        [SerializeField] private Toggle m_LogToggle;
        [SerializeField] private Toggle m_MuteToggle;
        [SerializeField] private GameObject m_AutoMark;
        [SerializeField] private Button m_PressingButton; //長押しボタン

        public void SetLogtoggleOff()
        {
            m_LogToggle.isOn = false;
        }

        public void SetAutoMark(bool value)
        {
            m_AutoMark.gameObject.SetActive(value);
        }
    }
}

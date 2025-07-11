using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace fantec.Adventure
{
    public class AdventureMenuPresenter : MonoBehaviour
    {
        [SerializeField] private AdvEngine m_Engine;
        [SerializeField] private AdventureMenuView m_View;
        [SerializeField] private GameObject m_Buttons;

        private void Awake()
        {
            m_View.OnClickAutoObservable.Subscribe(OnClickAuto).AddTo(this);
            m_View.OnClickSkipObserveble.Subscribe(OnClickSkip).AddTo(this);
            m_View.OnClickLogObservable.Subscribe(OnClickLog).AddTo(this);
            m_View.OnClickMuteObservable.Subscribe(OnClickMute).AddTo(this);
            m_View.OnPressingButtonObservable.Subscribe(_ => { OnClickSkip(true); }).AddTo(this);
            m_View.OnReleaseButtonObservable.Subscribe(_ => { OnClickSkip(false); }).AddTo(this);
        }

        private void LateUpdate()
        {
            m_Buttons.SetActive(
                m_Engine.UiManager.IsShowingMenuButton &&
                m_Engine.UiManager.Status == AdvUiManager.UiStatus.Default);
        }

        private void OnClickSkip(bool enabled)
        {
            m_Engine.Config.IsSkip = enabled;
        }

        private void OnClickAuto(bool enabled)
        {
            m_Engine.Config.IsAutoBrPage = enabled;
            m_View.SetAutoMark(enabled);
        }

        private void OnClickLog(bool enabled)
        {
            if (enabled)
            {
                m_Engine.UiManager.Status = AdvUiManager.UiStatus.Backlog;
            }
        }

        private void OnClickMute(bool enabled)
        {
            if (enabled)
            {
                SoundManager.GetInstance().MasterVolume = 0;
            }
            else
            {
                SoundManager.GetInstance().MasterVolume = 1;
            }
        }
    }
}

using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace fantec.Title
{
    public class TitleView : MonoBehaviour
    {
        [SerializeField]
        private Button m_TapButton;

        [SerializeField]
        private Text m_VersionText;

        [SerializeField]
        private Text m_PlayerIdText;
        [SerializeField]
        private GameObject m_LoadingView;
        [SerializeField]
        private GameObject m_UpdateView;

        //       [SerializeField]
        //        private Button m_DeleteUserDataButton;
        [SerializeField]
        private Slider m_ProgressBar;
        [SerializeField]
        private Text m_ProgressText;
        // [SerializeField]

        //   private Button m_RetryButton;
        [SerializeField]
        private Button m_UpdateButton;
        [SerializeField]
        private AudioClip m_AudioSource;
        public IObservable<Unit> OnClickTapButtonObservable => m_TapButton.OnClickAsObservable();
        public IObservable<Unit> OnClickUpdateButtonObservable => m_UpdateButton.OnClickAsObservable();
        //     public IObservable<Unit> OnClickDeleteUserDataBtnObservable => m_DeleteUserDataButton.OnClickAsObservable();
        //     public IObservable<Unit> OnClickRetryButtonObservable => m_RetryButton.OnClickAsObservable();
        public void UpdateVersionText(string str) { m_VersionText.text = str; }
        public void UpdatePlayerIdText(string str) { m_PlayerIdText.text = str; }
        public void UpdateProgressText(float progress, string text) { m_ProgressBar.value = progress; m_ProgressText.text = text; }
        //       public void ShowRetryButton() { m_RetryButton.gameObject.SetActive(true); }
        public void SetLoadingViewObj(bool enable) { m_LoadingView.SetActive(enable); }
        public void SetUpdateViewObj(bool enable) { m_UpdateView.SetActive(enable); }
        public AudioClip m_audiosource => m_AudioSource;

        public readonly string GooglePlayUrl;
        public readonly string AppStoreUrl;
    }
}
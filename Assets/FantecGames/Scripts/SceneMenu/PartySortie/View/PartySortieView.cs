using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace fantec.Menu.PartySortie.View
{
    public class PartySortieView : MonoBehaviour
    {
        [SerializeField]
        private Text m_QuestNameText;

        [SerializeField]
        private Text m_DirectoryNameText;

        [SerializeField]
        private Button m_CloseButton;

        [SerializeField]
        private Button m_BackButton;

        [SerializeField]
        private Button m_SubmitButton;
        [SerializeField]
        private Button m_EditButton;

        [SerializeField]
        private Button m_DirectoryNameChangeButton;
        [SerializeField]
        private Button m_RightButton;
        [SerializeField]
        private Button m_LeftButton;

        [SerializeField]
        private Toggle m_EnemyInfoToggle;

        [SerializeField]
        private Toggle m_RewardToggle;

        [SerializeField]
        private Toggle m_ItemInfoToggle;

        public IObservable<Unit> OnClickCloseButtonObservable => m_CloseButton.OnClickAsObservable();
        public IObservable<Unit> OnClickBackButtonObservable=>m_BackButton.OnClickAsObservable();
        public IObservable<Unit> OnClickSubmitButtonObservable => m_SubmitButton.OnClickAsObservable();
        public IObservable<Unit>OnClickEditButtonObservable=>m_EditButton.OnClickAsObservable();
        public IObservable<Unit>OnClickRightButtonObservable=>m_RightButton.OnClickAsObservable();
        public IObservable<Unit>OnClickLeftButtonObservable=>m_LeftButton.OnClickAsObservable();
        public IObservable<Unit> OnClickDirectoryNameChangeObservable => m_DirectoryNameChangeButton.OnClickAsObservable();
        public IObservable<bool> OnValueChangeEnemyInfoToggleObservable => m_EnemyInfoToggle.OnValueChangedAsObservable();
        public IObservable<bool>OnValueChangeRewardToggleObservable=>m_RewardToggle.OnValueChangedAsObservable();
        public IObservable<bool>OnValueChangedItemInfoToggleObservable=>m_ItemInfoToggle.OnValueChangedAsObservable();

        /// <summary>
        /// クエスト名を更新する
        /// </summary>
        /// <param name="questName"></param>
        public void UpdateQuestName(string questName)
        {
            m_QuestNameText.text = questName;
        }

        /// <summary>
        /// パーティ名を更新する
        /// </summary>
        /// <param name="name"></param>
        public void UpdateDirectoryName(string name)
        {
            m_DirectoryNameText.text = name;
        }
    }
}
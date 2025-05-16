using Cysharp.Threading.Tasks;
using fantec.Common;
using fantec.Menu.Common;
using fantec.Menu.Manager;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

namespace fantec.Menu
{
    public class PlayerNameController : MonoBehaviour
    {
        [SerializeField]
        private InputField m_InputField;

        [SerializeField]
        private Text m_placeholderText;

        [SerializeField]
        private Button m_SubmitButton;

        [SerializeField]
        private Text m_ErrorText;

        private string m_NameStr;

        private readonly int NameMinLimit = 3;  // PlayFabの最低文字数が3文字
        private readonly int NameLimit = 12;

        void Start()
        {
            m_SubmitButton.OnClickAsObservable().Subscribe(unit => OnClickSubmitButton().Forget()).AddTo(this);

            m_placeholderText.text = PlayFabClient.PlayerProfileManager.UserDisplayName;

            m_InputField.characterLimit = NameLimit;
            m_InputField.OnValueChangedAsObservable().Subscribe(OnValueChangedInputField).AddTo(this);
            m_InputField.OnEndEditAsObservable().Subscribe(OnEndEditInputField).AddTo(this);
        }

        /// <summary>
        /// 入力内容の変更時
        /// </summary>
        private void OnValueChangedInputField(string value)
        {
            m_SubmitButton.interactable = value.Length >= NameMinLimit;
        }

        /// <summary>
        /// 入力終了時
        /// </summary>
        private void OnEndEditInputField(string value)
        {
            m_NameStr = value;
            m_SubmitButton.interactable = value.Length >= NameMinLimit;
        }

        /// <summary>
        /// 決定ボタン押下時
        /// </summary>
        private async UniTask OnClickSubmitButton()
        {
            // SE再生
       //     SEManager.Instance.Play(SEClipName.SystemButtonDownYes);

            var result = await PlayFabClient.PlayerProfileManager.UpdateUserDisplayNameAsync(m_NameStr);

            if (result.isSuccess)
            {
                HeaderManager.Instance.UpdateUserNameText();
                MenuWindowManager.Instance.Remove(MenuWindowManager.CreateType.PlayerNameEdit);
            }
            else
            {
                m_ErrorText.gameObject.SetActive(true);
                m_ErrorText.text = result.errorMessage;
            }
        }
    }
}
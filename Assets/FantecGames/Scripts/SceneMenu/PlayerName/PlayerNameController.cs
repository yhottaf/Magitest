using Cysharp.Threading.Tasks;
using fantec.Common;
using fantec.Menu.Common;
using fantec.Menu.Manager;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using fantec.PlayFabClient;
using System;

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
        private readonly int Delaytime = 2;

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
            bool isTutorial = false;
            if (string.IsNullOrEmpty(PlayerProfileManager.UserDisplayName))
            {
                // 初回ダイアログ表示時
                isTutorial = true;
            }

                var result = await PlayerProfileManager.UpdateUserDisplayNameAsync(m_NameStr);

            if (result.isSuccess)
            {
                HeaderManager.Instance.UpdateUserNameText();
                MenuWindowManager.Instance.Remove(MenuWindowManager.CreateType.PlayerNameEdit);
                
                // 今回初めて名前入力を行ったなら、入力後にお知らせ表示を行うようにする
                if (isTutorial)
                {
                    MenuWindowManager.Instance.Create(MenuWindowManager.CreateType.InputGuard);

                    Observable.Timer(TimeSpan.FromSeconds(Delaytime))
                        .Subscribe(_ => MenuWindowManager.Instance.Create(MenuWindowManager.CreateType.Notice));
                }
            }
            else
            {
                m_ErrorText.gameObject.SetActive(true);
                m_ErrorText.text = result.errorMessage;
            }
        }
    }
}
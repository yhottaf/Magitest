using Cysharp.Threading.Tasks;
using fantec.Menu.Manager;
using fantec.Menu.NameChange.Controller;
using fantec.Menu.NameChange.View;
using UniRx;
using UnityEngine;

namespace fantec.Menu.NameChange.Presenter
{
    public class NameChangePresenter : MonoBehaviour
    {
        [SerializeField]
        private NameChangeView m_View;
        private string m_InputName;
        private const int NameLimit = 10;

        private void Start()
        {
            if(PlayerPrefsManager.GetPartyName(PlayerPrefsManager.SelectPartyIndex)!="")
            {
                m_View.SetPlaceholder(PlayerPrefsManager.GetPartyName(PlayerPrefsManager.SelectPartyIndex));
            }
            else
            {
                m_View.SetPlaceholder(string.Empty);
            }

            m_InputName = "";
            m_View.OnClickCloseButtonObservable.Subscribe(OnClickCloseButton).AddTo(this);
            m_View.OnClickSubmitButtonObservable.Subscribe(OnClickSubmitButton).AddTo(this);
            m_View.OnClickKeywordInputObservable.Subscribe(OnClickNameword).AddTo(this);
        }

        /// <summary>
        /// キーワード検索
        /// </summary>
        /// <param name="value"></param>
        private void OnClickNameword(string value)
        {
            if(value.Length<=NameLimit)
            {
                m_View.SetInfoText(false);
                m_InputName = value;
            }
            else if(value.Length>NameLimit)
            {
                m_InputName = value;
                m_View.SetInfoText(true);
            }
        }


        /// <summary>
        /// 閉じるボタン押下時
        /// </summary>
        /// <param name="unit"></param>
        private void OnClickCloseButton(Unit unit)
        {
            MenuWindowManager.Instance.Remove(MenuWindowManager.CreateType.NameChange);
        }

        /// <summary>
        /// 決定ボタン押下時
        /// </summary>
        /// <param name="unit"></param>
        private void OnClickSubmitButton(Unit unit)
        {
            if (m_InputName.Length > NameLimit)
            {
                return;
            }

            NameChangeController.Instance.m_DirectoryNameChange.OnNext(m_InputName);// 入力内容の通知
            MenuWindowManager.Instance.Remove(MenuWindowManager.CreateType.NameChange);
        }
    }
}
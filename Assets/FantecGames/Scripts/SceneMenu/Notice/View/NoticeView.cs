using UnityEngine;
using UnityEngine.UI;

namespace fantec.Menu.Notice.View
{
    public class NoticeView : MonoBehaviour
    {
        [SerializeField]
        private Button m_CloseButton; // お知らせの閉じるボタン
        [SerializeField]
        private Text m_NoticeBodyText; // お知らせ本文
        [SerializeField]
        private Button m_ReceiveButton; // 受け取りボタン
        
        public Text GetNoticeTextObj()
        {
            return m_NoticeBodyText;
        }

        public Button GetReceiveButton()
        {
            return m_ReceiveButton;
        }
    }
}
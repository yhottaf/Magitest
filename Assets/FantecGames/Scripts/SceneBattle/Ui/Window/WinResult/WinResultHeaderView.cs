using UnityEngine;
using UnityEngine.UI;

namespace fantec.Battle.Ui.Window
{
    public class WinResultHeaderView : MonoBehaviour
    {
        [SerializeField] private Text m_StageNameText;

        public void SetStageName(string text)
        {
            m_StageNameText.text = text;
        }
    }
}
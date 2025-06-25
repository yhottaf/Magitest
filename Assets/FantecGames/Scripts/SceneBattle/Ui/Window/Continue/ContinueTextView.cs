using UnityEngine;
using UnityEngine.UI;

namespace fantec.Battle.Ui.Window
{
    public class ContinueTextView : MonoBehaviour
    {
        [SerializeField] private Text m_OwnedText;

        public void SetText(int paid,int free)
        {
            m_OwnedText.text = $"所有している課金石：{ToCommaSeparated(paid+free)}個\n<size=33> (有償 {ToCommaSeparated(paid)} / 無償 {ToCommaSeparated(free)}) </size>";
        }

        private string ToCommaSeparated(int value)
        {
            return string.Format("{0:#,0}", value);
        }
    }
}
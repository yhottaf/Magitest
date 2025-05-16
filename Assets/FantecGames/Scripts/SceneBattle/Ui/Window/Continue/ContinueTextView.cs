using UnityEngine;
using UnityEngine.UI;

namespace fantec.Battle.Ui.Window
{
    public class ContinueTextView : MonoBehaviour
    {
        [SerializeField] private Text m_OwnedText;

        public void SetText(int paid,int free)
        {
            m_OwnedText.text = $"Š—L‚µ‚Ä‚¢‚é‰Û‹àÎF{ToCommaSeparated(paid+free)}ŒÂ\n<size=33> (—L {ToCommaSeparated(paid)} / –³ {ToCommaSeparated(free)}) </size>";
        }

        private string ToCommaSeparated(int value)
        {
            return string.Format("{0:#,0}", value);
        }
    }
}
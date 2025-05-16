using UnityEngine;
using UnityEngine.UI;

namespace fantec.Menu.PartyEdit
{
    public class ProgressDot : MonoBehaviour
    {
        [SerializeField]
        private Image m_SmallImage;

        [SerializeField]
        private Image m_BigImage;

        public void ChangeProgress(bool bProgress)
        {
            m_BigImage.gameObject.SetActive(bProgress);
            m_SmallImage.gameObject.SetActive(!bProgress);
        }
    }
}
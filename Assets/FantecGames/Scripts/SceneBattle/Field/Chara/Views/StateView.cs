using UnityEngine;
using UnityEngine.UI;

namespace fantec.Battle.Field.Chara
{
    public class StateView : MonoBehaviour,IInitializable
    {
        [SerializeField] private GameObject m_ViewGroup;
        [SerializeField] private Slider m_HpFillSlider;

        public void Initialize()
        {

        }

        public void SetActiveHpSlider(bool enabled)
        {
            m_HpFillSlider.gameObject.SetActive(enabled);
        }

        public void UpdateHpFill(float value)
        {
            m_HpFillSlider.value = value;
        }

        public void Show()
        {
            m_ViewGroup.SetActive(true);
        }

        public void Hide()
        {
            m_ViewGroup.SetActive(false);
        }
    }
}
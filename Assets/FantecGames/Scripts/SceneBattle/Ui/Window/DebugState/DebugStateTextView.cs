using fantec.Battle.Model;
using UnityEngine;
using UnityEngine.UI;

namespace fantec.Battle.Ui.Window
{
    public class DebugStateTextView : MonoBehaviour
    {
        [SerializeField] Text m_HPText;
        [SerializeField] Text m_HPTextB;
        [SerializeField] Text m_ATKText;
        [SerializeField] Text m_ATKTextB;
        [SerializeField] Text m_SPDText;
        [SerializeField] Text m_SPDTextB;
        [SerializeField] Text m_MovementText;
        [SerializeField] Text m_MovementTextB;
        public void SetStateText(IBattler battler)
        {
            m_HPText.text = ToValueFormat("HP", battler.State.Health.CurrentHealth);
            m_ATKText.text = ToValueFormat("ATK", battler.State.CurrentATK);
            m_SPDText.text = ToValueFormat("SPD", battler.State.CurrentSPD);
            m_MovementText.text = ToValueFormat("ˆÚ“®—Í", battler.State.CurrentMOVE);
            SetBuff(m_HPTextB,battler.State.BuffMaxHP);
            SetBuff(m_ATKTextB,battler.State.BuffATK);
            SetBuff(m_SPDTextB,battler.State.BuffSPD);
            SetBuff(m_MovementTextB, battler.State.BuffMOVE);
        }

        private string ToValueFormat(string title,int value)
        {
            return $"{title}:{value}";
        }

        private void SetBuff(Text text,int value)
        {
            if (value == 0) text.text = "";
            else
                if (value < 0)
            {
                text.text = $"(-{Mathf.Abs(value)})";
                text.color = Color.red;
            }
            else
            {
                text.text = $"(+{value})";
                text.color = Color.green;
            }
        }
    }
}
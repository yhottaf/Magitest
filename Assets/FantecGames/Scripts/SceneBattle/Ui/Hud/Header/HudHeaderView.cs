using UnityEngine;
using UnityEngine.UI;
using System;

namespace fantec.Battle.Ui
{
    public interface IHudHeaderView:ILocatable
    {
        void ShowSpeedText();
        void HideSpeedText();
        void UpdateCurrentWaveText(int wave);
        void UpdateMaxWaveText(int wave);
        void UpdateCurrentTurnText(int turn);
    }
}

namespace fantec.Battle.Ui.Hud.Wave
{
    public class HudHeaderView : MonoBehaviour,IHudHeaderView,IRegistable
    {
        [Header("Wave")]
        [SerializeField] Text m_CurrentWaveText;
        [SerializeField] Text m_MaxWaveText;

        [Header("Speed")]
        [SerializeField] Text m_SpeedText;

        [Header("Turn")]
        [SerializeField] Text m_CurrentTurnText;

        public void Register()
        {
            Locator.Register<IHudHeaderView>(this);
        }

        public void ShowSpeedText()
        {
            m_SpeedText.gameObject.SetActive(true);
        }

        public void HideSpeedText()
        {
            m_SpeedText.gameObject.SetActive(false);
        }

        public void UpdateCurrentWaveText(int wave)
        {
            try { m_CurrentWaveText.text = wave.ToString(); }
            catch { throw new IndexOutOfRangeException($"[index : {wave}] 配列の範囲外、または要素が存在しない。"); }
        }

        public void UpdateMaxWaveText(int wave)
        {
            try { m_MaxWaveText.text = wave.ToString(); }
            catch { throw new IndexOutOfRangeException($"[index : {wave}] 配列の範囲外、または要素が存在しない。"); }
        }

        public void UpdateCurrentTurnText(int turn)
        {
            try { m_CurrentTurnText.text = turn.ToString(); }
            catch { throw new IndexOutOfRangeException($"[index : {turn}] 配列の範囲外、または要素が存在しない。"); }
        }
    }
}
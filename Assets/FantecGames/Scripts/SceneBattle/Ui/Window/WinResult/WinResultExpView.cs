using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace fantec.Battle.Ui.Window
{
    public class WinResultExpView : MonoBehaviour
    {
        [SerializeField] private Text m_CurrentRankText;
        [SerializeField] private Text m_NextRankText;
        [SerializeField] private Text m_TableExpText;
        [SerializeField] private Text m_GainExpText;
        [SerializeField] private Text m_RequiredExpText;
        [SerializeField] private Image m_GaugeFillDynamicImage; // 今回の報酬で経験値がもらえる前の経験値
        [SerializeField] private Image m_GaugeFillStaticImage;  // 経験値が増えた分のゲージバーの画像

        private Sequence m_Sequence;

        public void Init()
        {
            m_GaugeFillDynamicImage.fillAmount = 0;
            m_GaugeFillStaticImage.fillAmount = 0;
        }

        public void SetCurrentRank(int rank)
        {
            m_CurrentRankText.text = rank.ToString();   
        }

        public void SetNextRank(int rank)
        {
            m_NextRankText.text=rank.ToString();
        }

        public void SetGainExp(int exp)
        {
            m_GainExpText.text = "+" + exp.ToString();
        }

        public void SetTable(int exp)
        {
            m_TableExpText.text = exp.ToString();
        }

        public void PlayAnimation(int[] nextExps,float fillStart,float fillEnd,Action<int>onLevelUp,Action onCompleted)
        {
            var loopCount = Mathf.FloorToInt(fillEnd); // レベルアップする回数
            var remainder = fillEnd - loopCount;       // レベルアップの余り経験値
            var level = 0;                             // 加算されるレベル

            m_Sequence = DOTween.Sequence();

            // レベルアップ分
            for(int i=0;i<loopCount;i++)
            {
                if (loopCount == 0) break;

                var from = i == 0 ? fillStart : 0;
                m_Sequence.Append(GetFillSequence(nextExps[i], from, 1, Ease.Linear, () =>
                {
                    level++;
                    onLevelUp.Invoke(level);
                }));
            }

            // 余り分
            {
                var to = remainder;
                m_Sequence.Append(GetFillSequence(nextExps[nextExps.Length - 1], 0, to,Ease.OutCubic, null));
            }

            // 完了通知
            m_Sequence.OnComplete(() => onCompleted.Invoke());
        }

        private Sequence GetFillSequence(int nextExp,float from,float to ,Ease ease,Action onLevelUp)
        {
            return DOTween.Sequence()
                .OnStart(() =>
                {
                    m_GaugeFillStaticImage.fillAmount = to;
                })
                .Append(DOVirtual.Float(from, to, 1.0f, value =>
                {
                    m_GaugeFillDynamicImage.fillAmount = value;
                    m_RequiredExpText.text = (value * nextExp).ToString("f0");
                })
                .OnComplete(() =>
                {
                    if (to == 1.0f) onLevelUp?.Invoke();
                }))
                .SetEase(ease);
        }

        public void PauseAnimation()
        {
            m_Sequence.timeScale = 0;
        }

        public void ResumeAnimation()
        {
            m_Sequence.timeScale = 1;
        }
    }
}
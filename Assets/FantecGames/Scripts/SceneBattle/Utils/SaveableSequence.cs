using DG.Tweening;
using NUnit.Framework;
using UnityEngine;

namespace fantec.Battle.Utiles
{
    /// <summary>
    /// パラメーター保持できるシーケンス
    /// </summary>
    public class SaveableSequence
    {
        private Sequence m_Sequence;
        private float m_TimeScale = 1;

        public bool IsExist => m_Sequence != null;
        public bool IsPlaying => IsExist ? !m_Sequence.IsActive() : false;

        public float TimeScale
        {
            get { return m_TimeScale; }
            set
            {
                m_TimeScale = value;
                if (IsExist) m_Sequence.timeScale = value;
            }
        }

        public Sequence Value
        {
            get { return m_Sequence; }
            set
            {
                m_Sequence = value;
                m_Sequence.timeScale = m_TimeScale;
            }
        }

        public void Kill() => m_Sequence?.Kill();
        public void Complete()=>m_Sequence?.Complete();
    }
}
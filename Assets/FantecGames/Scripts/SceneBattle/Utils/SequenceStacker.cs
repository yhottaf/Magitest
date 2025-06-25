using DG.Tweening;
using JetBrains.Annotations;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace fantec.Battle.Utiles
{
    /// <summary>
    /// タイムスケールが保持される
    /// </summary>
    public class SequenceStacker
    {
        private readonly List<Sequence>m_SequenceList=new List<Sequence>();
        private float m_TimeScale = 1;

        // 再生中のシーケンスが存在するか否か
        public bool IsSequenceExist => m_SequenceList.Count != 0;

        public float TimeScale
        {
            get { return m_TimeScale; }
            set
            {
                m_TimeScale = value;
                if(IsSequenceExist)
                    m_SequenceList.ForEach(x=>x.timeScale = m_TimeScale);
            }
        }

        public void Add(Sequence sequence)
        {
            sequence.timeScale = m_TimeScale;
            sequence.OnKill(()=>m_SequenceList.Remove(sequence));
            m_SequenceList.Add(sequence);
        }

        public Sequence Value
        {
            set { Add(value); }
        }

        public void Kill()
        {
            for(int i=0;i<m_SequenceList.Count();i++)
            {
                m_SequenceList[i].Kill();
            }
        }

        public void Complete()
        {
            for(int i=0;i<m_SequenceList.Count();i++)
            {
                m_SequenceList[i].Complete();
            }
        }
    }
}
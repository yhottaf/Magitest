using System.Collections.Generic;
using UnityEngine;

namespace fantec.Menu.PartyEdit
{
    public class CommonProgressView : MonoBehaviour
    {
        [Header("プログレスビューに私用するドットクラス")]
        [SerializeField]
        private ProgressDot m_ProgressParts;

        [SerializeField]
        private Transform m_PartsContainer;

        private int m_PartsNum = 0;

        public int PartsNum { get { return m_PartsNum; } set { m_PartsNum = value; } }

        private List<ProgressDot>m_CloneObjects=new List<ProgressDot>();

        public void Initialize(int ProgressNum, int InitNum = 0)
        {
            PartsNum = ProgressNum;

            if (m_CloneObjects.Count > 0 || ProgressNum < 0) return;

            ProgressDot _Clone;
            for(int nCnt=0;nCnt<ProgressNum;nCnt++)
            {
                _Clone = Instantiate(m_ProgressParts, m_PartsContainer);

                _Clone.ChangeProgress(nCnt == InitNum);

                m_CloneObjects.Add(_Clone); 
            }
        }

        public void ChangeProgress(int ProgressNum)
        {
            if (m_CloneObjects.Count < 1) return;

            int nCnt = 0;
            foreach(var item in m_CloneObjects)
            {
                item.ChangeProgress(nCnt == ProgressNum);

                ++nCnt;
            }
        }
    }
}
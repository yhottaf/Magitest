using fantec.Battle.Utiles;
using UnityEngine;

namespace fantec.Battle.Ui.Animation
{
    public class SpeedableAnimator : SpeedableBehaviour
    {
        private readonly int m_HashSpeed = Animator.StringToHash("Speed");

        [SerializeField] private Animator m_Animator;
        [SerializeField] private bool m_IsThroughPause;

        protected override void Awake()
        {
            base.Awake();

            if(m_Animator==null)m_Animator=this.GetComponent<Animator>();
            if(m_Animator==null)
            {
                Debug.LogError("Animator がアタッチされていません。");
            }
        }

        private void Reset()
        {
            m_Animator=this.GetComponent<Animator>();
        }

        private void OnEnable()
        {
            m_Animator.SetFloat(m_HashSpeed, m_TimeScale);
            base.SetThroughDirectingPause(m_IsThroughPause);
        }

        protected override void OnSetCurrentTimeScale(float speed)
        {
            m_Animator.SetFloat(m_HashSpeed, speed);
        }
    }
}
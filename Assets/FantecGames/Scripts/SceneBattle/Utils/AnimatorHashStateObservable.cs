using System;
using UnityEngine;
using UniRx;

namespace fantec.Battle.Utiles
{
    public class AnimatorHashStateObservable : MonoBehaviour,IObservable<bool>
    {
        [SerializeField] private Animator m_Animator;
        [SerializeField] private int m_LayerIndex;
        [SerializeField] private string m_StateName;
        [SerializeField] private float m_Delay;

        private AnimatorStateInfo m_CurrentStateInfo;
        private BoolReactiveProperty m_HashStateReactive=new BoolReactiveProperty();
        private int m_HashStateName;

        private void Awake()
        {
            m_HashStateName=Animator.StringToHash(m_StateName);

            if(m_Animator==null)m_Animator=GetComponent<Animator>();
            if(m_Animator==null)
            {
                throw new Exception("アニメーターコンポーネントがアタッチされていません。");
            }
        }

        private void Reset()
        {
            m_Animator=this.GetComponent<Animator>();
        }

        private void Update()
        {
            m_CurrentStateInfo=m_Animator.GetCurrentAnimatorStateInfo(m_LayerIndex);
            m_HashStateReactive.Value = m_CurrentStateInfo.shortNameHash == m_HashStateName;
        }

        private void OnDestroy()
        {
            m_HashStateReactive.Dispose();
        }

        public IDisposable Subscribe(IObserver<bool> observer)
        {
            return m_HashStateReactive.Delay(TimeSpan.FromSeconds(m_Delay)).Subscribe(observer).AddTo(this);
        }
    }
}
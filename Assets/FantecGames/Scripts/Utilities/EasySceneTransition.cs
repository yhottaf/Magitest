using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System;
using fantec.Common;
namespace fantec.Utilities
{
    public class EasySceneTransition : MonoBehaviour
    {
        [SerializeField] SceneIndex m_TargetScene;
        [SerializeField] Button m_Button;
        [SerializeField] double m_Timer;

        private IDisposable m_Disposable;

        private void Awake()
        {
            //ボタンを押したら遷移
            if (m_Button)
                m_Button.OnClickAsObservable()
                    .Subscribe(_ =>
                    {
                        ChangeSceane();
                    }).AddTo(this);

            //指定時間が過ぎたら遷移
            if (m_Timer != 0)
                m_Disposable = Observable.Timer(TimeSpan.FromSeconds(m_Timer))
                    .Subscribe(_ =>
                    {
                        ChangeSceane();
                    }).AddTo(this);
        }

        /// <summary>
        /// フェード後にシーン遷移する
        /// </summary>
       private void ChangeSceane()
        {
            Fade.FadeOut(1.0f, () =>
            {
                ExSceneManager.Instance.LoadScene(m_TargetScene);
            });

            if(m_Disposable!=null)
            { 
                m_Disposable.Dispose(); 
            }
        }
    }
}
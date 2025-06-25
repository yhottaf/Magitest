using System;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

namespace fantec.Menu.Common.View
{
    public class CommonFooterView : MonoBehaviour
    {
        [SerializeField] private Button m_HomeButton;
        [SerializeField] private Button m_GachaButton;
        [SerializeField] private Button m_QuestButton; // 仮のボタン あとで消す

        [SerializeField] private GameObject[] m_FooterObjects;

        public IObservable<Unit> OnClickHomeButtonObservable => m_HomeButton.OnClickAsObservable();

        public IObservable<Unit> OnClickQuestButtonObservable => m_QuestButton.OnClickAsObservable();

        public IObservable<Unit>OnClickGachaButtonObservable=>m_GachaButton.OnClickAsObservable();

        public void ShowButton(FooterType footerType)
        {
            for(int i=0;i<m_FooterObjects.Length;i++)
            {
                m_FooterObjects[i].gameObject.SetActive(i==(int)footerType);
            }
        }
    }
}
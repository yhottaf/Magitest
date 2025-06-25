using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace fantec.Battle.Ui.Window
{
    public class ContinueMenuView : MonoBehaviour
    {
        public IObservable<Unit> OnClickYes => m_YesButton.OnClickAsObservable();
        public IObservable<Unit>OnClickNo=> m_NoButton.OnClickAsObservable();

        [SerializeField]private Button m_YesButton;
        [SerializeField]private Button m_NoButton;
    }
}
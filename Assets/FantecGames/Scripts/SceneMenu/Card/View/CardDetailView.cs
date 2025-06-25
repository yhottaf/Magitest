using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace fantec.Menu.Card.View
{
    public class CardDetailView : MonoBehaviour
    {
        [SerializeField] private Button m_CloseButton;
        public IObservable<Unit> OnClickCloseButtonObservable => m_CloseButton.OnClickAsObservable();
    }
}
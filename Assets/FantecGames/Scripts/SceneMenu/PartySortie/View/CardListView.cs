using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace fantec.Menu.Card.View
{
    public class CardListView : MonoBehaviour
    {
        [SerializeField]
        private Button m_SortButton;

        [SerializeField]
        private Toggle m_Ordertoggle;

        [SerializeField]
        private Button m_BackButton;

        [SerializeField]
        private GameObject m_NoticeWindow;
        [SerializeField]
        private GameObject m_CheckWindow;
        [SerializeField]
        private Button m_YesButton;
        [SerializeField]
        private Button m_NoButton;
        [SerializeField]
        private Button m_CheckYesButton;
        [SerializeField]
        private Button m_CheckNoButton;


        public IObservable<Unit> OnClickBackButtonObservable => m_BackButton.OnClickAsObservable();
        public IObservable<Unit>OnClickYesButtonObservable=>m_YesButton.OnClickAsObservable();
        public IObservable<Unit>OnClickNoButtonObservable=>m_NoButton.OnClickAsObservable();
        public IObservable<Unit> OnClickCheckYesButtonObservable => m_CheckYesButton.OnClickAsObservable();
        public IObservable<Unit>OnClickCheckNoButtonObservable=>m_CheckNoButton.OnClickAsObservable();

        private Subject<Unit> changePartyData = new Subject<Unit>();

        public IObservable<Unit> OnNextSwipe => changePartyData.AsObservable();

        private bool m_bSortingOrder;

        private void Start()
        {
            m_bSortingOrder = true;
        }

        public bool GetSortingOrder()
        {
            return m_bSortingOrder;
        }

        public void SetSortingOrder(bool value)
        {
            m_bSortingOrder = value;
        }

        // マギが1体も編成されていませんのダイアログを出す
        public void SetNoticeWindow(bool enable)
        {
            m_NoticeWindow.SetActive(enable);
        }

        // 編成を保存しますか？ いいえを選ぶと破棄されます のダイアログを出す
        public void SetCheckWindow(bool enable)
        {
            m_CheckWindow.SetActive(enable);
        }

        // スワイプによってデッキの内容が変更されたことを通知させる
        public void ChangePartyData()
        {
            changePartyData.OnNext(Unit.Default);
        }
    }
}
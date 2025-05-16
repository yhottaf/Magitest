using System;
using UniRx;
using UnityEngine;

namespace fantec.Battle
{
    public interface IRankUpPopupWindow:IWindow<IRankUpPopupWindow>
    {
        IObservable<Unit> OnEnd { get; }
        IRankUpPopupWindow SetRankValue(int oldValue, int newValue);
        IRankUpPopupWindow SetStaminaValue(int oldValue, int newValue);
    }
}

namespace fantec.Battle.Ui.Window
{
    public class RankUpPopupWindow : WindowBase,IRankUpPopupWindow
    {
        public CompositeDisposable ClosedDisposable { get; private set; }=new CompositeDisposable();

        public IObservable<Unit> OnEnd => m_Fader.OnHideCompleted;
        [SerializeField] private RankUpPopupTextView m_TextView;
        [SerializeField] private GroupFader m_Fader;

        public IRankUpPopupWindow OnCreate()
        {
            m_TextView.OnClickCloseButton.Subscribe(_=>m_Fader.PlayHide()).AddTo(this);
            OnEnd.Subscribe(_=>this.Close()).AddTo(this);
            m_Fader.PlayShow();
            return this;
        }

        public void Close()
        {
            ClosedDisposable.Dispose();
            Destroy(this.gameObject);
        }

        public IRankUpPopupWindow SetRankValue(int oldValue,int newValue) { m_TextView.SetRankText(oldValue, newValue);return this; }
        public IRankUpPopupWindow SetStaminaValue(int oldValue,int newValue) { m_TextView.SetStaminaText(oldValue, newValue);return this; }
    }
}
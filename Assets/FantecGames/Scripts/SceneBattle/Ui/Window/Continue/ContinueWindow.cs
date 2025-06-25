using System;
using UniRx;
using UnityEngine;

namespace fantec.Battle
{
    public interface IContinueWindow:IWindow<IContinueWindow>
    {
        IObservable<Unit> OnClickYes { get; }
        IObservable<Unit> OnClickNo { get; }

        IContinueWindow SetText(int paid, int free);
    }
}

namespace fantec.Battle.Ui.Window
{
    public class ContinueWindow : WindowBase,IContinueWindow
    {
        public CompositeDisposable ClosedDisposable { get; private set; }=new CompositeDisposable();

        [SerializeField] ContinueMenuView m_MenuView;
        [SerializeField] ContinueTextView m_TextView;

        public IObservable<Unit> OnClickYes => m_MenuView.OnClickYes;
        public IObservable<Unit> OnClickNo => m_MenuView.OnClickNo;

        public IContinueWindow OnCreate()
        {
            OnClickYes.DelayFrame(1).Subscribe(_ => this.Close());
            OnClickNo.DelayFrame(1).Subscribe(_ => this.Close());
            return this;
        }

        public IContinueWindow OnOpen()
        {
            return this;
        }

        public void Close()
        {
            ClosedDisposable.Dispose();
            Destroy(this.gameObject);
        }

        public IContinueWindow SetText(int paid,int free) { m_TextView.SetText(paid, free);return this; }
    }
}
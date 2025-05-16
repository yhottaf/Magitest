using System;
using UniRx;
using UnityEngine;

namespace fantec.Battle
{
    public interface IContinueDoubleWindow:IWindow<IContinueDoubleWindow>
    {
        IObservable<Unit> OnClickYes { get; }
        IObservable<Unit> OnClickNo { get; }
    }
}

namespace fantec.Battle.Ui.Window
{
    public class ContinueDoubleWindow : WindowBase, IContinueDoubleWindow
    {
        public CompositeDisposable ClosedDisposable { get; private set; }=new CompositeDisposable();

        [SerializeField] private ContinueDoubleMenuView m_MenuView;

        public IObservable<Unit> OnClickYes => m_MenuView.OnClickYes;
        public IObservable<Unit> OnClickNo => m_MenuView.OnClickNo;

        #region IContinueDoubleWindow

        public IContinueDoubleWindow OnCreate()
        {
            OnClickYes.DelayFrame(1).Subscribe(_=>this.Close()).AddTo(this);
            OnClickNo.DelayFrame(1).Subscribe(_ => this.Close()).AddTo(this);

            return this;
        }

        public void Close()
        {
            ClosedDisposable.Dispose();
            Destroy(this.gameObject);
        }
        #endregion
    }
}
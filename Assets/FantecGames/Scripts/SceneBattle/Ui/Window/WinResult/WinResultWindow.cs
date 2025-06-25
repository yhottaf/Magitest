using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace fantec.Battle
{
    public interface IWinResultWindow : IWindow<IWinResultWindow>
    {
        IObservable<Unit> OnRematch { get; }
        IObservable<Unit> OnNext { get; }
        IWinResultWindow SetStageName(string text);
        IWinResultWindow SetCurrentRank(int rank);
        IWinResultWindow SetNextRank(int rank);
        IWinResultWindow SetGainExp(int exp);
        IWinResultWindow SetTableExp(int exp);
        IWinResultWindow PlayExpAnimation(int[] nextExps, float fillStart, float fillEnd, Action<int> onLevelUp, Action onCompleted);
        IWinResultWindow PauseExpAnimation();
        IWinResultWindow ResumeExpAnimation();
        IWinResultWindow PlayDropItem(IEnumerable<RewerdInfo> infos,Action onComleted);
        IWinResultWindow ClearDropItem();

        IWinResultWindow SetInteractableRetryButton(bool enable);
        IWinResultWindow SetInteractableNextButton(bool enabled);
        IWinResultWindow SetStaminaText(int consumeStamina, int currentStamina);
    }
}


namespace fantec.Battle.Ui.Window
{
    public class WinResultWindow : WindowBase,IWinResultWindow
    {
        public CompositeDisposable ClosedDisposable { get; private set; }=new CompositeDisposable();

        public IObservable<Unit> OnRematch => m_MenuView.OnClickRematchObservable;
        public IObservable<Unit> OnNext => m_MenuView.OnClickNextObservable;

        [SerializeField] WinResultHeaderView m_HeaderView;
        [SerializeField] WinResultExpView m_ExpView;
        [SerializeField] WinResultDropItemView m_DropView;
        [SerializeField] WinResultMenuView m_MenuView;

        public IWinResultWindow OnCreate()
        {
            m_DropView.Init();
            m_ExpView.Init();
            return this;
        }

        public void Close()
        {
            ClosedDisposable.Dispose();
            Destroy(this.gameObject);
        }

        // ---------------------------------------------//
        // Header
        // ---------------------------------------------//
        public IWinResultWindow SetStageName(string text) { m_HeaderView.SetStageName(text); return this; }
        // ---------------------------------------------//
        // Exp
        // ---------------------------------------------//
        public IWinResultWindow SetCurrentRank(int rank) { m_ExpView.SetCurrentRank(rank);return this; }
        public IWinResultWindow SetNextRank(int rank) { m_ExpView.SetNextRank(rank);return this; }
        public IWinResultWindow SetGainExp(int exp) { m_ExpView.SetGainExp(exp);return this; }
        public IWinResultWindow SetTableExp(int exp) { m_ExpView.SetTable(exp);return this; }
        public IWinResultWindow PlayExpAnimation(int[] nextExps, float fillStart, float fillEnd, Action<int> onLevelUp, Action onCompleted) { m_ExpView.PlayAnimation(nextExps, fillStart, fillEnd, onLevelUp, onCompleted);return this; }
        public IWinResultWindow PauseExpAnimation() { m_ExpView.PauseAnimation(); return this; }
        public IWinResultWindow ResumeExpAnimation() { m_ExpView.ResumeAnimation(); return this; }
        // ---------------------------------------------//
        // DropItem
        // ---------------------------------------------//
        public IWinResultWindow PlayDropItem(IEnumerable<RewerdInfo>infos,Action onCompleted) { m_DropView.PlayDropItem(infos, onCompleted);return this; }
        public IWinResultWindow ClearDropItem() { m_DropView.ClearDropItem();return this; }


        // ---------------------------------------------//
        // Menu
        // ---------------------------------------------//

        public IWinResultWindow SetInteractableRetryButton(bool enabled) { m_MenuView.SetInteractableRetryButton(enabled); return this; }
        public IWinResultWindow SetInteractableNextButton(bool enabled) { m_MenuView.SetInteractableNextButton(enabled);return this; }
        public IWinResultWindow SetStaminaText(int consumeStamina,int currentStamina) { m_MenuView.SetStaminaText(consumeStamina, currentStamina);return this; }
    }
}
using fantec.Battle.Model;
using UniRx;
using UnityEngine;

namespace fantec.Battle.Ui
{
    public class HudHeaderPresenter : ILocatable
    {
        private IHudHeaderView m_View;

        public HudHeaderPresenter(CompositeDisposable disposables)
        {
            m_View=Locator.Resolve<IHudHeaderView>();

            var modelStage = Locator.Resolve<IBattleModelStage>();
            var modelTime=Locator.Resolve<IBattleModelTime>();
           // var modelUnits=Locator.Resolve<IBattleModelUnits>();

            modelStage.OnCurrentWaveIndexReactive.Subscribe(OnUpdateWave).AddTo(disposables);
            modelStage.OnCurrentTurnIndexReactive.Subscribe(OnUpdateTurn).AddTo(disposables);
            modelStage.OnUpdateStageDataObservable.Subscribe(OnUpdateStageData).AddTo(disposables);
            modelTime.OnGameTimeScale.Subscribe(OnUpdateTimeScale).AddTo(disposables);
        }


        private void OnUpdateStageData(IBattleModelStage property)
        {
            m_View.UpdateMaxWaveText(property.Entity.maxWave);
        }

        private void OnUpdateWave(int waveIndex)
        {
            m_View.UpdateCurrentWaveText(waveIndex + 1);
        }

        private void OnUpdateTurn(int turnIndex)
        {
            if(turnIndex!=30)
            m_View.UpdateCurrentTurnText(turnIndex + 1);
        }

        private void OnUpdateTimeScale(float timeScale)
        {
            switch(timeScale)
            {
                case 1:m_View.HideSpeedText();break;
                case 2:m_View.ShowSpeedText();break;
            }
        }
    }
}
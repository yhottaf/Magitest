using fantec.Battle.Field.Chara;
using fantec.Battle.Utiles;
using UnityEngine;

namespace fantec.Battle.Field.Chara
{
    public class EnemyView : SpeedableBehaviour,IInitializable,IReloadable
    {
        public SpineView SpineView => m_SpineView;
        public MovementView MovementView => m_MovementView;
        public SortingView SortingView => m_SortingView;
        public StateView StateView => m_StateView;
        public BuffView BuffView => m_BuffView;

        [SerializeField] SpineView m_SpineView;
        [SerializeField] MovementView m_MovementView;
        [SerializeField] SortingView m_SortingView;
        [SerializeField] StateView m_StateView;
        [SerializeField] BuffView m_BuffView;

        protected override void OnSetCurrentTimeScale(float timeScale)
        {
            m_SpineView.SetTimeScale(timeScale);
            m_MovementView.SetTimeScale(timeScale);
        }

        public void Initialize()
        {
            m_MovementView.Initialize();
            m_SortingView.Initialize();
        }

        public void Reload()
        {
            m_MovementView.Reload();
            m_SortingView.Reload();


            m_SpineView.SetTimeScale(base.m_TimeScale);
            m_MovementView.SetTimeScale(base.m_TimeScale);
        }

        public void Show()
        {
            this.gameObject.SetActive(true);
        }

        public void Hide()
        {
            this.gameObject.SetActive(false);
        }

        public void SetThroughPause(bool enabled)
        {
            base.SetThroughDirectingPause(enabled);
        }
    }
}
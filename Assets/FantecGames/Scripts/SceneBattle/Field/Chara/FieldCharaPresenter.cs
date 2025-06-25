using fantec.Battle.Model;
using UniRx;
using UnityEngine;

namespace fantec.Battle.Field.Chara
{
    public class FieldCharaPresenter : MonoBehaviour
    {
        [SerializeField] PlayerView[] m_PlayerViews;
        [SerializeField] EnemyView[] m_EnemyViews;
        private CompositeDisposable m_Disposables=new CompositeDisposable();

        public void Start()
        {
            var modelUnits = Locator.Resolve<IBattleModelUnits>();

            for (int i = 0; i < Define.PARTY_CAPACITY; i++)
            {
                new PlayerCharaPresenter(m_PlayerViews[i]).Initialize(modelUnits.PlayerDatas[i], m_Disposables);
                new PlayerStatePresenter(m_PlayerViews[i].StateView).Initialize(modelUnits.PlayerDatas[i], m_Disposables);

                new EnemyCharaPresenter(m_EnemyViews[i]).Initialize(modelUnits.EnemyDatas[i], m_Disposables);
                new EnemyStatePresenter(m_EnemyViews[i].StateView).Initialize(modelUnits.EnemyDatas[i], m_Disposables);
            }
        }

        public void PlayerInit()
        {
            var modelUnits = Locator.Resolve<IBattleModelUnits>();

            for (int i = 0; i < Define.PARTY_CAPACITY; i++)
            {
                new PlayerCharaPresenter(m_PlayerViews[i]).Initialize(modelUnits.PlayerDatas[i], m_Disposables);
                new PlayerStatePresenter(m_PlayerViews[i].StateView).Initialize(modelUnits.PlayerDatas[i], m_Disposables);
            }
        }

        public void EnemyInit()
        {
            var modelUnits = Locator.Resolve<IBattleModelUnits>();

            for (int i = 0; i < Define.PARTY_CAPACITY; i++)
            {
                new EnemyCharaPresenter(m_EnemyViews[i]).Initialize(modelUnits.EnemyDatas[i], m_Disposables);
                new EnemyStatePresenter(m_EnemyViews[i].StateView).Initialize(modelUnits.EnemyDatas[i], m_Disposables);
            }
        }

        private void OnDestroy()
        {
            m_Disposables.Dispose();
        }
    }
}
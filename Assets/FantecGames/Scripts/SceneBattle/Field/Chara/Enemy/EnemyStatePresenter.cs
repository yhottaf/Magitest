using fantec.Battle.Model;
using UnityEngine;

namespace fantec.Battle.Field.Chara
{
    public class EnemyStatePresenter : IStatePresenter
    {
        private StateView m_View;

        public EnemyStatePresenter(StateView view)
        {
            m_View = view;
        }

        public void OnDead(AffectInfo info)
        {
            m_View.Hide();
        }

        public void OnRevival(AffectInfo info)
        {
            m_View.Show();
        }

        public void OnUpdateHealth(ValueChangeInfo info)
        {
            m_View.UpdateHpFill(info.Normalized);
        }

        public void OnUpdateUnitData(IBattlerParamUnit unit)
        {
            if(unit.IsExist)
            {
                m_View.Show();
            }
            else
            {
                m_View.Hide();
            }
        }
    }
}

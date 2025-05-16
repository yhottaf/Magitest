using System.Collections.Generic;

namespace fantec.Battle.Field.Chara
{
    public class PlayerBuffPresenter : IBuffPresenter
    {
        private BuffView m_View;

        public PlayerBuffPresenter(BuffView view)
        {
            m_View = view;
        }

        public void OnUpdateToken(List<TokenCell>cellList)
        {
            m_View.SetActiveBuff(cellList.GetTryBuffToken(out _));
            m_View.SetActiveDebuff(cellList.GetTryDebuffToken(out _));
        }

        public void OnDead()
        {
            m_View.SetActiveBuff(false);
            m_View.SetActiveDebuff(false);
        }
    }
}
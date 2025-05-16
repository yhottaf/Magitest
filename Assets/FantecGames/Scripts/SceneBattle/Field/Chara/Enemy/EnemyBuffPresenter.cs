using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace fantec.Battle.Field.Chara
{
    public class EnemyBuffPresenter : IBuffPresenter
    {
        private BuffView m_View;

        public EnemyBuffPresenter(BuffView view)
        {
            m_View = view;
        }

        public void OnUpdateToken(List<TokenCell> cellList)
        {
            m_View.SetActiveBuff(cellList.GetTryBuffToken(out _));
            m_View.SetActiveDebuff(cellList.GetTryBuffToken(out _));
        }

        public void OnDead()
        {
            m_View.SetActiveBuff(false);
            m_View.SetActiveDebuff(false);
        }
    }
}
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace fantec.Battle.Field.Chara
{
    public class EnemyParticlePresenter : IParticlePresenter
    {
        // TODO:ParticleViewを作る
       // ParticleView

        public EnemyParticlePresenter() { }

        public void OnUpdateTokenList(List<TokenCell>cellList)
        {
            foreach(var cell in cellList)
            {
                if(cell.command.categoryType.GetIsAbnormalCondition())
                {

                }
            }
        }
    }
}
using fantec.Battle.Model;
using NUnit.Framework;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace fantec.Battle.Field.Chara
{
    public interface IParticlePresenter
    {
        void OnUpdateTokenList(List<TokenCell> cellList);
    }

    public static class ParticlePresenterExtentions
    {
        public static void Initialize(this IParticlePresenter @this,IBattler battler,CompositeDisposable disaposables)
        {
            battler.State.OnUpdateTokenCellList.Subscribe(@this.OnUpdateTokenList).AddTo(disaposables);
        }
    }
}
using fantec.Battle;
using System;
using UnityEngine;

namespace fantec.Battle.Model
{
    public interface IBattleModelFrontBattle : ILocatable, IDisposable, IFookable, IResetable
    {
        IObservable<IBattler>OnSetupCompleted { get; }
    }
}
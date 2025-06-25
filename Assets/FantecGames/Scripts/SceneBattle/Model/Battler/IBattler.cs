using Cysharp.Threading.Tasks;
using fantec.Common;
using System;
using UnityEngine;

namespace fantec.Battle.Model
{
    public interface IBattler : IDisposable,IResetable,IReloadable
    {
        IObservable<IBattler> OnSetupCompleted { get; }
        IObservable<IBattler> OnReload { get; }
        IBattlerParamUnit Unit { get; }
        IBattlerParamState State { get; }
        IBattlerParamOverrideSkill OverrideSkill { get; }
        IBattlerParamAdventSkill AdventSkill { get; }
        IBattlerParamTransform Transform { get; }

        void SetUp(TeamData.Unit unit ,bool isTakeOver=false,bool leader=false);
    }
}
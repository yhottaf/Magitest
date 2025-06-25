using fantec.Battle.Manager;
using fantec.Battle.Model;
using System;
using UniRx;
using UnityEngine;

namespace fantec.Battle.Utiles
{
    public class HomingLuncher
    {
        public static void Lunch(AffectInfo info,Action onCompleted=null)
        {
            foreach(var target in info.Targets)
            {
                var start = Locator.Resolve<IBattlePlacementManager>().GetHomingPosition(info.Owner);
                var end = Locator.Resolve<IBattlePlacementManager>().GetHomingPosition(target);
                Locator.Resolve<IBattlePoolManager>().Rent(PoolableHomingParticle.GetIndex(info.Command.attributeType)).SetupStraight(start, end).OnCompleted.First().Subscribe(_ =>
                {
                    onCompleted?.Invoke();
                });
            }
        }
    }
}
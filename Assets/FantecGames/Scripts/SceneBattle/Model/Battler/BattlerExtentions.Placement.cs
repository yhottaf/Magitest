using fantec.Battle.Manager;
using fantec.Battle.Model;
using UnityEngine;

namespace fantec.Battle
{
    public static partial class BattlerExtentions
    {
        public static Vector3 GetBattlerPosition(this IBattler @this)=>
            Locator.Resolve<IBattlePlacementManager>().GetBattlerPosition(@this);

        public static Vector3 GetCenterPosition(this IBattler @this)=>
            Locator.Resolve<IBattlePlacementManager>().GetCenterPosition(@this);

        public static Vector3 GetBeAttackPosition(this IBattler @this)=>
            Locator.Resolve<IBattlePlacementManager>().GetBeAttackPosition(@this);

        public static Vector3 GetOffScreenPosition(this IBattler @this)=>
            Locator.Resolve<IBattlePlacementManager>().GetOffScreenPosition(@this);

        public static Vector3 GetBeAttackCenterPosition(this IBattler @this)=>
            Locator.Resolve<IBattlePlacementManager>().GetBeAttackCenterPosition(@this);

        public static Vector3 GetBuffCenterPosition(this IBattler @this)=>
            Locator.Resolve<IBattlePlacementManager>().GetBuffCenterPosition(@this);

        public static Vector3 GetKnockBackPosition(this IBattler @this)=>
            Locator.Resolve<IBattlePlacementManager>().GetKnockBackPosition(@this);

        public static Vector3 GetBlowBackPosition(this IBattler @this)=>
            Locator.Resolve<IBattlePlacementManager>().GetBlowBackPosition(@this);

        public static Vector3 GetTargetPosition(this IBattler @this) =>
            @this.Transform.TargetPosition;
    }
}
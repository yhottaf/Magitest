using UnityEngine;

namespace fantec.Battle
{
    public struct TakeDamageInfo
    {
        public ValueChangeInfo valueInfo;
        public AffectInfo affectInfo;
        public AffectAttributeType AttributeType => affectInfo.Command.attributeType;
        public AffectHitType HitType => affectInfo.Owner.Unit.Entity.hitType;
    }
}
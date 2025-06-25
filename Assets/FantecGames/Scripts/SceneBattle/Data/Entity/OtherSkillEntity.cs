using UnityEngine;
using fantec.Master;
using System.Collections.Generic;
using fantec.Battle.Manager;


namespace fantec.Battle
{
    public abstract class AbstructOtherEntity : AbstructSkillEntity
    {
        public AbstructOtherEntity(OtherSkillData data) : base(data)
        {
            commands.SetSuccessRate(data.successRate);
            commands.SetConditionValue(data.conditionValue);
        }

        public AbstructOtherEntity(OtherSkillData data, AffectAttributeType attributeType):this(data)
        {
            commands.InterpolationAttributeType(attributeType);
        }
    }

    public sealed class DummyEntity:AbstructOtherEntity
    {
        public static int DataId => BD.Entity.GetOtherSkillDataId<DummyEntity>();

        public DummyEntity(OtherSkillData data) : base(data) { }

        public static DummyEntity GetEntity()
        {
            return new DummyEntity(Locator.Resolve<IBattleMasterManager>().OtherSkillMaster.GetData(DataId));
        }
    }

    public sealed class NormalAttackEntity : AbstructOtherEntity
    {
        public static int DataId => BD.Entity.GetOtherSkillDataId<NormalAttackEntity>();

        public NormalAttackEntity(OtherSkillData data, AffectAttributeType attributeType) : base(data, attributeType) { }

        public static NormalAttackEntity GetEntity(AffectAttributeType attributeType)
        {
            return new NormalAttackEntity(Locator.Resolve<IBattleMasterManager>().OtherSkillMaster.GetData(DataId),attributeType);
        }
    }

    public sealed class DangerEntity:AbstructOtherEntity
    {
        public static int DataId => BD.Entity.GetOtherSkillDataId<DangerEntity>();

        public DangerEntity(OtherSkillData data) : base(data) { }

        public static DangerEntity GetEntity()
        {
            return new DangerEntity(Locator.Resolve<IBattleMasterManager>().OtherSkillMaster.GetData(DataId));
        }
    }

    public sealed class ReviveEntity : AbstructOtherEntity
    {
        public static int DataId => BD.Entity.GetOtherSkillDataId<ReviveEntity>();

        public ReviveEntity(OtherSkillData data) : base(data) { }

        public static ReviveEntity GetEntity()
        {
            return new ReviveEntity(Locator.Resolve<IBattleMasterManager>().OtherSkillMaster.GetData(DataId));
        }
    }

    public sealed class FullRecoveryEntity:AbstructOtherEntity
    {
        public static int DataId => BD.Entity.GetOtherSkillDataId<FullRecoveryEntity>();

        public FullRecoveryEntity(OtherSkillData data) : base(data) { }

        public static FullRecoveryEntity GetEntity()
        {
            return new FullRecoveryEntity(Locator.Resolve<IBattleMasterManager>().OtherSkillMaster.GetData(DataId));
        }
    }

    public sealed class FullSkillBoostEntity : AbstructOtherEntity
    {
        public static int DataId => BD.Entity.GetOtherSkillDataId<FullSkillBoostEntity>();

        public FullSkillBoostEntity(OtherSkillData data) : base(data) { }

        public static FullSkillBoostEntity GetEntity()
        {
            return new FullSkillBoostEntity(Locator.Resolve<IBattleMasterManager>().OtherSkillMaster.GetData(DataId));
        }
    }

    public sealed class TokenEntity : AbstructOtherEntity
    {
        public TokenEntity(OtherSkillData data):base(data) { }

        public TokenEntity(OtherSkillData data,AffectAttributeType attributeType):base(data,attributeType) { }
    }

    public static class OtherSkillEntityExtensions
    {
        public static TokenEntity ToTokenEntity(this OtherSkillData data, AffectAttributeType attributeType = AffectAttributeType.NONE)
        {
            return attributeType == AffectAttributeType.NONE ? new TokenEntity(data) : new TokenEntity(data, attributeType);
        }
    }
}

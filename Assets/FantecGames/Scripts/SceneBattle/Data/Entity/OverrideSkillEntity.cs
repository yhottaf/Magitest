using fantec.Battle.Manager;
using fantec.Master;
using System.Collections.Generic;
using UnityEngine;

namespace fantec.Battle
{
    public sealed class OverrideSkillEntity:AbstructSkillEntity
    {
        public readonly int[] triggerCardOriginId;
        public readonly AffectOverrideType overrideType;

        public OverrideSkillEntity(OverrideSkillData data):base(data)
        {
            this.triggerCardOriginId = data.triggerCardOriginId;
            this.overrideType = data.overrideType;
        }

        public OverrideSkillEntity(OverrideSkillData data,AffectAttributeType attributeType):this(data)
        {
            this.commands.InterpolationAttributeType(attributeType); // ‘®«‚ğ•âŠ®‚·‚é
        }
    }

    public static class OverrideSkillEntityExtensions
    {
        public static OverrideSkillEntity ToEntity(this OverrideSkillData data)
        {
            return new OverrideSkillEntity(data);
        }

        public static OverrideSkillEntity ToEntity(this OverrideSkillData data,AffectAttributeType attributeType)
        {
            return new OverrideSkillEntity(data,attributeType);
        }

        public static List<OverrideSkillEntity> ConvertToEntity(AffectAttributeType attributeType,List<(int id,int level)>partList)
        {
            var result=new List<OverrideSkillEntity>();

            foreach(var part in partList)
            {
                if (part.id == -1) continue;
                result.Add(Locator.Resolve<IBattleMasterManager>().OverrideSkillMaster.GetData(part.id, part.level).ToEntity(attributeType));
            }

            return result;
        }
    }
}
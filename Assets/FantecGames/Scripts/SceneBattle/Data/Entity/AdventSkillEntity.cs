using fantec.Battle.Manager;
using fantec.Master;
using System.Collections.Generic;


namespace fantec.Battle
{
    public sealed class AdventSkillEntity : AbstructSkillEntity
    {
        public readonly float triggerProbability;

        public AdventSkillEntity(AdventSkillData data):base(data)
        {
            this.triggerProbability = data.triggerProbility;
        }

        public AdventSkillEntity(AdventSkillData data,AffectAttributeType attributeType):this(data)
        {
            this.commands.InterpolationAttributeType(attributeType);  // ‘®«‚ğ•âŠ®‚·‚é
        }
    }

    public static class AdventSkillEntityExtensions
    {
        public static AdventSkillEntity ToEntity(this AdventSkillData data)
        {
            return new AdventSkillEntity(data);
        }

        public static AdventSkillEntity ToEntity(this AdventSkillData data,AffectAttributeType attributeType)
        {
            return new AdventSkillEntity(data, attributeType);
        }

        public static List<AdventSkillEntity>ConvertToEntity(AffectAttributeType attributeType,List<(int id,int level)>partList)
        {
            var result = new List<AdventSkillEntity>();

            foreach(var part in partList)
            {
                if (part.id == -1) continue;
                result.Add(Locator.Resolve<IBattleMasterManager>().AdventSkillMaster.GetData(part.id,part.level).ToEntity(attributeType));
            }

            return result;
        }
    }
}
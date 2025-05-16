using UnityEngine;
using fantec.Master;

namespace fantec.Battle
{
    public abstract class AbstructSkillEntity
    {
        public readonly int skillId;
        public readonly string skillName;
        public readonly string detail;

        public readonly SkillCommand[] commands;

        protected AbstructSkillEntity(AbstructSkillData abstructSkillData)
        {
            this.skillId=abstructSkillData.skillId;
            this.skillName=abstructSkillData.skillName;
            this.detail = abstructSkillData.detail;
            this.commands = abstructSkillData.ConvertToSkillCommandList().ToArray();
        }

        public bool GetIs<T>() where T: AbstructSkillEntity
        {
            return this is T;
        }
    }
}
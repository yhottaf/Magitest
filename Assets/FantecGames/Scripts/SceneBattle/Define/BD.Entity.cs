using System;
using System.Collections.Generic;
using UnityEngine;

namespace fantec.Battle
{
    public partial class BD
    {
        public class Entity
        {
            private static readonly Dictionary<Type, int> m_OtherEntityToDataIdDic = new Dictionary<Type, int>()
            {
                {typeof(DummyEntity),                 101 },
                {typeof(NormalAttackEntity),        10000 },
                {typeof(ReviveEntity),              10200 },
                { typeof(DangerEntity),             10300 },
                {typeof(FullRecoveryEntity),        10400 },
            };

            public static int GetOtherSkillDataId<T>() where T : AbstructOtherEntity
            {
                try { return m_OtherEntityToDataIdDic[typeof(T)]; }
                catch { throw new InvalidOperationException($"[{typeof(T).Name}] ÇÕÉLÅ[Ç∆ÇµÇƒê›íËÇ≥ÇÍÇƒÇ¢Ç‹ÇπÇÒÅB"); }
            }
        }
    }
}
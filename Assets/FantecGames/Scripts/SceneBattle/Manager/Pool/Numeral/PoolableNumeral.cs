using System.Collections.Generic;
using UnityEngine;

namespace fantec.Battle
{
    public static class PoolableNumeral
    {
        public enum Index
        {
            Numeral_Damage,
            Numeral_Heal,
        }

        private static readonly Dictionary<int, string> m_IndexNameDic = new Dictionary<int, string>()
        {
            {(int)Index.Numeral_Damage,              Index.Numeral_Damage.ToString()},
            {(int)Index.Numeral_Heal,                Index.Numeral_Heal.ToString()},
        };

        public static string ToStringQuickly(this Index @this)
        {
            try { return m_IndexNameDic[(int)@this]; }
            catch { throw new KeyNotFoundException($"[index : {@this}] が key として登録されていません。"); }
        }
    }
}
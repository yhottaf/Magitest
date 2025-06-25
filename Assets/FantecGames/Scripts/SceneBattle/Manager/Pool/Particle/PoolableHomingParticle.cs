using System.Collections.Generic;

namespace fantec.Battle.Manager
{
    public static class PoolableHomingParticle
    {
        // TODO: できれば属性毎にパーティクルの色を分ける
        public enum Index
        {
            Homing_通常, // 通常使用するパーティクル
        }


        private static readonly Dictionary<int, string> m_IndexNameDis = new Dictionary<int, string>()
        {
            {(int)Index.Homing_通常,              Index.Homing_通常.ToString() },
        };

        private static readonly Dictionary<int, Index> m_AttributeTypeDic = new Dictionary<int, Index>()
        {
            {(int)AffectAttributeType.通常,            Index.Homing_通常 },
        };

        public static string ToStringQuickly(this Index @this)
        {
            try { return m_IndexNameDis[(int)@this]; }
            catch (KeyNotFoundException) { throw new KeyNotFoundException($"[attribute : {@this}] がkey として割り当てられていません。"); }
        }

        public static Index GetIndex(AffectAttributeType attribute)
        {
            try { return m_AttributeTypeDic[(int)attribute]; }
            catch(KeyNotFoundException) { throw new KeyNotFoundException($"[attribute: {attribute}] が key として割り当てれていません。"); }
        }
    }
}
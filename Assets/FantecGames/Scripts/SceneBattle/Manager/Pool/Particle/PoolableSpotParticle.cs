using System.Collections.Generic;

namespace fantec.Battle
{
    public static class PoolableSpotParticle
    {
        /// <summary>
        /// 要素名はプレハブのファイル名と同じものにする
        /// </summary>
        public enum Index
        {
            // ヒット
            Spot_Hit_打,
            Spot_Hit_通常,
            Spot_Hit_炎,
            Spot_Hit_氷,

            // バフ 
            Spot_Buff_ATK,
            Spot_Buff,
            Spot_Debuff,

            // バフ以外の効果反映
            Spot_Affect_Heal,

            // ヒット種別テキスト
            Spot_HitText_Miss,      // ミス
            Spot_HitText_Guard,     // ガード

            // その他
            Spot_Other_Lightning,   // 落雷
            Spot_Other_Glitter,     // キラーン
        }

        #region Dictionary

        private static readonly Dictionary<int, string> m_IndexNameDic = new Dictionary<int, string>()
        {
            {(int)Index.Spot_Buff_ATK,              Index.Spot_Buff_ATK.ToString() },
            {(int)Index.Spot_Affect_Heal,           Index.Spot_Affect_Heal.ToString() },
            {(int)Index.Spot_Hit_打,                Index.Spot_Hit_打.ToString() },
            {(int)Index.Spot_Hit_炎,                Index.Spot_Hit_炎.ToString()},
            {(int)Index.Spot_Hit_氷,                Index.Spot_Hit_氷.ToString() },
            {(int)Index.Spot_Hit_通常,              Index.Spot_Hit_通常.ToString() },
            {(int)Index.Spot_HitText_Miss,          Index.Spot_HitText_Miss.ToString() },
            {(int)Index.Spot_HitText_Guard,         Index.Spot_HitText_Guard.ToString() },
            {(int)Index.Spot_Other_Lightning,       Index.Spot_Other_Lightning.ToString() },
            {(int)Index.Spot_Other_Glitter,         Index.Spot_Other_Glitter.ToString() },
        };

        private static readonly Dictionary<int, Index> m_AttributeTypeDic = new Dictionary<int, Index>()
        {
            {(int)AffectAttributeType.通常,         Index.Spot_Hit_通常 },
            {(int)AffectAttributeType.炎,           Index.Spot_Hit_炎 },
            {(int)AffectAttributeType.氷,           Index.Spot_Hit_氷 },
        };

        private static readonly Dictionary<int, Index> m_HitTextTypeDic = new Dictionary<int, Index>()
        {
            {(int)HitResultType.Miss,      Index.Spot_HitText_Miss },
            {(int)HitResultType.Guard,     Index.Spot_HitText_Guard},
        };

        // TODO : バフのパーティクルを決めたい場合ここで設定
        private static readonly Dictionary<int, Index> m_CategoryTypeDic = new Dictionary<int, Index>()
        {
            {(int)AffectCategoryType.ATK_Buff,        Index.Spot_Buff },
            {(int)AffectCategoryType.DEX_Buff,        Index.Spot_Buff },
            {(int)AffectCategoryType.DEF_Buff,        Index.Spot_Buff},
            {(int)AffectCategoryType.SPD_Buff,        Index.Spot_Buff},
            {(int)AffectCategoryType.MOVE_Buff,       Index.Spot_Buff },
            {(int)AffectCategoryType.ATK_Debuff,      Index.Spot_Debuff },
            {(int)AffectCategoryType.DEX_Debuff,      Index.Spot_Debuff },
            {(int)AffectCategoryType.DEF_Debuff,      Index.Spot_Debuff },
            {(int)AffectCategoryType.SPD_Debuff,      Index.Spot_Debuff },
            {(int)AffectCategoryType.MOVE_Debuff,     Index.Spot_Debuff },
            {(int)AffectCategoryType.HP_Buff,         Index.Spot_Buff },
            {(int)AffectCategoryType.HP_Debuff,       Index.Spot_Debuff },
            {(int)AffectCategoryType.Poison,          Index.Spot_Buff }, // TODO : 正しいエフェクト(Index)に後で考える
            {(int)AffectCategoryType.Burn,            Index.Spot_Hit_炎 },
            {(int)AffectCategoryType.Frost,           Index.Spot_Hit_氷},
            {(int)AffectCategoryType.HealFixed,       Index.Spot_Affect_Heal },
            {(int)AffectCategoryType.HealRatio,       Index.Spot_Affect_Heal },
            {(int)AffectCategoryType.RegenRatio,      Index.Spot_Affect_Heal },
            {(int)AffectCategoryType.RegenFixed,      Index.Spot_Affect_Heal },
        };

        #endregion

        public static string ToStringQuickly(this Index @this)
        {
            try { return m_IndexNameDic[(int)@this]; }
            catch { throw new KeyNotFoundException($"[index : {@this}] がエフェクト用の key として割り当てられていません。"); }
        }

        public static Index GetIndex(AffectAttributeType attributeType)
        {
            try { return m_AttributeTypeDic[(int)attributeType]; }
            catch { throw new KeyNotFoundException($"[{nameof(attributeType)} : {attributeType}] がエフェクト用の key として割り当てられていません。"); }
        }

        public static Index GetIndex(AffectCategoryType categoryType)
        {
            try { return m_CategoryTypeDic[(int)categoryType]; }
            catch { throw new KeyNotFoundException($"[{nameof(categoryType)} : {categoryType}] がエフェクト用の key として割り当てられていません。"); }
        }

        public static Index GetIndex(HitResultType hitType)
        {
            try { return m_HitTextTypeDic[(int)hitType]; }
            catch { throw new KeyNotFoundException($"[{nameof(hitType)} : {hitType}] がエフェクト用の key として割り当てられていません。"); }
        }
    }
}
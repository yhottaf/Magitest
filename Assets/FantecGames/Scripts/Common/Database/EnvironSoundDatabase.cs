using System.Collections.Generic;
using UnityEngine;
namespace fantec
{
    [CreateAssetMenu(menuName =nameof(fantec)+"/"+nameof(EnvironSoundDatabase))]
    public class EnvironSoundDatabase : ScriptableObject
    {
        [Header("ヒット音")]
        [SerializeField] AudioClip m_HitBlow;     // 打撃音
        [SerializeField] AudioClip m_HitSlash;    // 切り裂き音
        [SerializeField] AudioClip m_HitThrust;   // 突き音

        [Header("属性音")]
        [SerializeField] AudioClip m_AttHeat;

        [Header("効果反映音")]
        [SerializeField] AudioClip m_AffHeal;      // ヒール音
        [SerializeField] AudioClip m_AffPoison;    // 毒音
        [SerializeField] AudioClip m_AffBurn;      // バーン音
        [SerializeField] AudioClip m_AffFrost;     // 氷音
        [SerializeField] AudioClip m_AffDebuff;    // デバフ音
        [SerializeField] AudioClip m_AffConfusion;
        [SerializeField] AudioClip m_AffBuff;      // バフ音

        private Dictionary<int, AudioClip> m_HitDict;
        private Dictionary<int, AudioClip> m_AttributeDict;
        private Dictionary<int, AudioClip> m_CategoryDict;

        public void Regist()
        {
            m_HitDict = new Dictionary<int, AudioClip>()
            {
                //{(int)AffectHitType.打,m_HitBlow },
                //{(int)AffectHitType.斬,m_HitSlash },
                //{(int)AffectHitType.突,m_HitThrust },
            };

            m_CategoryDict = new Dictionary<int, AudioClip>()
            {

                // 状態異常
                //{ (int)AffectCategoryType.Poison, m_AffPoison },
                //{ (int)AffectCategoryType.Confusion, m_AffConfusion },
                //{ (int)AffectCategoryType.Burn, m_AffBurn },
                //{ (int)AffectCategoryType.Frost, m_AffFrost },
            };
        }

        public bool GetIsExist(AffectCategoryType categoryType)
        {
            return m_CategoryDict.ContainsKey((int)categoryType);
        }

        public AudioClip GetClip(AffectCategoryType categoryType)
        {
            if (GetIsExist(categoryType))
            {
                try { return m_CategoryDict[(int)categoryType]; }
                catch { throw new KeyNotFoundException($"[{categoryType}]"); }
            }
            else
            {
                Debug.LogWarning($"[{categoryType}]");
                return m_AffBuff;
            }
        }

        public AudioClip GetClip(AffectAttributeType attributeType)
        {
            try { return m_AttributeDict[(int)attributeType]; }
            catch { throw new KeyNotFoundException($"[{attributeType}]"); }
        }

        public AudioClip GetClip(AffectHitType hitType)
        {
            try { return m_HitDict[(int)hitType]; }
            catch { throw new KeyNotFoundException($"[{hitType}]"); }
        }
    }
}
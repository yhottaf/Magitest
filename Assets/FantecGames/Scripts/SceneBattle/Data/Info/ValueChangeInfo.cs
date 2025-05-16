using UnityEngine;

namespace fantec.Battle
{
    public struct ValueChangeInfo
    {
        // 計算の元となった値
        public int affectValue;

        // 新しい計算後の値
        public int newValue;

        // 元の値
        public int oldValue;

        // 最大値
        public int maxValue;

        /// <summary>
        /// 効果量の上限、下限を反映した値
        /// </summary>
        public int ClampedNewValue
        {
            get{return Mathf.Clamp(newValue, 0, maxValue);}
        }

        /// <summary>
        /// 正規化した値
        /// </summary>
        public float Normalized
        {
            get
            {
                // MEMO: 同じ値同士で割ると Nan になるため
                if (ClampedNewValue == maxValue) return 1;
                else return (float)ClampedNewValue / (float)maxValue;
            }
        }

        /// <summary>
        /// 反転させた正規化した値
        /// </summary>
        public float InversionNormalized
        {
            get { return 1 - Normalized; }
        }

        /// <summary>
        /// オーバーフローした値
        /// </summary>
        public int Overflow
        {
            get { return newValue - maxValue; }
        }

        /// <summary>
        /// オーバーフローした値で周回できる回数
        /// </summary>
        public int OverflowAroundCount
        {
            get { return newValue == 0 ? 0 : newValue / maxValue; }
        }

        /// <summary>
        /// オーバーフローの余り
        /// </summary>
        public int OverflowAroundSurplus
        {
            get { return newValue == 0 ? 0 : newValue % maxValue; }
        }

        /// <summary>
        /// 最大値であるか否か
        /// </summary>
        public bool IsLimit
        {
            get { return ClampedNewValue >= maxValue; }
        }
    }
}
using UnityEngine;

namespace fantec.Extensions
{
    public static class RectTransformExtensions
    {
        /// <summary>
        /// 親を設定するついでにポジションやスケールを基本値に設定する
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="parentTransform">親にしたいトランスフォーム</param>
        public static void FullStretch(this RectTransform rect)
        {
            rect.anchorMax = Vector2.one;
            rect.anchorMin = Vector2.zero;
            rect.offsetMax= Vector2.zero;
            rect.offsetMin = Vector2.zero;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.localScale = Vector3.one;
            rect.localPosition= Vector3.zero;
            rect.sizeDelta = Vector2.zero;
        }
    }
}

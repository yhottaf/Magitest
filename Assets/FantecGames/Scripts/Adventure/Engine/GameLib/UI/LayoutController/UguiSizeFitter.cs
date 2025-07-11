using UnityEngine;
using UnityEngine.UI;
namespace fantec
{

    /// <summary>
    /// サイズを指定オブジェクトと合わせる
    /// </summary>
    [ExecuteInEditMode]
    [AddComponentMenu("fantec/Lib/UI/UguiSizeFitter")]
    public class UguiSizeFitter : UguiLayoutControllerBase, ILayoutSelfController
    {
        public RectTransform target;
        protected override void Update()
        {
            if (target == null) return;

            if (target.rect.size != CachedRectTransform.rect.size)
            {
                SetDirty();
                return;
            }
        }

        public void SetLayoutHorizontal()
        {
            tracker.Clear();
            if (target == null) return;

            tracker.Add(this, CachedRectTransform,
                DrivenTransformProperties.SizeDelta);
            //ゲーム解像度設定
            CachedRectTransform.sizeDelta = target.sizeDelta;
        }

        public void SetLayoutVertical()
        {
        }
    }
}

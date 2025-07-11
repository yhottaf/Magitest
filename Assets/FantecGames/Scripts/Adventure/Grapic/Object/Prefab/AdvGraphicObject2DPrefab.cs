using UnityEngine;

namespace fantec
{

    /// <summary>
    /// フェード切り替え機能つきのスプライト表示
    /// </summary>
    [AddComponentMenu("fantec/ADV/Internal/GraphicObject/AdvGraphicObject2DPrefab")]
    public class AdvGraphicObject2DPrefab : AdvGraphicObjectPrefabBase
    {
        protected SpriteRenderer sprite;

        //********描画時のリソース変更********//
        protected override void ChangeResourceOnDrawSub(AdvGraphicInfo graphic)
        {
            this.sprite = currentObject.GetComponent<SpriteRenderer>();
        }


        //エフェクト用の色が変化したとき
        internal override void OnEffectColorsChange(AdvEffectColor color)
        {
            if (sprite == null) return;
            sprite.color = color.MulColor;
        }
    }
}

using fantec;
using System;
using UnityEngine;

namespace fantec
{
    [ExecuteInEditMode]
    [AddComponentMenu("fantec/Lib/Image Effects/Displacement/Twirl")]
    public class Twirl : ImageEffectSingelShaderBase
    {
        public Vector2 radius = new Vector2(0.3F, 0.3F);
        [Range(0.0f, 360.0f)]
        public float angle = 50;
        public Vector2 center = new Vector2(0.5F, 0.5F);

        //描画ロジック
        protected override void RenderImage(RenderTexture source, RenderTexture destination)
        {
            ImageEffectUtil.RenderDistortion(Material, source, destination, angle, center, radius);
        }
    }
}

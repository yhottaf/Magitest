using UnityEngine;

namespace fantec
{
    [ExecuteInEditMode]
    [AddComponentMenu("fantec/Lib/Image Effects/Color Adjustments/NegaPosi")]
    public class NegaPosi : ImageEffectSingelShaderBase
    {
        //描画ロジック
        protected override void RenderImage(RenderTexture source, RenderTexture destination)
        {
            Graphics.Blit(source, destination, Material);
        }
    }
}

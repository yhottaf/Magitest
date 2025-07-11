using UnityEngine;

namespace fantec
{

    /// <summary>
    /// シェーダーの管理
    /// </summary>
    public static class ShaderManager
    {
        //ルール画像付きのフェード処理をする場合のシェーダー
        static public Shader RuleFade { get { return Shader.Find("fantec/UI/RuleFade"); } }

        //背景を透過しないクロスフェード処理をする場合のシェーダー
        static public Shader CrossFade { get { return Shader.Find("fantec/CrossFadeImage"); } }

        //透過画像を描きこむシェーダー
        static public Shader RenderTexture { get { return Shader.Find("fantec/RenderTexture"); } }

        //透過画像を描き込んだRenderTextureを描画するシェーダー
        static public Shader DrawByRenderTexture { get { return Shader.Find("fantec/DrawByRenderTexture"); } }

        //カラーフェード
        static public string ColorFade = "fantec/ImageEffect/ColorFade";

        //ブルームシェーダー名
        static public string BloomName = "fantec/ImageEffect/Bloom";

        //ブラー
        static public string BlurName = "fantec/ImageEffect/Blur";

        //モザイク
        static public string MosaicName = "fantec/ImageEffect/Mosaic";

        //カラーコレクション（ランプ画像）
        static public string ColorCorrectionRampName = "fantec/ImageEffect/ColorCorrectionRamp";

        //グレースケール
        static public string GrayScaleName = "fantec/ImageEffect/Grayscale";

        //モーションブラー
        static public string MotionBlurName = "fantec/ImageEffect/MotionBlur";

        //ノイズ
        static public string NoiseAndGrainName = "fantec/ImageEffect/NoiseAndGrain";

        //オーバーレイ
        static public string BlendModesOverlayName = "fantec/ImageEffect/BlendModesOverlay";

        //セピア
        static public string SepiatoneName = "fantec/ImageEffect/Sepiatone";

        //ネガポジ反転
        static public string NegaPosiName = "fantec/ImageEffect/NegaPosi";

        //魚眼
        static public string FisheyeName = "fantec/ImageEffect/Fisheye";

        //一点を中心に画像を歪ませる
        static public string TwirlName = "fantec/ImageEffect/Twirl";

        //円で画像を歪ませる
        static public string VortexName = "fantec/ImageEffect/Vortex";

        //ルール画像付きのフェード
        static public string RuleFadeName = "fantec/ImageEffect/RuleFade";
    }
}

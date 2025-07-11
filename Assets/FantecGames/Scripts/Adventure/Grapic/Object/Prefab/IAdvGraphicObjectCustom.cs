namespace fantec
{

    /// <summary>
    /// カスタム機能つきのオブジェクト表示のインターフェース
    /// </summary>
    public interface IAdvGraphicObjectCustom
    {
        //描画時のリソース変更
        void ChangeResourceOnDrawSub(AdvGraphicInfo graphic);

        //エフェクト用の色が変化したとき
        void OnEffectColorsChange(AdvEffectColor color);
    }
}

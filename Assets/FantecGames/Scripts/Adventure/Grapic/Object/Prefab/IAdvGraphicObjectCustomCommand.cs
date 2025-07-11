namespace fantec
{

    /// <summary>
    /// カスタム機能つきのオブジェクト表示のインターフェース
    /// </summary>
    public interface IAdvGraphicObjectCustomCommand
    {
        //********描画時の引数適用********//
        void SetCommandArg(AdvCommand command);
    }
}

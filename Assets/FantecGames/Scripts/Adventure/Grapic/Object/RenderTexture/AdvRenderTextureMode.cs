namespace fantec
{
    /// <summary>
    /// テクスチャ書き込みのタイプ
    /// </summary>
    public enum AdvRenderTextureMode
    {
        None,           //
        Image,          //ImageやRawImageを描き込む（カスタムシェーダーを使う）
        DefaultShader,  //シェーダーそのまま使う
        Cusotm,         //
    }
}
namespace fantec
{

    //レイヤー変更時の座標の再設定のタイプ
    public enum AdvChangeLayerRepositionType
    {
        KeepGlobal, //グローバル座標を保持
        KeepLocal,  //ローカル座標を保持
        ResetLocal, //ローカル座標をリセット
    }
}

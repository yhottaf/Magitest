namespace fantec
{

    /// <summary>
    /// コマンド：フェードアウト処理
    /// </summary>
    internal class AdvCommandFadeOut : AdvCommandFadeBase
    {
        public AdvCommandFadeOut(StringGridRow row, AdvSettingDataManager dataManager)
            : base(row, dataManager, false)
        {
        }
    }
}

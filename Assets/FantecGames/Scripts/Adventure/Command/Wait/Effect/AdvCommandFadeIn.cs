namespace fantec
{

    /// <summary>
    /// コマンド：フェードイン処理
    /// </summary>
    internal class AdvCommandFadeIn : AdvCommandFadeBase
    {

        public AdvCommandFadeIn(StringGridRow row, AdvSettingDataManager dataManager)
            : base(row, dataManager, true)
        {
        }
    }
}

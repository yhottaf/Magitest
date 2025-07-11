namespace fantec
{

    /// <summary>
    /// コマンド：フェードアウト処理
    /// </summary>
    internal class AdvCommandImageEffectOff : AdvCommandImageEffectBase
    {
        public AdvCommandImageEffectOff(StringGridRow row, AdvSettingDataManager dataManager)
            : base(row, dataManager, true)
        {
        }
    }
}
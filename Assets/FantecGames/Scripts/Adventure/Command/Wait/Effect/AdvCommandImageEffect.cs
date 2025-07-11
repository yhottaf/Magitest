namespace fantec
{

    /// <summary>
    /// コマンド：イメージエフェクト開始
    /// </summary>
    internal class AdvCommandImageEffect : AdvCommandImageEffectBase
    {
        public AdvCommandImageEffect(StringGridRow row, AdvSettingDataManager dataManager)
            : base(row, dataManager, false)
        {
        }
    }
}
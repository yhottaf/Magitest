namespace fantec
{

    /// <summary>
    /// コマンド：選択肢追加終了
    /// </summary>
    internal class AdvCommandSelectionClickEnd : AdvCommand
    {
        public AdvCommandSelectionClickEnd(StringGridRow row, AdvSettingDataManager dataManager)
            : base(row)
        {
        }

        public override void DoCommand(AdvEngine engine)
        {
            engine.Config.StopSkipInSelection();
            engine.SelectionManager.Show();
        }
    }
}
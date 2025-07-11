namespace fantec
{

    /// <summary>
    /// コマンド：サブルーチンの終了
    /// </summary>
    internal class AdvCommandEndSubroutine : AdvCommand
    {
        public AdvCommandEndSubroutine(StringGridRow row, AdvSettingDataManager dataManager)
            : base(row)
        {
        }

        public override void DoCommand(AdvEngine engine)
        {
            CurrentTread.JumpManager.EndSubroutine();
        }
    }
}
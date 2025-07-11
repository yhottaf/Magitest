namespace fantec
{

    /// <summary>
    /// コマンド：ランダムジャンプ終了
    /// </summary>
    internal class AdvCommandJumpRandomEnd : AdvCommand
    {
        public AdvCommandJumpRandomEnd(StringGridRow row, AdvSettingDataManager dataManager)
            : base(row)
        {
        }

        public override void DoCommand(AdvEngine engine)
        {
            AdvCommandJumpRandom command = CurrentTread.JumpManager.GetRandomJumpCommand() as AdvCommandJumpRandom;
            if (command != null)
            {
                command.DoRandomEnd(engine, CurrentTread);
            }
        }
    }
}
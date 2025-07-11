namespace fantec
{

    /// <summary>
    /// コマンド：スレッドの終了
    /// </summary>
    internal class AdvCommandEndThread : AdvCommand
    {
        public AdvCommandEndThread(StringGridRow row)
            : base(row)
        {
        }

        public override void DoCommand(AdvEngine engine)
        {
            CurrentTread.IsPlaying = false;
        }
    }
}
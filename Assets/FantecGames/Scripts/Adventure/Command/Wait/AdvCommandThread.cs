namespace fantec
{

    /// <summary>
    /// コマンド：別スレッド作成
    /// </summary>
    internal class AdvCommandThread : AdvCommand
    {
        public AdvCommandThread(StringGridRow row)
            : base(row)
        {
            this.label = ParseScenarioLabel(AdvColumName.Arg1);
        }


        public override void DoCommand(AdvEngine engine)
        {
            CurrentTread.StartSubThread(label);
        }

        string label;
    }
}
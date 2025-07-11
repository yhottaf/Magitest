namespace fantec
{

    /// <summary>
    /// コマンド：テキスト表示（地の文）
    /// </summary>
    internal class AdvCommandWait : AdvCommand
    {

        public AdvCommandWait(StringGridRow row)
            : base(row)
        {
            this.time = ParseCell<float>(AdvColumName.Arg6);
        }

        public override void DoCommand(AdvEngine engine)
        {
            waitEndTime = engine.Time.Time + (engine.Page.CheckSkip() ? time / engine.Config.SkipSpeed : time);
        }

        public override bool Wait(AdvEngine engine)
        {
            return (engine.Time.Time < waitEndTime);
        }

        float time;
        float waitEndTime;
    }
}
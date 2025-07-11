namespace fantec
{

    /// <summary>
    /// コマンド：BGM停止
    /// </summary>
    internal class AdvCommandStopBgm : AdvCommand
    {
        public AdvCommandStopBgm(StringGridRow row)
            : base(row)
        {
            this.fadeTime = ParseCellOptional<float>(AdvColumName.Arg6, 0.2f);
        }

        public override void DoCommand(AdvEngine engine)
        {
            engine.SoundManager.StopBgm(fadeTime);
        }

        float fadeTime;
    }
}
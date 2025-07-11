namespace fantec
{

    /// <summary>
    /// コマンド：ボイス停止
    /// </summary>
    internal class AdvCommandStopVoice : AdvCommand
    {
        public AdvCommandStopVoice(StringGridRow row)
            : base(row)
        {
            this.fadeTime = ParseCellOptional<float>(AdvColumName.Arg6, 0.2f);
        }

        public override void DoCommand(AdvEngine engine)
        {
            engine.SoundManager.StopVoice(fadeTime);
        }

        float fadeTime;
    }
}
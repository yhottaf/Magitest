namespace fantec
{

    /// <summary>
    /// コマンド：環境音停止
    /// </summary>
    internal class AdvCommandStopAmbience : AdvCommand
    {
        public AdvCommandStopAmbience(StringGridRow row)
            : base(row)
        {
            this.fadeTime = ParseCellOptional<float>(AdvColumName.Arg6, 0.2f);
        }

        public override void DoCommand(AdvEngine engine)
        {
            engine.SoundManager.StopAmbience(fadeTime);
        }

        float fadeTime;
    }
}
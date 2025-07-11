namespace fantec
{

    /// <summary>
    /// コマンド：背景表示OFF
    /// </summary>
    internal abstract class AdvCommandBgOffBase : AdvCommand
    {
        protected float fadeTime;
        protected AdvCommandBgOffBase(StringGridRow row)
            : base(row)
        {
            this.fadeTime = ParseCellOptional<float>(AdvColumName.Arg6, 0.2f);
        }

        public override void DoCommand(AdvEngine engine)
        {
            engine.GraphicManager.BgManager.FadeOutAll(engine.Page.ToSkippedTime(this.fadeTime));
        }
    }
}
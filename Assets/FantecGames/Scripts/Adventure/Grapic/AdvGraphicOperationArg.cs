namespace fantec
{

    /// <summary>
    /// グラフィック描画のための引数としてのデータ
    /// </summary>
    public class AdvGraphicOperationArg
    {
        float FadeTime { get; set; }
        public float GetSkippedFadeTime(AdvEngine engine)
        {
            return engine.Page.ToSkippedTime(FadeTime);
        }

        AdvCommand Command { get; set; }
        public AdvGraphicInfo Graphic { get; private set; }

        internal AdvGraphicOperationArg(AdvCommand command, AdvGraphicInfo graphic, float fadeTime)
        {
            this.Command = command;
            this.Graphic = graphic;
            this.FadeTime = fadeTime;
        }
    }
}

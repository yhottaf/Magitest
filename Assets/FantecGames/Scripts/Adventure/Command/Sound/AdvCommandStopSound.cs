namespace fantec
{

    /// <summary>
    /// コマンド：サウンド停止
    /// </summary>
    internal class AdvCommandStopSound : AdvCommand
    {
        public AdvCommandStopSound(StringGridRow row)
            : base(row)
        {
            this.groups = ParseCellOptionalArray<string>(AdvColumName.Arg1, new string[] { SoundManager.IdBgm, SoundManager.IdAmbience });
            this.fadeTime = ParseCellOptional<float>(AdvColumName.Arg6, fadeTime);
        }

        public override void DoCommand(AdvEngine engine)
        {
            if (groups.Length == 1 && groups[0] == "All")
            {
                engine.SoundManager.StopAll(fadeTime);
            }
            else
            {
                engine.SoundManager.StopGroups(groups, fadeTime);
            }
        }

        string[] groups;
        float fadeTime = 0.15f;
    }
}
namespace fantec
{

    /// <summary>
    /// コマンド：シェイク表示
    /// </summary>
    internal class AdvCommandShake : AdvCommandTween
    {
        public AdvCommandShake(StringGridRow row, AdvSettingDataManager dataManager)
            : base(row, dataManager)
        {
        }

        //Tweenデータの初期化
        protected override void InitTweenData()
        {
            string defaultStr = " x=30 y=30";
            string arg = ParseCellOptional<string>(AdvColumName.Arg3, defaultStr);
            if (!arg.Contains("x=") && !arg.Contains("y="))
            {
                arg += defaultStr;
            }
            string easeType = ParseCellOptional<string>(AdvColumName.Arg4, "");
            string loopType = ParseCellOptional<string>(AdvColumName.Arg5, "");
            this.tweenData = new iTweenData(iTweenType.ShakePosition.ToString(), arg, easeType, loopType);
        }
    }
}

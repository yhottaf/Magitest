namespace fantec
{

    // コマンド：オブジェクト単位のフェードアウト
    internal class AdvCommandWaitEffectTime : AdvCommandWaitBase
        , IAdvCommandEffect
        , IAdvCommandUpdateWait
    {
        float time = 0;
        float waitEndTime = 0;
        AdvEngine Engine { get; set; }
        AdvScenarioThread Thread { get; set; }

        internal AdvCommandWaitEffectTime(StringGridRow row, AdvSettingDataManager dataManager)
            : base(row)
        {
            this.time = ParseCell<float>(AdvColumName.Arg6);
            WaitType = ParseCellOptional(AdvColumName.WaitType, AdvCommandWaitType.Default);
        }

        //開始時のコールバック
        protected override void OnStart(AdvEngine engine, AdvScenarioThread thread)
        {
            waitEndTime = engine.Time.Time + (engine.Page.CheckSkip() ? time / engine.Config.SkipSpeed : time);
            Engine = engine;
            Thread = thread;
        }

        //コマンド終了待ち
        public bool UpdateCheckWait()
        {
            return (Engine.Time.Time < waitEndTime);
        }

        public void OnEffectFinalize()
        {
            Engine = null;
            Thread = null;
        }

        public void OnEffectSkip()
        {
            waitEndTime = 0;
        }
    }
}

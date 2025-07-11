namespace fantec
{

    /// <summary>
    /// コマンド：MessageWindow操作　ChangeCurrent
    /// </summary>
    internal class AdvCommandMessageWindowChangeCurrent : AdvCommand
    {
        public AdvCommandMessageWindowChangeCurrent(StringGridRow row)
            : base(row)
        {
            this.name = ParseCell<string>(AdvColumName.Arg1);
        }

        /// <summary>
        /// ページ用のデータからコマンドに必要な情報を初期化
        /// </summary>
        /// <param name="pageData"></param>
        public override void InitFromPageData(AdvScenarioPageData pageData)
        {
            pageData.InitMessageWindowName(this, name);
        }

        public override void DoCommand(AdvEngine engine)
        {
            engine.MessageWindowManager.ChangeCurrentWindow(name);
        }

        string name;
    }
}

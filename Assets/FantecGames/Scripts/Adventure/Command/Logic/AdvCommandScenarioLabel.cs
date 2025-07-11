namespace fantec
{

    /// <summary>
    /// コマンド：シナリオラベル
    /// </summary>
    public class AdvCommandScenarioLabel : AdvCommand
    {
        public AdvCommandScenarioLabel(StringGridRow row)
            : base(row)
        {
            this.ScenarioLabel = ParseScenarioLabel(AdvColumName.Command);
            this.Type = ParseCellOptional<ScenarioLabelType>(AdvColumName.Arg1, ScenarioLabelType.None);
        }


        public override void DoCommand(AdvEngine engine)
        {
        }

        public enum ScenarioLabelType
        {
            None,
            SavePoint,
        };
        public string ScenarioLabel { get; protected set; }

        public ScenarioLabelType Type { get; protected set; }
        public string Title
        {
            get
            {
                string title = ParseCellOptional<string>(AdvColumName.Arg2, "");
                if (string.IsNullOrEmpty(title)) return "";

                return ParseCellLocalized(AdvColumName.Arg2.QuickToString());
            }
        }
    }
}
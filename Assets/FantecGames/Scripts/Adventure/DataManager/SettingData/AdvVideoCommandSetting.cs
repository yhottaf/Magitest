namespace fantec
{
    /// <summary>
    /// コマンドから直接ファイル設定がある場合のデータ（今のところビデオのみ）
    /// </summary>
    public class AdvCommandSetting : IAssetFileSettingData
    {
        public AdvCommand Command { get; private set; }
        public StringGridRow RowData { get { return Command.RowData; } }

        public AdvCommandSetting(AdvCommand command)
        {
            this.Command = command;
        }
    }
}
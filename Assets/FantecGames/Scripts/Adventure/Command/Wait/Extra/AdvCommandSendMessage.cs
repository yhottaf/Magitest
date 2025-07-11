using fantecExtensions;

namespace fantec
{

    /// <summary>
    /// コマンド：ゲーム固有の独自処理のためにSendMessageをする
    /// </summary>
    public class AdvCommandSendMessage : AdvCommand
    {
        public AdvCommandSendMessage(StringGridRow row)
            : base(row)
        {
            this.name = ParseCell<string>(AdvColumName.Arg1);
            this.arg2 = ParseCellOptional<string>(AdvColumName.Arg2, "");
            this.arg3 = ParseCellOptional<string>(AdvColumName.Arg3, "");
            this.arg4 = ParseCellOptional<string>(AdvColumName.Arg4, "");
            this.arg5 = ParseCellOptional<string>(AdvColumName.Arg5, "");
            this.arg6 = ParseCellOptional<string>(AdvColumName.Arg6, "");
            this.voice = ParseCellOptional<string>(AdvColumName.Voice, "");
            this.voiceVersion = ParseCellOptional<int>(AdvColumName.VoiceVersion, 0);
        }

        public override void DoCommand(AdvEngine engine)
        {
            this.text = ParseCellLocalizedText();
            engine.ScenarioPlayer.SendMessageTarget.SafeSendMessage("OnDoCommand", this);
        }

        public override bool Wait(AdvEngine engine)
        {
            engine.ScenarioPlayer.SendMessageTarget.SafeSendMessage("OnWait", this);
            return IsWait;
        }

        /// <summary>
        /// コマンドの待機処理をするか
        /// </summary>
        public bool IsWait { get { return isWait; } set { isWait = value; } }
        bool isWait = false;

        /// <summary>
        /// 名前
        /// </summary>
        public string Name { get { return name; } }
        string name;

        /// <summary>
        /// 引数2
        /// </summary>
        public string Arg2 { get { return arg2; } }
        string arg2;

        /// <summary>
        /// 引数3
        /// </summary>
        public string Arg3 { get { return arg3; } }
        string arg3;

        /// <summary>
        /// 引数4
        /// </summary>
        public string Arg4 { get { return arg4; } }
        string arg4;

        /// <summary>
        /// 引数5
        /// </summary>
        public string Arg5 { get { return arg5; } }
        string arg5;

        /// <summary>
        /// 引数6
        /// </summary>
        public string Arg6 { get { return arg6; } }
        string arg6;

        /// <summary>
        /// テキスト
        /// </summary>
        public string Text { get { return text; } }
        string text;

        /// <summary>
        /// ボイス
        /// </summary>
        public string Voice { get { return voice; } }
        string voice;


        /// <summary>
        /// ボイスバージョン
        /// </summary>
        public int VoiceVersion { get { return voiceVersion; } }
        int voiceVersion;
    }
}
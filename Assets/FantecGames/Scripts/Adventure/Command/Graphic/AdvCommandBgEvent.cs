namespace fantec
{

    /// <summary>
    /// コマンド：背景表示・切り替え
    /// </summary>
    internal class AdvCommandBgEvent : AdvCommandBgBase
    {
        public AdvCommandBgEvent(StringGridRow row, AdvSettingDataManager dataManager)
            : base(row, dataManager)
        {
        }

        public override void DoCommand(AdvEngine engine)
        {
            engine.GraphicManager.IsEventMode = true;
            //表示する
            AdvGraphicOperationArg graphicOperationArg = DoCommandBgSub(engine);
            //キャラクターは非表示にする
            engine.GraphicManager.CharacterManager.FadeOutAll(graphicOperationArg.GetSkippedFadeTime(engine));
        }
    }
}
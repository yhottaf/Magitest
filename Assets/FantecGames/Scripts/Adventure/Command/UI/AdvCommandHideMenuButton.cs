using UnityEngine;

namespace fantec
{
    /// <summary>
    /// コマンド: メニューボタンを非表示
    /// </summary>
    internal class AdvCommandHideMenuButton : AdvCommand
    {
        public AdvCommandHideMenuButton(StringGridRow row):base(row)
        {

        }

        public override void DoCommand(AdvEngine engine)
        {
            engine.UiManager.HideMenuButton();
        }
    }
}
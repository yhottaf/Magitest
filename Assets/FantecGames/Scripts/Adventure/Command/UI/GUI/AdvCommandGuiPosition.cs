using UnityEngine;

namespace fantec
{

    /// <summary>
    /// コマンド：GUI操作　Position
    /// </summary>
    internal class AdvCommandGuiPosition : AdvCommand
    {
        public AdvCommandGuiPosition(StringGridRow row)
            : base(row)
        {
            this.name = this.ParseCell<string>(AdvColumName.Arg1);
            this.x = this.ParseCellOptionalNull<float>(AdvColumName.Arg2);
            this.y = this.ParseCellOptionalNull<float>(AdvColumName.Arg3);
        }

        public override void DoCommand(AdvEngine engine)
        {
            AdvGuiBase gui;
            if (!engine.UiManager.GuiManager.TryGet(this.name, out gui))
            {
                Debug.LogError(this.ToErrorString(name + " is not found in GuiManager"));
            }
            else
            {
                gui.SetPosition(x, y);
            }
        }

        string name;
        float? x;
        float? y;
    }
}

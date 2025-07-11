using UnityEngine;


namespace fantec
{

    /// <summary>
    /// コマンド：GUI操作　Size
    /// </summary>
    internal class AdvCommandGuiSize : AdvCommand
    {
        public AdvCommandGuiSize(StringGridRow row)
            : base(row)
        {
            this.name = this.ParseCellOptional<string>(AdvColumName.Arg1, "");
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
                gui.SetSize(x, y);
            }
        }

        string name;
        float? x;
        float? y;

    }
}

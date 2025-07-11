using UnityEngine;


namespace fantec
{

    /// <summary>
    /// コマンド：GUI操作　Active
    /// </summary>
    internal class AdvCommandGuiActive : AdvCommand
    {
        public AdvCommandGuiActive(StringGridRow row)
            : base(row)
        {
            this.name = this.ParseCellOptional<string>(AdvColumName.Arg1, "");
            this.isActive = this.ParseCell<bool>(AdvColumName.Arg2);
        }

        public override void DoCommand(AdvEngine engine)
        {
            if (string.IsNullOrEmpty(name))
            {
                foreach (var obj in engine.UiManager.GuiManager.Objects.Values)
                {
                    obj.SetActive(this.isActive);
                }
            }
            else
            {
                AdvGuiBase gui;
                if (!engine.UiManager.GuiManager.TryGet(this.name, out gui))
                {
                    Debug.LogError(this.ToErrorString(name + " is not found in GuiManager"));
                }
                else
                {
                    gui.SetActive(this.isActive);
                }
            }
        }

        string name;
        bool isActive;
    }
}

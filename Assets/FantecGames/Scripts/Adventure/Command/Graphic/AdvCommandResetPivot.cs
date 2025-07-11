using UnityEngine;

namespace fantec
{
    /// <summary>
    /// コマンド：ピボットのリセット
    /// </summary>
    internal class AdvCommandResetPivot : AdvCommand
    {
        private readonly string targetName;
        public AdvCommandResetPivot(StringGridRow row, AdvSettingDataManager dataManager)
            : base(row)
        {
            targetName = this.ParseCell<string>(AdvColumName.Arg1);
        }

        public override void DoCommand(AdvEngine engine)
        {
            var target = engine.GraphicManager.FindObject(targetName);
            if (target == null)
            {
                Debug.LogError(targetName + " is not found");
                return;
            }
            target.ResetPivot();
        }
    }
}

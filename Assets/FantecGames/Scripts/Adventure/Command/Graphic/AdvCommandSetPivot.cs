using UnityEngine;

namespace fantec
{
    // コマンド：ピボットの設定
    internal class AdvCommandSetPivot : AdvCommand
    {
        private readonly string targetName;
        private readonly float pivotX;
        private readonly float pivotY;
        private readonly float x;
        private readonly float y;
        private readonly AdvGraphicObjectPivotType pivotType;

        public AdvCommandSetPivot(StringGridRow row, AdvSettingDataManager dataManager)
            : base(row)
        {
            targetName = this.ParseCell<string>(AdvColumName.Arg1);

            string strPivotX = ParseCell<string>(AdvColumName.Arg2);
            switch (strPivotX)
            {
                case AdvCommandKeyword.Left:
                    pivotX = 0.0f;
                    break;
                case AdvCommandKeyword.Center:
                    pivotX = 0.5f;
                    break;
                case AdvCommandKeyword.Right:
                    pivotX = 1.0f;
                    break;
                default:
                    pivotX = this.ParseCell<float>(AdvColumName.Arg2);
                    break;
            }

            string strPivotY = ParseCell<string>(AdvColumName.Arg3);
            switch (strPivotY)
            {
                case AdvCommandKeyword.Bottom:
                    pivotY = 0.0f;
                    break;
                case AdvCommandKeyword.Center:
                    pivotY = 0.5f;
                    break;
                case AdvCommandKeyword.Top:
                    pivotY = 1.0f;
                    break;
                default:
                    pivotY = this.ParseCell<float>(AdvColumName.Arg3);
                    break;
            }

            x = this.ParseCellOptional<float>(AdvColumName.Arg4, 0.0f);
            y = this.ParseCellOptional<float>(AdvColumName.Arg5, 0.0f);
            pivotType = this.ParseCellOptional(AdvColumName.Arg6, AdvGraphicObjectPivotType.SpritePos);
        }

        public override void DoCommand(AdvEngine engine)
        {
            var target = engine.GraphicManager.FindObject(targetName);
            if (target == null)
            {
                Debug.LogError(targetName + " is not found");
                return;
            }
            target.SetPivot(pivotX, pivotY, x, y, pivotType);
        }
    }
}
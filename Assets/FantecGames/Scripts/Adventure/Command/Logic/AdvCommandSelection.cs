using UnityEngine;

namespace fantec
{

    /// <summary>
    /// コマンド：選択肢表示
    /// </summary>
    public class AdvCommandSelection : AdvCommand
    {

        public AdvCommandSelection(StringGridRow row, AdvSettingDataManager dataManager)
            : base(row)
        {
            this.jumpLabel = ParseScenarioLabel(AdvColumName.Arg1);
            string expStr = ParseCellOptional<string>(AdvColumName.Arg2, "");
            if (string.IsNullOrEmpty(expStr))
            {
                this.exp = null;
            }
            else
            {
                this.exp = dataManager.DefaultParam.CreateExpressionBoolean(expStr);
                if (this.exp.ErrorMsg != null)
                {
                    Debug.LogError(ToErrorString(this.exp.ErrorMsg));
                }
            }

            string selectedExpStr = ParseCellOptional<string>(AdvColumName.Arg3, "");
            if (string.IsNullOrEmpty(selectedExpStr))
            {
                this.selectedExp = null;
            }
            else
            {
                this.selectedExp = dataManager.DefaultParam.CreateExpression(selectedExpStr);
                if (this.selectedExp.ErrorMsg != null)
                {
                    Debug.LogError(ToErrorString(this.selectedExp.ErrorMsg));
                }
            }
            this.prefabName = ParseCellOptional<string>(AdvColumName.Arg4, "");
            this.x = ParseCellOptionalNull<float>(AdvColumName.Arg5);
            this.y = ParseCellOptionalNull<float>(AdvColumName.Arg6);

            //エディター用のチェック
            if (AdvCommand.IsEditorErrorCheck)
            {
                TextData textData = new TextData(ParseCellLocalizedText());
                if (!string.IsNullOrEmpty(textData.ErrorMsg))
                {
                    Debug.LogError(ToErrorString(textData.ErrorMsg));
                }
            }
        }

        public override void DoCommand(AdvEngine engine)
        {
            if (exp == null || engine.Param.CalcExpressionBoolean(exp))
            {
                engine.SelectionManager.AddSelection(jumpLabel, ParseCellLocalizedText(), selectedExp, prefabName, x, y, RowData);
            }
        }

        public override string[] GetJumpLabels() { return new string[] { jumpLabel }; }
        // 選択肢終了などの特別なコマンドを自動生成する場合、そのIDを返す
        public override string[] GetExtraCommandIdArray(AdvCommand next)
        {
            if (next != null && ((next is AdvCommandSelection) || (next is AdvCommandSelectionClick)))
            {
                return null;
            }
            else
            {
                if (AdvPageController.IsPageEndType(ParseCellOptional<AdvPageControllerType>(AdvColumName.PageCtrl, AdvPageControllerType.InputBrPage)))
                {
                    return new string[] { AdvCommandParser.IdSelectionEnd, AdvCommandParser.IdPageControler };
                }
                else
                {
                    return new string[] { AdvCommandParser.IdSelectionEnd };
                }
            }
        }

        //ページ区切り系のコマンドか
        public override bool IsTypePage() { return true; }

        string jumpLabel;
        ExpressionParser exp;               //選択肢表示条件式
        ExpressionParser selectedExp;       //選択後に実行する演算式
        string prefabName;
        float? x;
        float? y;
    }
}
using System.Collections.Generic;
using UnityEngine;

namespace fantec
{
    /// <summary>
    /// コマンド:MessageWindow操作 初期化
    /// </summary>
    internal class AdvCommandMessageWindowInit : AdvCommand
    {
        List<string>names=new List<string>();
        public AdvCommandMessageWindowInit(StringGridRow row) : base(row)
        {
            if (!IsEmptyCell(AdvColumName.Arg1)) AddName(ParseCell<string>(AdvColumName.Arg1));
            if (!IsEmptyCell(AdvColumName.Arg2)) AddName(ParseCell<string>(AdvColumName.Arg2));
            if (!IsEmptyCell(AdvColumName.Arg3)) AddName(ParseCell<string>(AdvColumName.Arg3));
            if (!IsEmptyCell(AdvColumName.Arg4)) AddName(ParseCell<string>(AdvColumName.Arg4));
            if (!IsEmptyCell(AdvColumName.Arg5)) AddName(ParseCell<string>(AdvColumName.Arg5));
            if (!IsEmptyCell(AdvColumName.Arg6)) AddName(ParseCell<string>(AdvColumName.Arg6));
            if (names.Count <= 0)
            {
                Debug.LogError(ToErrorString("Not set data in this command"));
            }
        }

        void AddName(string name)
        {
            if (names.Contains(name))
            {
                Debug.LogError(ToErrorString(name + " is duplicated. You cannot use the same message windows name more than once."));
                return;
            }
            names.Add(name);
        }

        /// <summary>
        /// ページ用のデータからコマンドに必要な情報を初期化
        /// </summary>
        /// <param name="pageData"></param>
        public override void InitFromPageData(AdvScenarioPageData pageData)
        {
            if(names.Count>0)
            {
                pageData.InitMessageWindowName(this, names[0]);
            }
        }

        public override void DoCommand(AdvEngine engine)
        {
            engine.MessageWindowManager.ChangeActiveWindows(names);
            engine.MessageWindowManager.ChangeCurrentWindow(engine.Page.CurrentData.MessageWindowName);
        }
    }
}
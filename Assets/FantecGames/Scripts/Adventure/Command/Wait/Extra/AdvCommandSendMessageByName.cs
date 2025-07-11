using fantecExtensions;
using UnityEngine;

namespace fantec
{

    /// <summary>
    /// コマンド：ゲーム固有の独自処理のためにSendMessageをする
    /// </summary>
    public class AdvCommandSendMessageByName : AdvCommand
    {
        public AdvCommandSendMessageByName(StringGridRow row)
            : base(row)
        {
        }

        public override void DoCommand(AdvEngine engine)
        {
            Engine = engine;
            string name = ParseCell<string>(AdvColumName.Arg1);
            string function = ParseCell<string>(AdvColumName.Arg2);
            GameObject go = GameObject.Find(name);
            if (go == null)
            {
                Debug.LogError(name + " is not found in current scene");
            }
            else
            {
                go.SafeSendMessage(function, this);
            }
        }

        public override bool Wait(AdvEngine engine)
        {
            return IsWait;
        }

        public bool IsWait { get; set; }
        public AdvEngine Engine { get; private set; }
    }
}
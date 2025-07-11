using UnityEngine;

namespace fantec
{

    /// <summary>
    /// コマンド：ゲーム固有の独自処理のためにSendMessageをする
    /// </summary>
    public class AdvCommandBroadcastMessageByName : AdvCommand
    {
        enum TargetType
        {
            Default,
            FantecObject,
            RenderTexture,
        }
        readonly string name;
        readonly string function;
        readonly TargetType targetType;
        public bool IsWait { get; set; }
        public AdvEngine Engine { get; private set; }
        public AdvCommandBroadcastMessageByName(StringGridRow row)
            : base(row)
        {
            name = ParseCell<string>(AdvColumName.Arg1);
            function = ParseCell<string>(AdvColumName.Arg2);
            targetType = ParseCellOptional(AdvColumName.Arg3, TargetType.Default);
        }

        public override void DoCommand(AdvEngine engine)
        {
            Engine = engine;
            GameObject target = FindTarget(engine);
            if (target == null) return;

            target.BroadcastMessage(function, this, SendMessageOptions.RequireReceiver);
        }

        GameObject FindTarget(AdvEngine engine)
        {
            GameObject target = null;
            switch (targetType)
            {
                case TargetType.FantecObject:
                    target = engine.GraphicManager.FindObjectOrLayer(name);
                    if (target == null)
                    {
                        Debug.LogError(name + " is not found in Fantec Objects");
                    }
                    break;
                case TargetType.RenderTexture:
                    AdvGraphicObject obj = engine.GraphicManager.FindObject(name);
                    if (obj == null)
                    {
                        Debug.LogError(name + " is not found in Fantec Objects");
                    }
                    else
                    {
                        target = obj.TargetObject.gameObject;
                    }
                    break;
                case TargetType.Default:
                default:
                    target = GameObject.Find(name);
                    if (target == null)
                    {
                        Debug.LogError(name + " is not found in current scene");
                    }
                    break;
            }
            return target;
        }

        public override bool Wait(AdvEngine engine)
        {
            return IsWait;
        }
    }
}
using UnityEngine;

namespace fantec
{

    /// <summary>
    /// コマンド：Tweenアニメーションをする
    /// </summary>
    public class AdvCommandTween : AdvCommandEffectBase
        , IAdvCommandEffect
    {
        protected iTweenData tweenData;
        private AdvITweenPlayer Player { get; set; }

        public AdvCommandTween(StringGridRow row, AdvSettingDataManager dataManager)
            : base(row, dataManager)
        {
            //Tweenデータの初期化
            InitTweenData();

            //ストップの場合のみ、特殊
            if (this.tweenData.Type == iTweenType.Stop)
            {
                this.WaitType = AdvCommandWaitType.Add;
            }

            //エラーチェック
            if (!string.IsNullOrEmpty(tweenData.ErrorMsg))
            {
                Debug.LogError(ToErrorString(tweenData.ErrorMsg));
            }
        }


        //解析必要に応じてオーバーライド
        protected override void OnParse(AdvSettingDataManager dataManager)
        {
            ParseEffectTarget(AdvColumName.Arg1);

            //ウェイトタイプ設定されているなら、それを優先
            if (!IsEmptyCell(AdvColumName.WaitType))
            {
                ParseWait(AdvColumName.WaitType);
            }
            else if (!IsEmptyCell(AdvColumName.Arg6))
            {
                //ウェイトタイプがなく、Arg6がある
#if UNITY_EDITOR
                if (AdvCommand.IsEditorErrorCheck && AdvCommand.IsEditorErrorCheckWaitType)
                {
                    Debug.LogWarning(this.ToErrorString("Please use 'WaitType' Column"));
                }
#endif
                ParseWait(AdvColumName.Arg6);
            }
            else
            {
                ParseWait(AdvColumName.WaitType);
            }
        }

        //Tweenデータの初期化
        protected virtual void InitTweenData()
        {
            string type = ParseCell<string>(AdvColumName.Arg2);
            string arg = ParseCellOptional<string>(AdvColumName.Arg3, "");
            string easeType = ParseCellOptional<string>(AdvColumName.Arg4, "");
            string loopType = ParseCellOptional<string>(AdvColumName.Arg5, "");
            this.tweenData = new iTweenData(type, arg, easeType, loopType);
        }

        //エフェクト開始時のコールバック
        protected override void OnStartEffect(GameObject target, AdvEngine engine, AdvScenarioThread thread)
        {
            if (!string.IsNullOrEmpty(tweenData.ErrorMsg))
            {
                Debug.LogError(tweenData.ErrorMsg);
                OnComplete(thread);
                return;
            }
            Player = target.AddComponent<AdvITweenPlayer>();
            float skipSpeed = engine.Page.CheckSkip() ? engine.Config.SkipSpeed : 0;

            Player.Init(tweenData, IsUnder2DSpace(target), engine.GraphicManager.PixelsToUnits, skipSpeed, engine.Time.Unscaled,
                (x) =>
                {
                    Player = null;
                    OnComplete(thread);
                });
            Player.Play();
            if (Player.IsEndlessLoop)
            {
                //				waitType = EffectWaitType.Add;
            }
        }

        //2D座標以下にあるか
        bool IsUnder2DSpace(GameObject target)
        {
            switch (this.targetType)
            {
                case AdvEffectManager.TargetType.MessageWindow:
                    return true;
                case AdvEffectManager.TargetType.Default:
                    return target.GetComponent<AdvGraphicObject>() != null;
                default:
                    return false;
            }
        }

        public void OnEffectSkip()
        {
            if (Player == null)
            {
                Debug.LogError(" cant skip tween effect");
            }
            Player.SkipToEnd();
        }

        public void OnEffectFinalize()
        {
            Player = null;
        }
    }
}

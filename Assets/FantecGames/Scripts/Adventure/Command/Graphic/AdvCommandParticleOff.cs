namespace fantec
{

    /// <summary>
    /// コマンド：パーティクル表示
    /// </summary>
    internal class AdvCommandParticleOff : AdvCommand
    {
        string name;
        AdvParticleStopType stopType;
        public AdvCommandParticleOff(StringGridRow row)
            : base(row)
        {
            this.name = ParseCellOptional<string>(AdvColumName.Arg1, "");
            this.stopType = ParseCellOptional<AdvParticleStopType>(AdvColumName.Arg2, AdvParticleStopType.Default);
        }

        public override void DoCommand(AdvEngine engine)
        {
            if (string.IsNullOrEmpty(name))
            {
                engine.GraphicManager.FadeOutAllParticle(stopType);
            }
            else
            {
                if (engine.GraphicManager.FindParticle(name) != null)
                {
                    engine.GraphicManager.FadeOutParticle(name, stopType);
                }
                else
                {
                    var layer = engine.GraphicManager.FindLayer(name);
                    if (layer != null)
                    {
                        //消す
                        layer.FadeOutAllParticle(stopType);
                    }
                    else
                    {
                        //パーティクルの場合は自動で消えている可能性があるので
                        //						Debug.LogError("Not found " + name + " Please input particle name or layer name");
                    }
                }
            }
        }
    }
}
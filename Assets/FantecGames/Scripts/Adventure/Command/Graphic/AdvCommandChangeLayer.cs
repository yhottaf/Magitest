using UnityEngine;

namespace fantec
{

    // コマンド：レイヤー変更
    internal class AdvCommandChangeLayer : AdvCommand
    {
        readonly string objectName;
        readonly string layerName;
        readonly AdvChangeLayerRepositionType repositionType;
        readonly float fadeTime;
        public AdvCommandChangeLayer(StringGridRow row, AdvSettingDataManager dataManager)
            : base(row)
        {
            this.objectName = ParseCell<string>(AdvColumName.Arg1);
            this.repositionType = ParseCellOptional(AdvColumName.Arg2, AdvChangeLayerRepositionType.KeepGlobal);
            this.layerName = ParseCell<string>(AdvColumName.Arg3);
            if (!dataManager.LayerSetting.Contains(layerName))
            {
                Debug.LogError(row.ToErrorString("Not found " + layerName + " Please input Layer name"));
            }
            fadeTime = ParseCellOptional(AdvColumName.Arg6, 0.2f);
        }

        public override void DoCommand(AdvEngine engine)
        {
            //レイヤー変更
            engine.GraphicManager.ChangeLayer(objectName, layerName, repositionType, engine.Page.ToSkippedTime(fadeTime));
        }
    }
}

using UnityEngine;

namespace fantec
{

    /// <summary>
    /// コマンド：環境音再生
    /// </summary>
    internal class AdvCommandAmbience : AdvCommand
    {
        public AdvCommandAmbience(StringGridRow row, AdvSettingDataManager dataManager)
            : base(row)
        {
            string label = ParseCell<string>(AdvColumName.Arg1);
            if (!dataManager.SoundSetting.Contains(label, SoundType.Ambience))
            {
                Debug.LogError(ToErrorString(label + " is not contained in file setting"));
            }

            this.file = AddLoadFile(dataManager.SoundSetting.LabelToFilePath(label, SoundType.Ambience), dataManager.SoundSetting.FindData(label));
            this.isLoop = ParseCellOptional<bool>(AdvColumName.Arg2, false);
            this.volume = ParseCellOptional<float>(AdvColumName.Arg3, 1.0f);
            this.fadeOutTime = ParseCellOptional<float>(AdvColumName.Arg5, 0.2f);
            this.fadeInTime = ParseCellOptional<float>(AdvColumName.Arg6, 0);
        }
        public override void DoCommand(AdvEngine engine)
        {
            engine.SoundManager.PlayAmbience(file, volume, isLoop, fadeInTime, fadeOutTime);
        }
        AssetFile file;
        float volume;
        bool isLoop;
        float fadeInTime;
        float fadeOutTime;
    }
}
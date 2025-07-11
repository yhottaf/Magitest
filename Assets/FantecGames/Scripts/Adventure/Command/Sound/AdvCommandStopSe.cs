using UnityEngine;

namespace fantec
{

    /// <summary>
    /// コマンド：Se停止
    /// </summary>
    internal class AdvCommandStopSe : AdvCommand
    {
        public AdvCommandStopSe(StringGridRow row, AdvSettingDataManager dataManager)
            : base(row)
        {
            this.label = ParseCellOptional<string>(AdvColumName.Arg1, "");

            //存在しないSEのチェック
            if (!string.IsNullOrEmpty(this.label))
            {
                if (!dataManager.SoundSetting.Contains(label, SoundType.Se))
                {
                    Debug.LogError(ToErrorString(label + " is not contained in file setting"));
                }
            }
            this.fadeTime = ParseCellOptional<float>(AdvColumName.Arg6, 0.2f);
        }

        public override void DoCommand(AdvEngine engine)
        {
            if (string.IsNullOrEmpty(label))
            {
                engine.SoundManager.StopSeAll(fadeTime);
            }
            else
            {
                engine.SoundManager.StopSe(label, fadeTime);
            }
        }

        string label;
        float fadeTime;
    }
}
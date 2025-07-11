using UnityEngine;

namespace fantec
{

    /// <summary>
    /// コマンド：バイブレーションを作動
    /// </summary>
    internal class AdvCommandVibrate : AdvCommand
    {
        public AdvCommandVibrate(StringGridRow row, AdvSettingDataManager dataManager)
            : base(row)
        {
        }

        public override void DoCommand(AdvEngine engine)
        {
#if (UNITY_IPHONE || UNITY_ANDROID) && !FANTEC_IGNORE_VIBRATE
            Handheld.Vibrate();
#endif
        }
    }
}

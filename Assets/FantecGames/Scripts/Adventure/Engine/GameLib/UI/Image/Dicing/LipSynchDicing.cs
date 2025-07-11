using UnityEngine;
using System.Collections;
using fantecExtensions;



#if UNITY_EDITOR
using UnityEditor;
#endif


namespace fantec
{

    /// <summary>
    /// 口パク処理
    /// </summary>
    [AddComponentMenu("fantec/Lib/UI/LipSynchDicing")]
    public class LipSynchDicing : LipSynch2d
    {
        DicingImage Dicing { get { return this.gameObject.GetComponentCache<DicingImage>(ref dicing); } }
        DicingImage dicing;

        protected override IEnumerator CoUpdateLipSync()
        {
            while (IsPlaying)
            {
                string pattern = Dicing.MainPattern;
                foreach (var data in AnimationData.DataList)
                {
                    Dicing.TryChangePatternWithOption(pattern, LipTag, data.ComvertNameSimple());
                    yield return TimeUtil.WaitForSeconds(UnscaledTime, data.Duration);
                    while (IsPausing) yield return null;
                }
                Dicing.TryChangePatternWithOption(pattern, LipTag, "");
                if (!IsPlaying) break;
                yield return TimeUtil.WaitForSeconds(UnscaledTime, Interval);
                while (IsPausing) yield return null;
            }
            coLypSync = null;
        }

        protected override void OnStopLipSync()
        {
            base.OnStopLipSync();
            Dicing.TryChangePatternWithOption(Dicing.MainPattern, LipTag, "");
        }
    }
}

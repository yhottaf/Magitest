using UnityEngine;
using System.Collections;
using System;
using fantecExtensions;


namespace fantec
{

    /// <summary>
    /// アバタータイプのまばたき処理の基本クラス
    /// </summary>
    [RequireComponent(typeof(DicingImage))]
    [AddComponentMenu("fantec/Lib/UI/EyeBlinkDicing")]
    public class EyeBlinkDicing : EyeBlinkBase
    {
        DicingImage Dicing { get { return this.gameObject.GetComponentCache<DicingImage>(ref dicing); } }
        DicingImage dicing;

        protected override IEnumerator CoEyeBlink(Action onComplete)
        {
            foreach (var data in AnimationData.DataList)
            {
                Dicing.TryChangePatternWithOption(Dicing.MainPattern, EyeTag, data.ComvertNameSimple());
                yield return TimeUtil.WaitForSeconds(UnscaledTime, data.Duration);
            }
            Dicing.TryChangePatternWithOption(Dicing.MainPattern, EyeTag, "");
            if (onComplete != null) onComplete();
            yield break;
        }
    }
}

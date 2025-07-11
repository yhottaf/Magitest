using UnityEngine;
using System.Collections;

namespace fantec
{
    /// 時間制御（スケール設定を反映させるか決めるスイッチ）
    [AddComponentMenu("fantec/ADV/Internal/AdvTime")]
    public class AdvTime : MonoBehaviour
    {
        public bool Unscaled { get { return unscaled; } set { unscaled = value; } }
        [SerializeField]
        bool unscaled = false;

        public float Time
        {
            get { return Unscaled ? UnityEngine.Time.unscaledTime : UnityEngine.Time.time; }
        }

        public float DeltaTime
        {
            get { return TimeUtil.GetDeltaTime(unscaled); }
        }

        public IEnumerator WaitForSeconds(float time)
        {
            yield return TimeUtil.WaitForSeconds(unscaled, time);
        }
    }
}
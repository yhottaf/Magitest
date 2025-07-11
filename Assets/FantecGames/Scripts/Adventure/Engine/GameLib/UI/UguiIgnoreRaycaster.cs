using UnityEngine;

namespace fantec
{

    /// <summary>
    /// レイキャストを無視する
    /// </summary>
    [AddComponentMenu("fantec/Lib/UI/UguiIgnoreRaycaster")]
    public class UguiIgnoreRaycaster : MonoBehaviour, ICanvasRaycastFilter
    {
        public bool ignoreRaycaster = true;

        public virtual bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
        {
            return !ignoreRaycaster;
        }
    }
}

using UnityEngine;

namespace fantec
{
    /// <summary>
    /// 速度可変可能
    /// </summary>
    public interface ISpeedable
    {
        void SetTimeScale(float timeScale);
    }
}
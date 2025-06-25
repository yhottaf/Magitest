using UnityEngine;

namespace fantec.Battle
{
    /// <summary>
    /// <see cref="Locator"/> への
    /// <see cref="Locator.Register{T}(T)"/> を
    /// 強制し、まとめて呼び出せるようにする
    /// </summary>
    public interface IRegistable:ILocatable
    {
        void Register();
    }
}
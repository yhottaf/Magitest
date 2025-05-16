using UnityEngine;

namespace fantec.Battle
{
    /// <summary>
    /// <see cref="Locator"/> ‚Ö‚Ì
    /// <see cref="Locator.Register{T}(T)"/> ‚ğ
    /// ‹­§‚µA‚Ü‚Æ‚ß‚ÄŒÄ‚Ño‚¹‚é‚æ‚¤‚É‚·‚é
    /// </summary>
    public interface IRegistable:ILocatable
    {
        void Register();
    }
}
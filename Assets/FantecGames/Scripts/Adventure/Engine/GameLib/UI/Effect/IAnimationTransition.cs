using UnityEngine;

namespace fantec
{
    public interface IAnimationRuleFade
    {
        GameObject gameObject { get; }
        void BeginRuleFade(Texture texture, float vague, bool isPremultipliedAlpha);
        void EndRuleFade();
    }
}

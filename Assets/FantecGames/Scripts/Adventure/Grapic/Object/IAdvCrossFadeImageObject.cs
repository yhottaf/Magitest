using System;

namespace fantec
{

    // クロスフェード処理用のインターフェース
    public interface IAdvCrossFadeImageObject
    {
        bool IsCrossFading { get; }
        void RestartCrossFade(float fadeTime, Action onComplete);
        void SkipCrossFade();
    }
}

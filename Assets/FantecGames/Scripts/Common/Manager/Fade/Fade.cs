using Cysharp.Threading.Tasks;
using System;

namespace fantec.Common
{
    public class Fade
    {
        private static FadeView m_View;

        public static void FadeIn(float duration = 1, Action onFinish = null)
        {
            LazyLoad();
            m_View.FadeIn(duration, onFinish);
        }

        public static async UniTask FadeInAsync(float duration = 1)
        {
            LazyLoad();
            await m_View.FadeInAsync(duration);
        }

        public static void FadeOut(float duration = 1, Action onFinish = null)
        {
            LazyLoad();
            m_View.FadeOut(duration, onFinish);
        }

        public static async UniTask FadeOutAsync(float duration = 1)
        {
            LazyLoad();
            await m_View.FadeOutAsync(duration);
        }

        private static void LazyLoad()
        {
            if (m_View == null)
            {
                m_View = OverlayCanvasManager.Instance.Create<FadeView>(AssetPath.UiFade);
            }
        }
    }
}
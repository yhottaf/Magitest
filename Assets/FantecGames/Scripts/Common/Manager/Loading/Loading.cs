using Cysharp.Threading.Tasks;
using UnityEngine;

namespace fantec.Common
{
    public class Loading
    {
        private static LoadingView m_View;

        public static void Show(float duration = 1f, float alpha = 1f, System.Action onFinish = null)
        {
            LazyLoad();
            m_View.Show(duration, alpha, onFinish);
        }

        public static void Hide(float duration = 1f, System.Action onFinish = null)
        {
            LazyLoad();
            m_View.Hide(duration, onFinish);
        }

        private static void LazyLoad()
        {
            if (m_View == null)
            {
                m_View = OverlayCanvasManager.Instance.Create<LoadingView>(AssetPath.UiLoading);
            }
        }
    }
}
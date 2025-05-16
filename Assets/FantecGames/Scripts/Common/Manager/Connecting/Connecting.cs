using Cysharp.Threading.Tasks;

namespace fantec.Common
{
    public class Connecting
    {
        private static ConnectingView m_View;

        public static void Open()
        {
            LazyLoad();
            m_View.Show();
        }

        public static void Close()
        {
            LazyLoad();
            m_View.Hide();
        }

        private static void LazyLoad()
        {
            if (m_View == null)
            {
                m_View = OverlayCanvasManager.Instance.Create<ConnectingView>(AssetPath.UiConnecting);
            }
        }
    }
}
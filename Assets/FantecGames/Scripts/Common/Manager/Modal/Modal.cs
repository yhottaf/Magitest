
namespace fantec.Common
{
    public class Modal
    {
        public static ModalView Create()
        {
            return OverlayCanvasManager.Instance.Create<ModalView>(AssetPath.UiModal);
        }

        public static void Clear()
        {
            OverlayCanvasManager.Instance.Remove<ModalView>();
        }
    }
}
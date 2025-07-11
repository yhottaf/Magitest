using fantecExtensions;
using UnityEngine;
using UnityEngine.Events;

namespace fantec
{
    [System.Serializable]
    public class CaptureCameraEvent : UnityEvent<CaptureCamera> { }

    /// <summary>
    /// Cameraの画像を一時的にキャプチャする
    /// </summary>
    [RequireComponent(typeof(Camera))]
    [AddComponentMenu("fantec/Lib/Camera/CaptureCamera")]
    public class CaptureCamera : MonoBehaviour
    {
        public RenderTexture CaptureImage { get; set; }

        public CaptureCameraEvent OnCaptured = new CaptureCameraEvent();

        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (!this.enabled) return;

            if (CaptureImage != null)
            {
                RenderTexture.ReleaseTemporary(CaptureImage);
            }

            CaptureImage = source.CreateCopyTemporary();

            Graphics.Blit(source, destination);

            OnCaptured.Invoke(this);
        }
    }
}

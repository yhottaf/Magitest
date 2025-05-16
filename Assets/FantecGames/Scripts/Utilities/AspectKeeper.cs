using UnityEngine;

namespace fantec.Utilities
{
    public class AspectKeeper : MonoBehaviour
    {
        private readonly Rect DEFAULT_VIEWPORT_RECT = new Rect(0,0,1,1);

        [SerializeField] private Vector2 m_TargetAspect = new Vector2(1920,1080);
        [SerializeField] private bool m_IsEveryUpdate = false;

        private Camera m_Camera;

        private void Awake()
        {
            m_Camera = GetComponent<Camera>();
        }

        private void Start()
        {
            Resize();          
        }

        private void Update()
        {
            if(m_IsEveryUpdate)
            {
                Resize();
            }
        }

        private void Resize()
        {
            var screenAspect=Screen.width/ (float)Screen.height;     // 画面のアスペクト比
            var targetAspect = m_TargetAspect.x / m_TargetAspect.y;  // 目的のアスペクト比
            var magRate = targetAspect / screenAspect;               // 目的アスペクト比にするための倍率
            var viewportRect = DEFAULT_VIEWPORT_RECT;                // Viewport初期値でRectを作成

            if(magRate<1)
            {
                viewportRect.width = magRate;                        // 使用する横幅を変更
                viewportRect.x = 0.5f - viewportRect.width * 0.5f;   // 中央に寄せる
            }
            else
            {
                viewportRect.height = 1 / magRate;                    // 使用する縦幅を変更
                viewportRect.y = 0.5f - viewportRect.height * 0.5f;   // 中央に寄せる
            }

            m_Camera.rect = viewportRect;                             // カメラのViewportに適用
        }
    }
}
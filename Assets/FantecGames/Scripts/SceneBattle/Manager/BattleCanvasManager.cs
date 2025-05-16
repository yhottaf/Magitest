using UnityEngine;

namespace fantec.Battle.Manager
{
    public interface IBattleCanvasManager : IRegistable
    {
        Vector2 GetWorldToScreenPointForMain(Vector2 worldPos);
        Vector2 GetWorldToScreenPointForOver(Vector2 worldPos);

        Canvas MainCanvas { get; }
        Canvas OverCanvas { get; }

        RectTransform MainCanvasRect {  get; }
        RectTransform OverCanvasRect { get; }
    }
    public class BattleCanvasManager : MonoBehaviour, IBattleCanvasManager
    {
        public Canvas MainCanvas => m_MainCanvas;
        public Canvas OverCanvas => m_OverCanvas;

        public RectTransform MainCanvasRect { get; private set; }
        public RectTransform OverCanvasRect { get; private set; }

        [SerializeField] Canvas m_MainCanvas;
        [SerializeField] Canvas m_OverCanvas;

        public void Register()
        {
            Locator.Register<IBattleCanvasManager>(this);
        }

        private void Awake()
        {
            MainCanvasRect=m_MainCanvas.GetComponent<RectTransform>();
            OverCanvasRect=m_OverCanvas.GetComponent<RectTransform>();
        }

        private Vector2 GetWorldToScreenPoint(RectTransform canvasRect,Vector2 worldPos)
        {
            var targetPos = Vector2.zero;
            var camera = Locator.Resolve<IBattleCameraManager>().MainCamera;
            var screenPos = RectTransformUtility.WorldToScreenPoint(camera,worldPos);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect,screenPos,camera,out targetPos);

            return targetPos;
        }

        /// <summary>
        /// ワールド座標をキャンバス座標に変換して返す
        /// </summary>
        /// <param name="worldPos"></param>
        public Vector2 GetWorldToScreenPointForMain(Vector2 worldPos)
        {
            return GetWorldToScreenPoint(MainCanvasRect, worldPos);
        }

        /// <summary>
        /// ワールド座標をキャンバス座標に変換して返す
        /// </summary>
        /// <param name="worldPos"></param>
        public Vector2 GetWorldToScreenPointForOver(Vector2 worldPos)
        {
            return GetWorldToScreenPoint(OverCanvasRect, worldPos);
        }
    }
}
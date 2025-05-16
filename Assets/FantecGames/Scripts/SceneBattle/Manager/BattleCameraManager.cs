using DG.Tweening;
using fantec.Battle.Utiles;
using UnityEngine;

namespace fantec.Battle.Manager
{
    public interface IBattleCameraManager:IRegistable
    {
        Camera MainCamera { get; }
        void PlayShake(float duration);

        void MoveToDefault();
        void MoveToTarget(Vector3 targetPos);
        void ZoomIn();
    }

    public class BattleCameraManager : MonoBehaviour,IBattleCameraManager
    {
        private const float DURATION = 0.5f;

        private readonly Vector3 POSTION_DEFAULT = new Vector3(0, 1, POSITION_Z_DEFAULT);
        private const float POSITION_Z_DEFAULT = -10;

        private const float RTHO_SIZE_DEFAULT = 6;
        private const float RTHO_SIZE_ZOOM_IN = 3;
        private const float RTHO_SIZE_ZOOM_OUT = 8;

        [SerializeField] private Camera m_MainCamera;
        [SerializeField] private float m_ShakeStrength = 1;

        public Camera MainCamera => m_MainCamera;

        private readonly SequenceStacker m_ZoomSequence=new SequenceStacker();
        private readonly SaveableSequence m_MoveSequence = new SaveableSequence();

        public void Register()
        {
            Locator.Register<IBattleCameraManager>(this);
        }

        private void OnDestroy()
        {
            m_ZoomSequence.Kill();
            m_MoveSequence.Kill();
        }

        public void SetPlaySpeed(float spped)
        {
            m_ZoomSequence.TimeScale = spped;
            m_MoveSequence.TimeScale = spped;
        }

        public void PlayShake(float duration)
        {
            m_MoveSequence.Kill();
            m_MoveSequence.Value = DOTween.Sequence()
                .Append(m_MainCamera.transform.DOShakePosition(duration, new Vector3(0.1f,0.1f,0f)))
               // .Append(m_MainCamera.transform.DOMove(POSTION_DEFAULT, 1.0f))
                .SetLink(this.gameObject);
        }

        /// <summary>
        /// ターゲットをカメラの中心にとらえる
        /// </summary>
        public void MoveToTarget(Vector3 position)
        {
            position.z = POSITION_Z_DEFAULT;

            m_MoveSequence.Value = DOTween.Sequence()
                .Append(m_MainCamera.transform.DOMove(position, DURATION).SetEase(Ease.OutQuad))
                .SetLink(this.gameObject);
        }

        /// <summary>
        /// カメラを元の位置に戻す
        /// </summary>
        public void MoveToDefault()
        {
            m_MoveSequence.Kill();
            m_MoveSequence.Value = DOTween.Sequence()
                .Append(m_MainCamera.transform.DOMove(POSTION_DEFAULT, DURATION));
        }

        /// <summary>
        /// ズームイン
        /// </summary>
        public void ZoomIn()
        {
            m_ZoomSequence.Value = DOTween.Sequence()
                .Append(m_MainCamera.DOOrthoSize(RTHO_SIZE_ZOOM_IN, DURATION))
                .SetLink(this.gameObject);
        }

        /// <summary>
        /// ズームアウト
        /// </summary>
        public void ZoomOut()
        {
            m_ZoomSequence.Value = DOTween.Sequence()
                .Append(m_MainCamera.DOOrthoSize(RTHO_SIZE_ZOOM_OUT, DURATION))
                .SetLink(this.gameObject);
        }

        /// <summary>
        /// ズーム状態をデフォルトに戻す
        /// </summary>
        public void ZoomDefault()
        {
            m_ZoomSequence.Value = DOTween.Sequence()
                .Append(m_MainCamera.DOOrthoSize(RTHO_SIZE_DEFAULT, DURATION))
                .SetLink(this.gameObject);
        }
    }
}
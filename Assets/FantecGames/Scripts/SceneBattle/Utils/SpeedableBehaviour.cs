using Cysharp.Threading.Tasks;
using fantec.Battle.Model;
using UnityEngine;
using UniRx;

namespace fantec.Battle.Utiles
{
    /// <summary>
    /// ゲーム速度の可変に対応した MonoBehavior
    /// </summary>
    public abstract class SpeedableBehaviour : MonoBehaviour
    {
        private bool m_IsIgnoreDirectingPause = false;
        protected bool m_IsDirectingPause;
        protected float m_TimeScale;

        protected virtual void Awake()
        {
            var modelTime = Locator.Resolve<IBattleModelTime>();

            modelTime.OnCurrentTimeScale.Subscribe(OnUpdateCurrentTimeScale).AddTo(this);
            modelTime.OnIgnoreTimeScale.Subscribe(OnUpdateIgnoreTimeScale).AddTo(this);
            modelTime.OnIsDirectingPause.Subscribe(OnUpdateDirectingPause).AddTo(this);
        }

        /// <summary>
        /// 演出による停止中でも動き続けるか否かの設定
        /// </summary>
        /// <param name="enabled">
        /// true   動ける
        /// false  動けない
        /// </param>
        protected void SetThroughDirectingPause(bool enabled)
        {
            m_IsIgnoreDirectingPause=enabled;
            OnSetCurrentTimeScale(m_TimeScale);
        }

        // 演出でのポーズ状態更新時
        private void OnUpdateDirectingPause(bool enable)
        {
            m_IsDirectingPause = enable;
            if (enable == true && m_IsIgnoreDirectingPause) return;

            OnSetCurrentTimeScale(enable ? 0 : m_TimeScale);
        }

        // ゲーム速度の更新時
        private void OnUpdateCurrentTimeScale(float speed)
        {
            m_TimeScale = speed;

            // 演出停止が無効であるか、素通りできる場合
            if(m_IsDirectingPause==false||m_IsIgnoreDirectingPause)
            {
                // 速度を反映
                OnSetCurrentTimeScale(speed);
            }
        }

        // 無視の速度
        private void OnUpdateIgnoreTimeScale(float timeScale)
        {
            OnSetIgnoreTimeScale(timeScale);
        }

        // 通常のゲーム速度が更新された際に呼ばれる
        protected virtual void OnSetCurrentTimeScale(float timeScale) { }

        // システム速度が更新された際に呼ばれる
        protected virtual void OnSetIgnoreTimeScale(float timeScale) { }
    }
}
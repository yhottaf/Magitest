using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace fantec.Utilities
{
    /// <summary>
    /// 複数重ね掛けで再生可能な AudioSource もどき
    /// </summary>
    public class MultipleSource : MonoBehaviour
    {
        // 複製元のクリップ
        private AudioClip m_AudioClip;
        // 再生用のソースリスト
        private List<AudioSource> m_SourceList = new List<AudioSource>();

        // 再生初回フレームであるか否か識別用
        private bool m_IsOneFrame;

        /// <summary>
        /// リストから使用されていないSEを鳴らす
        /// </summary>
        public void PlayLayerd()
        {
            // 同時再生防止
            if (m_IsOneFrame) return;
            else
            {
                m_IsOneFrame = true;
                Observable.NextFrame().Subscribe(_ => m_IsOneFrame = false);
            }

            // 使用されていない AudioSource を探す
            foreach (AudioSource audioSource in m_SourceList)
            {
                if (audioSource.isPlaying == false)
                {
                    audioSource.Play();
                    return;
                }
            }

            // 再生可能な AudioSource がなかった場合複製し、再生
            CloneSource().Play();
        }

        /// <summary>
        /// リストの中のSEをミュートの管理
        /// </summary>
        public void AllMute(bool value)
        {
            foreach (AudioSource audioSource in m_SourceList)
            {
                audioSource.mute = value;
            }
        }

        /// <summary>
        /// 音量を調整する
        /// </summary>
        public void ChangeVolume(float value)
        {
            foreach (AudioSource audioSource in m_SourceList)
            {
                audioSource.volume = value;
            }
        }

        /// <summary>
        /// 再生中のものを上書きして鳴らす
        /// </summary>
        public void PlayOverride()
        {
            m_SourceList[0].Play();
        }

        /// <summary>
        /// 再生中なら鳴らさない
        /// </summary>
        public void PlayCanceld()
        {
            // 使用されていなる AudioSource を探す
            foreach (AudioSource audioSource in m_SourceList)
            {
                if (audioSource.isPlaying == true)
                {
                    // あれば何もしない
                    return;
                }
            }

            // ないため鳴らす
            m_SourceList[0].Play();
        }

        /// <summary>
        /// 初期設定 MEMO:使用する際は最初に必ず呼ぶ
        /// </summary>
        /// <param name="audioClip">再生する AudioClip</param>
        /// <param name="sourceCount">AudioSource の生成個数</param>
        public void Setup(AudioClip audioClip, int sourceCount = 1)
        {
            m_AudioClip = audioClip;

            for (int i = 0; i < sourceCount; i++)
            {
                CloneSource();
            }
        }

        /// <summary>
        /// AudioSource を複製する
        /// </summary>
        private AudioSource CloneSource()
        {
            var source = this.gameObject.AddComponent<AudioSource>();
            source.clip = m_AudioClip;
            m_SourceList.Add(source);
            return source;
        }
    }
}

using UnityEngine;
using fantec.Utilities;
using fantec.Menu;
using System.Threading;
using Cysharp.Threading.Tasks;
using System;
using Unity.VisualScripting.Antlr3.Runtime;

namespace fantec.Common
{
    public class BGMManager : PersistentSingleton<BGMManager>
    {
        private AudioSource m_AudioA;
        private AudioSource m_AudioB;
        private AudioSource m_MainAudio;
        private AudioSource m_NextAudio;
        private Type playerType;
        private CancellationTokenSource m_Cts;

        public enum Type
        {
            None,
            Title,
            Home,
            Shop,
            Gacha,
            BattleResultLose,
            BattoleResultWin,
        }

        protected override void Awake()
        {
            base.Awake();
            OptionManager.SoundData sound = PlayerPrefsManager.GetOptionSoundData();
            m_AudioA = gameObject.AddComponent<AudioSource>();
            m_AudioB = gameObject.AddComponent<AudioSource>();

            m_AudioA.volume = sound.nVolumeBGM;
            m_AudioB.volume = sound.nVolumeBGM;

            m_MainAudio = m_AudioA; // 初期状態
        }

        /// <summary>
        /// 非同期でBGMを再生する（Addressables経由）
        /// </summary>
        public async UniTask PlayAsync(Type type, bool isLoop = false, float fadeTime = 1f,CancellationToken externalCts = default)
        {
            if (playerType == type) return;

            playerType = type;
 

            m_Cts?.Cancel();
            m_Cts = CancellationTokenSource.CreateLinkedTokenSource(externalCts);

            string address = AssetPath.BGMFolderPath + type.ToString() + ".wav";
            var clip = await AssetManager.Instance.LoadAssetAsync<AudioClip>(address, m_Cts.Token);

            if (clip == null)
            {
                Debug.LogWarning($"[BGMManager] BGM {type} の読み込みに失敗しました。");
                return;
            }

            // 切り替え先オーディオソースの準備
            m_NextAudio = (m_MainAudio == m_AudioA) ? m_AudioB : m_AudioA;
            m_NextAudio.clip = clip;
            m_NextAudio.loop = isLoop;
            m_NextAudio.volume = 0f;
            m_NextAudio.Play();

            // フェード処理
            float time = 0f;
            float startVolume = m_MainAudio?.volume ?? 0f;
            while (time < fadeTime)
            {
                if (m_Cts.IsCancellationRequested) return;
                float t = time / fadeTime;
                m_MainAudio.volume = Mathf.Lerp(startVolume, 0f, t);
                m_NextAudio.volume = Mathf.Lerp(0f, startVolume, t);
                time += Time.deltaTime;
                await UniTask.Yield();
            }

            // 完了処理
            m_MainAudio?.Stop();
            m_MainAudio.volume = startVolume;
            m_MainAudio = m_NextAudio;
        }

        /// <summary>
        /// AudioClip 指定でBGMを再生する
        /// </summary>
        /// <param name="clip"></param>
        /// <param name="isLoop"></param>
        public void Play(AudioClip clip,bool isLoop=false)
        {
            m_MainAudio.clip=clip;
            m_MainAudio.loop=isLoop;
            m_MainAudio.Play();
        }

        /// <summary>
        /// BGMを停止する
        /// </summary>
        public void Stop()
        {
            playerType = Type.None;
            m_MainAudio.Stop();
            m_Cts?.Cancel();
        }

        /// <summary>
        /// BGMを一時停止する
        /// </summary>
        public void Pause()
        {
            m_MainAudio.Pause();
        }

        /// <summary>
        /// BGMを再開する
        /// </summary>
        public void Resume()
        {
            m_MainAudio.Play();
        }

        /// <summary>
        /// BGMをミュートにする
        /// </summary>
        /// <param name="value"></param>
        public void Mute(bool value)
        {
            m_MainAudio.mute = value;
        }

        /// <summary>
        /// BGMの音量を調整する
        /// </summary>
        /// <param name="value"></param>
        public void VolumeChange(float value)
        {
            m_MainAudio.volume = value;
        }
    }
}
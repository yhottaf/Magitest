using Cysharp.Threading.Tasks;
using fantec.Utilities;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace fantec.Common
{
    public enum VoicePlayType
    {
        /// <summary> 別 Clipとして再生 </summary>
        LAYERED,

        /// <summary> 同じ再生中の Clipを止めて再生 </summary>
        OVERRIDE,

        /// <summary> 同じ Clipが再生中の場合は再生しない </summary>
        CANCELD,
    }

    /// <summary>
    /// Voice ファイル名と一致するように
    /// </summary>
    public enum VoiceClipName
    {

        //---------  システム系  ----------------//

        SystemButtonDownNo,   //ボタン押下時-否定
        SystemButtonDownYes,  //ボタン押下時-肯定
        SystemTapScreen,      //画面タップ
        SystemLoginPop,      //タイトル画面タップ


        //---------  環境音  -------------------//
        //攻撃音など。
    }


    public partial class VoiceManager : PersistentSingleton<VoiceManager>
    {
        private Dictionary<string, MultipleSource> m_SourceDictionary = new Dictionary<string, MultipleSource>();

        /// <summary>
        /// クリップを読み込む
        /// </summary>
        /// <param name="clip"></param>
        /// <param name="clipCount">複製したいクリップ数</param>
        public void LoadClipData(AudioClip clip, int clipCount = 1)
        {
            MultipleSource source = new GameObject(clip.name).AddComponent<MultipleSource>();
            source.Setup(clip, clipCount);
            source.transform.SetParent(transform);
            m_SourceDictionary.Add(clip.name, source);
        }

        public void LoadClipData(string clipName, int clipCount = 1)
        {
            LoadClipData(AssetManager.Instance.LoadAsset<AudioClip>(AssetPath.VoiceFolderPath + clipName.ToString()));
        }

        /// <summary>
        /// 非同期でクリップデータを読み込む
        /// </summary>
        /// <param name="clipNameType"></param>
        /// <param name="clipCount"></param>
        /// <param name="cts"></param>
        /// <returns></returns>
        public async UniTask LoadClipDataAsync(SEClipName clipNameType, int clipCount, CancellationToken cts)
        {
            var clipName = clipNameType.ToString();
            var clip = await AssetManager.Instance.LoadAssetAsync<AudioClip>(AssetPath.VoiceFolderPath + clipName, cts);
            LoadClipData(clip);
        }

        /// <summary>
        /// 非同期でまとめてクリップデータを読み込む
        /// </summary>
        public async UniTask LoadClipDataAsync(SEClipName[] clipNames, int clipCount, CancellationToken cts)
        {
            foreach (SEClipName clipName in clipNames)
            {
                await LoadClipDataAsync(clipNames, clipCount, cts);
            }
        }

        /// <summary>
        /// クリップを取り除く
        /// </summary>
        /// <param name="clipNameType"></param>
        public void RemoveClipData(VoiceClipName clipNameType)
        {
            var clipName = clipNameType.ToString();
            if (m_SourceDictionary.ContainsKey(clipName))
            {
                Destroy(m_SourceDictionary[clipName].gameObject);
                m_SourceDictionary.Remove(clipName);
            }
        }

        /// <summary>
        /// まとめてクリップを取り除く
        /// </summary>
        /// <param name="clipNames"></param>
        public void RemoveClipData(VoiceClipName[] clipNames)
        {
            foreach (VoiceClipName clipName in clipNames)
            {
                RemoveClipData(clipName);
            }
        }

        public void Play(VoiceClipName clipNameType, VoicePlayType playType = VoicePlayType.LAYERED) => Play(clipNameType.ToString(), playType);
        public void Play(AudioClip clip, VoicePlayType playType = VoicePlayType.LAYERED)
        {
            if (m_SourceDictionary.ContainsKey(clip.name))
            {
                Play(clip.name, playType);
            }
            else
            {
                LoadClipData(clip);
                Play(clip);
            }
        }

        public void Play(string clipName, VoicePlayType playType = VoicePlayType.LAYERED)
        {
            void Play(VoicePlayType playType)
            {
                switch (playType)
                {
                    case VoicePlayType.LAYERED:
                        m_SourceDictionary[clipName].PlayLayerd();
                        break;
                    case VoicePlayType.OVERRIDE:
                        m_SourceDictionary[clipName].PlayOverride();
                        break;
                    case VoicePlayType.CANCELD:
                        m_SourceDictionary[clipName].PlayCanceld();
                        break;
                }
            }

            if (m_SourceDictionary.ContainsKey(clipName))
            {
                Play(playType);
            }
            else
            {
                LoadClipData(clipName);
                Play(playType);
            }
        }

        /// <summary>
        /// 複数のクリップを指定しその中からランダムに１つ再生
        /// </summary>
        /// <param name="clipNames"></param>
        /// <param name="playType"></param>
        public void PlayRandom(VoiceClipName[] clipNames, VoicePlayType playType = VoicePlayType.LAYERED)
        {
            Play(clipNames[Random.Range(0, clipNames.Length)], playType);
        }
        public bool GetIsLoaded(AudioClip clip)
        {
            return m_SourceDictionary.ContainsKey(clip.name);
        }

        /// <summary>
        /// ボリュームの設定
        /// </summary>
        /// <param name="value"></param>
        public void VolumeChange(float value)
        {
            foreach (var data in m_SourceDictionary)
            {
                m_SourceDictionary[data.Key].ChangeVolume(value);
            }
        }

        /// <summary>
        /// Voiceをミュートにするかどうか
        /// </summary>
        /// <param name="value"></param>
        public void VoiceMute(bool value)
        {
            foreach (var data in m_SourceDictionary)
            {
                m_SourceDictionary[data.Key].AllMute(value);
            }
        }
    }
}
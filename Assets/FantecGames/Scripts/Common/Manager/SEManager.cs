using Cysharp.Threading.Tasks;
using fantec.Utilities;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace fantec.Common
{
    public enum SEPlayType
    {
        /// <summary> 別 Clipとして再生 </summary>
        LAYERED,

        /// <summary> 同じ再生中の Clipを止めて再生 </summary>
        OVERRIDE,

        /// <summary> 同じ Clipが再生中の場合は再生しない </summary>
        CANCELD,
    }

    /// <summary>
    /// SE ファイル名と一致するように
    /// </summary>
    public enum SEClipName
    {

        //---------  システム系  ----------------//

        SystemButtonDownNo,   //ボタン押下時-否定
        SystemButtonDownYes,  //ボタン押下時-肯定
        SystemTapScreen,      //画面タップ
        SystemLoginPop,      //タイトル画面タップ


        //---------  環境音  -------------------//
        //攻撃音など。
    }


    public partial class SEManager:PersistentSingleton<SEManager>
    {
        private Dictionary<string, MultipleSource> m_SourceDictionary = new();
        private Dictionary<string, AudioClip> m_LoadedClips = new();

        public async UniTask LoadClipDataAsync(SEClipName clipNameType, int clipCount, CancellationToken cts)
        {
            var clipName = clipNameType.ToString();
            if (m_LoadedClips.ContainsKey(clipName)) return;

            var clip = await AssetManager.Instance.LoadAssetAsync<AudioClip>(AssetPath.SEFolderPath + clipName,cts);

            if (clip != null)
            {
                m_LoadedClips[clipName] = clip;
                LoadClipData(clip, clipCount);
            }
            else
            {
                Debug.LogError($"Failed to load SE clip: {clipName}");
            }
        }

        public async UniTask LoadClipDataAsync(SEClipName[] clipNames, int clipCount, CancellationToken cts)
        {
            foreach (var clipName in clipNames)
            {
                await LoadClipDataAsync(clipName, clipCount, cts);
            }
        }

        public void LoadClipData(AudioClip clip, int clipCount = 1)
        {
            if (m_SourceDictionary.ContainsKey(clip.name)) return;

            var source = new GameObject(clip.name).AddComponent<MultipleSource>();
            source.Setup(clip, clipCount);
            source.transform.SetParent(transform);
            m_SourceDictionary.Add(clip.name, source);
        }

        public void RemoveClipData(SEClipName clipNameType)
        {
            var clipName = clipNameType.ToString();
            if (m_SourceDictionary.ContainsKey(clipName))
            {
                Destroy(m_SourceDictionary[clipName].gameObject);
                m_SourceDictionary.Remove(clipName);
            }

            if (m_LoadedClips.ContainsKey(clipName))
            {
                Addressables.Release(m_LoadedClips[clipName]);
                m_LoadedClips.Remove(clipName);
            }
        }

        public void RemoveClipData(SEClipName[] clipNames)
        {
            foreach (var clipName in clipNames)
            {
                RemoveClipData(clipName);
            }
        }

        public void Play(SEClipName clipNameType, SEPlayType playType = SEPlayType.LAYERED) => Play(clipNameType.ToString()+".wav", playType);

        public void Play(string clipName, SEPlayType playType = SEPlayType.LAYERED)
        {
            if (!m_SourceDictionary.ContainsKey(clipName)) return;

            switch (playType)
            {
                case SEPlayType.LAYERED:
                    m_SourceDictionary[clipName].PlayLayerd();
                    break;
                case SEPlayType.OVERRIDE:
                    m_SourceDictionary[clipName].PlayOverride();
                    break;
                case SEPlayType.CANCELD:
                    m_SourceDictionary[clipName].PlayCanceld();
                    break;
            }
        }

        public void Play(AudioClip clip, SEPlayType playType = SEPlayType.LAYERED)
        {
            if (m_SourceDictionary.ContainsKey(clip.name))
            {
                Play(clip.name, playType);
            }
            else
            {
                LoadClipData(clip);
                Play(clip.name, playType);
            }
        }

        public void PlayRandom(SEClipName[] clipNames, SEPlayType playType = SEPlayType.LAYERED)
        {
            var randomClip = clipNames[Random.Range(0, clipNames.Length)].ToString();
            Play(randomClip, playType);
        }

        public bool GetIsLoaded(AudioClip clip)
        {
            return m_SourceDictionary.ContainsKey(clip.name);
        }

        public void VolumeChange(float value)
        {
            foreach (var data in m_SourceDictionary)
            {
                data.Value.ChangeVolume(value);
            }
        }

        public void SEMute(bool value)
        {
            foreach (var data in m_SourceDictionary)
            {
                data.Value.AllMute(value);
            }
        }
    }
}
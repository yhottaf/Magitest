using Cysharp.Threading.Tasks;
using fantec.Battle;
using fantec.Battle.Model;
using System.Linq;
using fantec.Common;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Spine.Unity;

namespace fantec.Battle.Manager
{
    public interface IBattleResourceManager : IRegistable
    {
        UniTask<T> LoadAsync<T>(string path, CancellationToken cts) where T : UnityEngine.Object;
        UniTask LoadVoiceAsync(string voiceName,CancellationToken cts);

        // TODO: 他の読み込まなきゃいけない画像を記載
        Sprite GetDropItemSprite(int id);

        // カットインアイコンなど
        Sprite GetCutinSprite(IBattler battler);
        Sprite GetCutinSprite(int originId);


        AudioClip GetBgmResultWin();
        AudioClip GetBgmResultLose();
        AudioClip GetVoice(string voiceName);

        SkeletonDataAsset GetCharaSpine(IBattler battler);
        SkeletonDataAsset GetCharaSpine(int originId);

        UniTask CasheSpriteAsync(string path,CancellationToken cts);
        UniTask CasheAudioAsync(string path,CancellationToken cts);
        UniTask CasheSkeletonAsync(string path,CancellationToken cts);

        Sprite GetSprite(string path);
        Sprite GetBg(string fileName);
        Sprite GetFieldImg(string fileName);
        AudioClip GetAudio(string path);
        AudioClip GetBgm(string fileName);
        SkeletonDataAsset GetSkeleton(string path);

        // データベース
        //IEnumerable<Sprite> GetStateIconSprite(IEnumerable<TokenCell> tokencells);
        //Sprite GetStateIconSprite(TokenCell tokenCell);
        //Sprite GetStateIconSprite(AffectCategoryType categoryType, AffectAttributeType attributeType);

        AudioClip GetEnvironSoundClip(AffectCategoryType categoryType);
        AudioClip GetEnvironSoundClip(AffectAttributeType attributeType);
        AudioClip GetEnvironSoundClip(AffectHitType hitType);
    }

    public partial class BattleResourceManager:MonoBehaviour,IBattleResourceManager
    {
        [Header("Sound")]
        [SerializeField] private AudioClip m_BgmResultWin;
        [SerializeField] private AudioClip m_BgmResultLose;

        [Header("Database")]
        [SerializeField] private EnvironSoundDatabase m_EnvironSoundDatabase;

        private Dictionary<string,AudioClip>m_VoiceDic=new Dictionary<string,AudioClip>();
        private Dictionary<string,Sprite>m_SpriteCacheDic=new Dictionary<string,Sprite>();
        private Dictionary<string,AudioClip>m_AudioCasheDic=new Dictionary<string,AudioClip>();
        private Dictionary<string, SkeletonDataAsset> m_SkeletonCasheDic = new Dictionary<string, SkeletonDataAsset>();

        public void Register()
        {
            Locator.Register<IBattleResourceManager>(this);

            // TODO:データベースなどを記述する
            m_EnvironSoundDatabase.Regist();
        }

        #region Cashe
        public async UniTask CasheSpriteAsync(string path,CancellationToken cts)
        {
            if (m_SpriteCacheDic.ContainsKey(path) == false)
            {
                var result = await LoadAsync<Sprite>(path,cts);
                m_SpriteCacheDic.Add(path, result);
            }
        }

        public async UniTask CasheAudioAsync(string path,CancellationToken cts)
        {
            if(m_AudioCasheDic.ContainsKey(path) == false)
            {
                var result =await LoadAsync<AudioClip>(path,cts);
                m_AudioCasheDic.Add(path,result);
            }
        }

        public async UniTask CasheSkeletonAsync(string path, CancellationToken cts)
        {
            if (m_SkeletonCasheDic.ContainsKey(path) == false)
            {
                var result = await LoadAsync<SkeletonDataAsset>(path, cts);
                m_SkeletonCasheDic.Add(path, result);
            }
        }

        #endregion

        #region Load

        public async UniTask<T> LoadAsync<T>(string path, CancellationToken cts) where T : UnityEngine.Object
        {
            T asset = await AssetManager.Instance.LoadAssetAsync<T>(path, cts);
            return asset;
        }

        public async UniTask LoadVoiceAsync(string voiceName, CancellationToken cts)
        {
            if (!string.IsNullOrEmpty(voiceName) && m_VoiceDic.ContainsKey(voiceName) == false)
            {
                var clip = await LoadAsync<AudioClip>(AssetPath.VoiceFolderPath + voiceName, cts);
                m_VoiceDic.Add(voiceName, clip);
            }
        }


        #endregion

        #region Get
        public Sprite GetSprite(string path)
        {
            try { return m_SpriteCacheDic[path]; }
            catch { throw new KeyNotFoundException($"リソースがキャッシュされていません。[path: {path}]"); }
        }

        public Sprite GetBg(string fileName)
        {
            return GetSprite($"{AssetPath.SpriteBackGroundPath}{fileName}");
        }

        public Sprite GetFieldImg(string fileName)
        {
            return GetSprite($"{AssetPath.SpriteFieldPath}{fileName}");
        }

        public AudioClip GetAudio(string path)
        {
            try { return m_AudioCasheDic[path]; }
            catch { throw new KeyNotFoundException($"リソースがキャッシュされていません。[path : {path}]"); }
        }

        public AudioClip GetBgm(string fileName)
        {
            return GetAudio($"{AssetPath.BGMFolderPath}{fileName}");
        }

        public SkeletonDataAsset GetSkeleton(string path)
        {
            try { return m_SkeletonCasheDic[path]; }
            catch { throw new KeyNotFoundException($"リソースがキャッシュされていません。[path : {path}]"); }
        }

        #endregion

        #region Sprite

        //------------------------------------------------------------------//
        // Sprite
        //------------------------------------------------------------------//

        public Sprite GetCutinSprite(IBattler battler) => GetCutinSprite(battler.Unit.Entity.originId);
        public Sprite GetCutinSprite(int originId)
        {
            string path = AssetPath.GetCharacterSpriteCutinPath(originId);
            return GetSprite(path); // キャッシュ済前提。なければ例外
        }

        public Sprite GetDropItemSprite(int id)
        {
            string path = AssetPath.GetSpriteItemIcon(id);
            return GetSprite(path); // 同上
        }

        #endregion

        #region Sound
        public AudioClip GetBgmResultWin() => m_BgmResultWin;
        public AudioClip GetBgmResultLose() => m_BgmResultLose;
        public AudioClip GetVoice(string voiceName)
        {
            try { return m_VoiceDic[voiceName]; }
            catch { throw new KeyNotFoundException($"{voiceName}"); }
        }
        public AudioClip GetEnvironSoundClip(AffectCategoryType categoryType) => m_EnvironSoundDatabase.GetClip(categoryType);
        public AudioClip GetEnvironSoundClip(AffectAttributeType attributeType) => m_EnvironSoundDatabase.GetClip(attributeType);
        public AudioClip GetEnvironSoundClip(AffectHitType hitType) => m_EnvironSoundDatabase.GetClip(hitType);
        #endregion

        #region SkeletonDataAsset

        //---------------------------------------------------------------------------//
        // SkeletonDataAsset
        //---------------------------------------------------------------------------//

        public SkeletonDataAsset GetCharaSpine(IBattler battler) => GetCharaSpine(battler.Unit.Entity.originId);
        public SkeletonDataAsset GetCharaSpine(int originId)
        {
            string path = AssetPath.GetCharacterSpinePath(originId);
            return GetSkeleton(path); // キャッシュ済前提。なければ例外
        }
        #endregion
    }
}
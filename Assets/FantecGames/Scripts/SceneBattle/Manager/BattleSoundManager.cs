using Cysharp.Threading.Tasks;
using fantec.Common;
using System.Threading;
using UnityEngine;

namespace fantec.Battle.Manager
{
    public interface IBattleSoundManager:IRegistable
    {
        UniTask UnloadSeAsync(CancellationToken cts);
        void PlayBgm(string fileName);
        void PlayBgmResultWin();
        void PlayBgmResultLose();
        void StopBgm();
        void PlaySe(SEClipName clipNameType);
        void PlaySe(AudioClip clip);
        void PlaySe(AffectAttributeType attributeType);
        void PlaySe(AffectCategoryType categoryType);
        void PlaySe(AffectHitType hitType);
        void PlayCollide();
        void PlayFuss();
        void PlayVoice(string clipName);
    }

    public class BattleSoundManager : MonoBehaviour,IBattleSoundManager
    {
        public void Register()
        {
            Locator.Register<IBattleSoundManager>(this);
        }

        #region BGM
        public void PlayBgm(string fileName)
        {
            BGMManager.Instance.Play(Locator.Resolve<IBattleResourceManager>().GetBgm(fileName),true);
        }

        public void PlayBgm(string fileName,bool enabled)
        {
            BGMManager.Instance.Play(Locator.Resolve<IBattleResourceManager>().GetBgm(fileName),enabled);
        }

        public void PlayBgmResultWin()
        {
            BGMManager.Instance.Play(Locator.Resolve<IBattleResourceManager>().GetBgmResultWin(),true);
        }

        public void PlayBgmResultLose()
        {
            BGMManager.Instance.Play(Locator.Resolve<IBattleResourceManager>().GetBgmResultLose(), false);
        }

        public void StopBgm()
        {
            BGMManager.Instance.Stop();
        }
        #endregion

        #region SE

        public async UniTask UnloadSeAsync(CancellationToken cts)
        {
            await Resources.UnloadUnusedAssets().WithCancellation(cts);
        }

        public void PlaySe(SEClipName clipNameType)
        {
            SEManager.Instance.Play(clipNameType);
        }

        public void PlaySe(AudioClip clip)
        {
            SEManager.Instance.Play(clip,SEPlayType.OVERRIDE);
        }

        public void PlaySe(AffectAttributeType attributeType)
        {
            SEManager.Instance.Play(Locator.Resolve<IBattleResourceManager>().GetEnvironSoundClip(attributeType));
        }

        public void PlaySe(AffectCategoryType categoryType)
        {
            SEManager.Instance.Play(Locator.Resolve<IBattleResourceManager>().GetEnvironSoundClip(categoryType));
        }

        public void PlaySe(AffectHitType hitType)
        {
            if (hitType != AffectHitType.NONE)
                SEManager.Instance.Play(Locator.Resolve<IBattleResourceManager>().GetEnvironSoundClip(hitType));
        }

        public void PlayCollide()
        {
        //    SEManager.Instance.Play(Locator.Resolve<IBattleResourceManager>().UniqueSoundDatabase.Collide);
        }

        public void PlayFuss()
        {

        }
        #endregion

        #region Voice

        public void PlayVoice(string clipName)
        {
            
        }
        #endregion
    }
}
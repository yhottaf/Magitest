using DG.Tweening;
using fantec.Battle.Utiles;
using fantec.Common;
using Spine.Unity;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace fantec.Battle.Field.Chara
{
    public class SpineView : MonoBehaviour, ISpeedable
    {
        public enum ANIM_NAME_TYPE
        {
            Attack, Wait, Damage, Spells, Run
        }

        private const string ANIM_NAME_ATTACK = "attack";// 近距離攻撃のアニメーション
        private const string ANIM_NAME_IDLE = "wait";    // 待機アニメーション
        private const string ANIM_NAME_DAMAGE = "damage";// ダメージアニメーション
        private const string ANIM_NAME_SPELL = "spell"; // 遠距離攻撃のアニメーション
      //  private const string ANIM_NAME_RUN = "wait";     // 移動時のアニメーション

        private const int ANIM_TRACK_INDEX = 0;

        [SerializeField] SkeletonAnimation m_SkeletonAnimation;

        private SaveableSequence m_Sequence = new SaveableSequence();

        private static Dictionary<int, string> m_AnimationNameDic = new Dictionary<int, string>()
        {
            { (int) ANIM_NAME_TYPE.Attack, ANIM_NAME_ATTACK},
            {(int)ANIM_NAME_TYPE.Wait, ANIM_NAME_IDLE},
            {(int)ANIM_NAME_TYPE.Damage, ANIM_NAME_DAMAGE},
            {(int)ANIM_NAME_TYPE.Spells,ANIM_NAME_SPELL},
        //    {(int)ANIM_NAME_TYPE.Run, ANIM_NAME_RUN},
        };

        public void Show()
        {
            m_SkeletonAnimation.gameObject.SetActive(true);
        }

        public void Hide()
        {
            m_SkeletonAnimation.gameObject.SetActive(false);  
        }

        public void SetTimeScale(float timeScale)
        {
            m_Sequence.TimeScale = timeScale;
            m_SkeletonAnimation.timeScale = timeScale;
        }

        public void SetSkeletonDataAsset(SkeletonDataAsset dataAsset)
        {
            m_SkeletonAnimation.skeletonDataAsset=dataAsset;
        }

        public void SetScale(float scale)
        {
            m_SkeletonAnimation.transform.localScale = Vector3.one * scale;
        }

        public void SetFlipX(bool isFlip)
        {
            m_SkeletonAnimation.initialFlipX = isFlip;
        }

        public void SetupFix(bool overwrite=true)
        {
            m_SkeletonAnimation.Initialize(overwrite);
        }

        public void SetColor(Color color)
        {
            m_SkeletonAnimation.skeleton.SetColor(color);
        }

        public void PlayIdle() { if(GetIsAnimationExists(ANIM_NAME_TYPE.Wait))PlayLoopAnimation(ANIM_NAME_TYPE.Wait); } // TODO:スパインデータのアニメーション名をIdleで書き出し
        public void PlayAttack()=>PlayOneTimeAnimation(ANIM_NAME_TYPE.Attack);
        public void PlayDamage()=>PlayOneTimeAnimation(ANIM_NAME_TYPE.Damage);
        public void PlaySpell() => PlayOneTimeAnimation(ANIM_NAME_TYPE.Spells);
        public void PlayRun() => PlayOneTimeAnimation(ANIM_NAME_TYPE.Run);
        
        public void PlayAction(AffectInfo info,Action onEnpak=null)
        {
            m_Sequence.Complete();
            m_Sequence.Value = DOTween.Sequence()
                .AppendInterval(0.4f)
                .OnComplete(() => onEnpak?.Invoke());

            if (info.Command.categoryType.GetIsAttack())
                PlayAttack();
            else
                PlaySpell();
        }

        public void PlayFade(float toValue,float duration,float delay=0,Ease ease=Ease.Unset)
        {
            var fromValue = m_SkeletonAnimation.skeleton.A;
            m_Sequence.Value=DOTween.Sequence()
                .AppendInterval(delay)
                .Append(DOVirtual.Float(fromValue,toValue,duration,value=>
                {
                    m_SkeletonAnimation.skeleton.A = value;
                }))
                .SetEase(ease);
        }

        public void PlayFadeIn(float duration = 1.0f, float delay = 0.0f, Ease ease = Ease.Unset) => PlayFade(1.0f, duration, delay, ease);
        public void PlayFadeOut(float duration = 1.0F, float delay = 0.0f, Ease ease = Ease.Unset) => PlayFade(0.0f, duration, delay, ease);

        #region private method

        private void PlayOneTimeAnimation(ANIM_NAME_TYPE nameType)
        {
            m_SkeletonAnimation.AnimationState.ClearTrack(ANIM_TRACK_INDEX);
            m_SkeletonAnimation.AnimationState.SetAnimation(ANIM_TRACK_INDEX, GetAnimationName(nameType), false).Complete += _ => PlayIdle();
            m_SkeletonAnimation.Skeleton.SetToSetupPose();
        }

        private void PlayLoopAnimation(ANIM_NAME_TYPE nameType)
        {
            m_SkeletonAnimation.AnimationState.SetAnimation(ANIM_TRACK_INDEX, GetAnimationName(nameType), true);
        }

        private string GetAnimationName(ANIM_NAME_TYPE nameType)
        {
            try { return m_AnimationNameDic[(int)nameType]; }
            catch { throw new KeyNotFoundException($"[nameType : {nameType}] [index : {(int)nameType}]");}
        }

        private bool GetIsAnimationExists(ANIM_NAME_TYPE nameType)
        {
            return m_SkeletonAnimation.Skeleton.Data.Animations.Exists(x => x.Name == m_AnimationNameDic[(int)nameType]);
        }

        private void AnimationExistsCheck(ANIM_NAME_TYPE nameType)
        {
            if(GetIsAnimationExists(nameType)==false) {
                Debug.LogWarning($"[skinNname : {m_SkeletonAnimation.name}] [animationName : {GetAnimationName(nameType)}] が存在しません。");
            }
        }

        public void LoadShader()
        {
            Shader loadedShader = Shader.Find("Spine/Skeleton");
            // SkeletonDataAsset → AtlasAssets → Materialを辿る
            var atlasAssets = m_SkeletonAnimation.skeletonDataAsset.atlasAssets;
            foreach (var atlasAsset in atlasAssets)
            {
                var materials = atlasAsset.Materials;
                foreach (var mat in materials)
                {
                    mat.shader = loadedShader;
                    Debug.Log($"Shaderを適用しました！Material: {mat.name}");
                }
            }
            Debug.Log("Shaderロード完了！マテリアルに適用しました！");
        }
        #endregion
    }
}
using DG.Tweening;
using fantec.Battle.Utiles;
using Spine.Unity;
using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace fantec.Battle.Field.Chara
{
    public class MovementView : MonoBehaviour,IInitializable,IReloadable,ISpeedable
    {
        [Header("軸")]
        [SerializeField] private Transform m_PrimeRig;
        [SerializeField] private Transform m_MotionRig;
        [SerializeField] private SkeletonAnimation skeletonAnimation;

        private SaveableSequence m_PrimeSequence=new SaveableSequence();
        private SaveableSequence m_MotionSequence=new SaveableSequence();

        public void Initialize()
        {
            Reload();
        }

        public void Reload()
        {
            m_PrimeRig.localPosition=Vector3.zero;
            m_MotionRig.localPosition=Vector3.zero;
            m_MotionRig.localScale=Vector3.one;
            m_MotionRig.localRotation = Quaternion.Euler(Vector3.zero);
        }

        public void SetTimeScale(float timeScale)
        {
            m_MotionSequence.TimeScale = timeScale;
            m_PrimeSequence.TimeScale = timeScale;
        }

        public void SetPosition(Vector3 position)
        {
            m_PrimeRig.transform.position= position;
        }

        public void PlayMoveTo(Vector3 position,float duration=0.4f,Action onCompleted=null)
        {
            m_PrimeSequence.Kill();
            m_PrimeSequence.Value = DOTween.Sequence()
                .Append(m_PrimeRig.DOMove(position, duration).SetEase(Ease.OutSine))
                .OnComplete(() => onCompleted?.Invoke())
                .SetLink(this.gameObject);
        }

        public void PlayKnockBack(Vector2 toPosition,Action onCompleted=null)
        {
            m_MotionSequence.Kill();
            m_MotionSequence.Value = DOTween.Sequence()
                .Append(m_MotionRig.DOMove(toPosition, 0.2f))
                .Append(m_MotionRig.DOLocalMove(Vector3.zero, 0.2f))
                .OnComplete(() => onCompleted?.Invoke())
                .SetLink(this.gameObject);
        }

        public void PlayBlowBack(Vector2 toPosition,Action onCompleted=null)
        {
            m_MotionSequence.Kill();
            m_MotionSequence.Value = DOTween.Sequence()
                .Append(m_MotionRig.DOMove(toPosition, 0.6f))
                .Join(m_MotionRig.DOLocalRotate(new Vector3(0, 0, 360), 0.6f, DG.Tweening.RotateMode.FastBeyond360))
                .SetLink(this.gameObject);
        }

        public void PlayBlowScreen(Vector3 collidePosition,Vector3 offScreenPosition,Action onCollided=null,Action onCompleted=null)
        {
            // 吹き飛び
            m_MotionSequence.Kill();
            m_MotionSequence.Value = DOTween.Sequence()
                .Append(m_MotionRig.DOMove(collidePosition, 0.4f).SetEase(Ease.Linear))
                .Join(m_MotionRig.DOLocalRotate(new Vector3(0, 0, UnityEngine.Random.Range(360, 720)), 0.4f, DG.Tweening.RotateMode.FastBeyond360).SetEase(Ease.Linear))
                .Join(m_MotionRig.DOScale(Vector3.one * 1.3f, 0.4f).SetEase(Ease.Linear));

            // 滑り落ちる
            m_MotionSequence.Value
                .Append(m_MotionRig.DOMove(offScreenPosition, 0.5f).SetEase(Ease.InCubic))
                .OnComplete(() => onCompleted?.Invoke())
                .SetLink(this.gameObject);
        }

        public void PlayAdmission(Vector3 position,Action onCompleted=null)
        {
            PlayMoveTo(position, UnityEngine.Random.Range(0.5f, 1.0f), onCompleted);
        }

        public void PlayPop(float scale,float duration)
        {
            m_MotionSequence.Kill();
            m_MotionSequence.Value = DOTween.Sequence()
                .Append(m_MotionRig.DOScale(Vector3.one * scale, duration))
                .Append(m_MotionRig.DOScale(Vector3.one, duration))
                .SetLink(this.gameObject);
        }

        public void StartBlurTrail(float interval = 0.6f, float duration = 0.2f, float startAlpha = 0.4f, float endScale = 2.0f,bool isPlayer=true)
        {
            m_MotionSequence.Kill();
            m_MotionSequence.Value = DOTween.Sequence()
                .AppendCallback(() => PlayBlurLoop(duration, startAlpha, endScale,isPlayer))
                .AppendInterval(interval)
                .SetLoops(-1, LoopType.Restart)
                .SetLink(this.gameObject);
        }

        public void PlayBlurLoop(float duration = 0.5f, float startAlpha = 0.4f, float endScale = 2.0f,bool isPlayer=true)
        {
            if (skeletonAnimation.skeletonDataAsset == null||skeletonAnimation==null) return;
            // Ghostオブジェクト生成
            GameObject ghost = new GameObject("SpineGhost");
            ghost.transform.SetParent(skeletonAnimation.gameObject.transform);
            ghost.transform.localPosition = Vector3.zero;
            ghost.transform.localRotation = Quaternion.identity;
            ghost.transform.localScale = Vector3.one;
            if(!isPlayer)
            {
                var scale = ghost.transform.localScale;
                scale.x = -1f; // x方向反転
                ghost.transform.localScale = scale;
            }

            var ghostRenderer = ghost.AddComponent<SkeletonAnimation>();
            ghostRenderer.skeletonDataAsset = skeletonAnimation.skeletonDataAsset;
            ghostRenderer.Initialize(true);




            // スキンとポーズを同期
            ghostRenderer.skeleton.SetSkin(skeletonAnimation.skeleton.Skin);
            ghostRenderer.skeleton.SetSlotsToSetupPose();
            ghostRenderer.skeleton.SetBonesToSetupPose();

            var currentTrack = skeletonAnimation.AnimationState.GetCurrent(0);
            if (currentTrack != null)
            {
                var anim = currentTrack.Animation;
                ghostRenderer.AnimationState.SetAnimation(0, anim, currentTrack.Loop);
                ghostRenderer.AnimationState.Apply(ghostRenderer.skeleton);
                ghostRenderer.timeScale = 0f; // 再生させない（ポーズ状態）
            }


            foreach (var slot in ghostRenderer.skeleton.Slots)
            {
                var originalSlot = skeletonAnimation.skeleton.FindSlot(slot.Data.Name);
                if (originalSlot != null)
                {
                    slot.SetColor(new Color(originalSlot.GetColor().r, originalSlot.GetColor().g, originalSlot.GetColor().b, startAlpha));
                }
            }

            float alpha = startAlpha;

            // Sequenceで統合（スケール + 透明度 + 削除）
            var seq = DOTween.Sequence()
                .Join(ghost.transform.DOScale(ghost.transform.localScale * endScale, duration))
                .Join(DOTween.To(() => alpha,
                                 a => {
                                     alpha = a;
                                     foreach (var slot in ghostRenderer.skeleton.Slots)
                                     {
                                         var c = slot.GetColor();
                                         c.a = a;
                                         slot.SetColor(c);
                                     }
                                 },
                                 0f,
                                 duration))
                .AppendCallback(() => Destroy(ghost));
        }

        // ブラーをストップさせる
        public void StopBlurTrail()
        {
            if (m_MotionSequence.Value != null && m_MotionSequence.Value.IsActive())
            {
                m_MotionSequence.Value.Kill();
            }
        }

        public void StartFadeBody(float duration = 0.5f, Action onCompleted = null)
        {
            if (skeletonAnimation.skeletonDataAsset == null || skeletonAnimation == null) return;

            float alpha = 1.0f;
            skeletonAnimation.timeScale = 0f; // 再生させない（ポーズ状態）
            m_MotionSequence.Kill();
            m_MotionSequence.Value = DOTween.Sequence()
                .Join(DOTween.To(() => alpha,
                                 a =>
                                 {
                                     alpha = a;
                                     foreach (var slot in skeletonAnimation.skeleton.Slots)
                                     {
                                         var c = slot.GetColor();
                                         c.a = a;
                                         slot.SetColor(c);
                                     }
                                 },
                                 0f,
                                 duration))
                .OnComplete(() => onCompleted?.Invoke())
                .SetLink(this.gameObject);
        }

        public void ResetBody()
        {
            if (skeletonAnimation.skeletonDataAsset == null || skeletonAnimation == null) return;
            float alpha = 1.0f;
            foreach (var slot in skeletonAnimation.skeleton.Slots)
            {
                var c = slot.GetColor();
                c.a = alpha;
                slot.SetColor(c);
            }
        }
    }
}
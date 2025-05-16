using fantec.Battle.Animations;
using fantec.Battle.Utiles;
using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace fantec.Battle
{
    public interface IOverrideSkillCutinAnimation:IFullScreenAnimation<IOverrideSkillCutinAnimation>
    {
        IOverrideSkillCutinAnimation SetCharaSprite(Sprite sprite);
        IOverrideSkillCutinAnimation SetFrameSprite(Sprite sprite);
        IOverrideSkillCutinAnimation SetBackSprite(AffectOverrideType type);
        IOverrideSkillCutinAnimation SetSkillName(string skillName);
    }
}

namespace fantec.Battle.Animations
{
    public class OverrideSkillCutinAnimation : AnimationBase,IOverrideSkillCutinAnimation
    {
        [SerializeField] private AnimatorHashStateObservable m_HashStateObservable;

        public IObservable<Unit> OnEnd => m_HashStateObservable.Where(enabled => enabled).Select(_=>Unit.Default);

        public CompositeDisposable ClosedDisposables { get; private set; }=new CompositeDisposable();

        [SerializeField] private SpriteRenderer m_CharaSpriteRenderer;
        [SerializeField] private SpriteRenderer m_BackSpriteRenderer;
        [SerializeField] private SpriteRenderer m_FrameSpriteRenderer;
        [SerializeField] private SpriteRenderer m_SplashSpriteRenderer;
        [SerializeField] private Text m_SkillNameText;
        [SerializeField] private Sprite[] m_Sprites;
        public IOverrideSkillCutinAnimation OnCreate()
        {
            OnEnd.DelayFrame(1).Subscribe(_=>Close()).AddTo(this);
            return this;
        }

        public IOverrideSkillCutinAnimation SetCharaSprite(Sprite sprite) { m_CharaSpriteRenderer.sprite = sprite;return this; }
        public IOverrideSkillCutinAnimation SetFrameSprite(Sprite sprite) { m_FrameSpriteRenderer.sprite = sprite;return this; }
        public IOverrideSkillCutinAnimation SetBackSprite(AffectOverrideType type) 
        {
            switch (type)
            {
                case AffectOverrideType.Override:
                    m_BackSpriteRenderer.sprite = m_Sprites[0];
                    break;
                case AffectOverrideType.エクサオーバーライド:
                    m_BackSpriteRenderer.sprite = m_Sprites[1];
                    break;
                case AffectOverrideType.ゼタオーバーライド:
                    m_BackSpriteRenderer.sprite = m_Sprites[2];
                    break;
                case AffectOverrideType.クエタオーバーライド:
                    m_BackSpriteRenderer.sprite = m_Sprites[3];
                    break;
            }
            return this;
        }
        public IOverrideSkillCutinAnimation SetSkillName(string text) { m_SkillNameText.text = text;return this; }

        public void Close()
        {
            ClosedDisposables.Dispose();
            Destroy(this.gameObject);
        }
    }
}
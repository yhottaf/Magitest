using UnityEngine;

namespace fantec.Battle.Field
{
    public interface IFieldBackgroundView : ILocatable
    {
        void SetSprite(Sprite sprite);
    }
}

namespace fantec.Battle.Field.Background
{
    public class FieldBackGroundView : MonoBehaviour,IFieldBackgroundView,IRegistable
    {
        [SerializeField] private SpriteRenderer m_SpriteRenderer;

        public void Register()
        {
            Locator.Register<IFieldBackgroundView>(this);
        }

        public void SetSprite(Sprite sprite)
        {
            m_SpriteRenderer.sprite=sprite;
        }
    }
}
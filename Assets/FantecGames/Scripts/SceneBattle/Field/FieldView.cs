using UnityEngine;

namespace fantec.Battle.Field
{ 
public interface IFieldView : ILocatable
    {
        void SetSprite(Sprite sprite);
    }
}

namespace fantec.Battle.Field
{
    public class FieldView : MonoBehaviour,IFieldView,IRegistable
    {
        [SerializeField] private SpriteRenderer m_SpriteRenderer;

        public void Register()
        {
            Locator.Register<IFieldView>(this);
        }

        public void SetSprite(Sprite sprite)
        {
            m_SpriteRenderer.sprite = sprite;
        }
    }
}
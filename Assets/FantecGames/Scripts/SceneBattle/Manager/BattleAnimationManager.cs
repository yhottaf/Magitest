using UnityEngine;
using fantec.Battle.Animations;

namespace fantec.Battle.Manager
{
    public interface IBattleAnimationManager:ILocatable
    {
        T Play<T>() where T : IFullScreenAnimation<T>;
        T Get<T>() where T : IFullScreenAnimation<T>;
        bool GetIsExist<T>(out T animation) where  T : IFullScreenAnimation<T>;
        void TryClose<T>()where T : IFullScreenAnimation<T>;
        void Clear();

    }

    public class BattleAnimationManager : MonoBehaviour,IBattleAnimationManager,IRegistable
    {
        [Header("Parent")]
        [SerializeField] private Transform m_Parent;

        [Header("Animations")]
        [SerializeField] AnimationBase[] m_Animations;

        public void Register()
        {
            Locator.Register<IBattleAnimationManager>(this);
        }

        public T Play<T>() where T : IFullScreenAnimation<T>
        {
            foreach(var anim in m_Animations)
            {
                if(anim is T)
                {
                    return Create<T>(anim);
                }
            }

            throw new System.Exception($"[type {typeof(T).FullName}] は未登録です。");
        }

        public T Get<T>() where T: IFullScreenAnimation<T>
        {
            for(int i=0;i<m_Parent.childCount;i++)
            {
                var target=m_Parent.GetChild(i).GetComponent<T>();
                if(target!= null)
                {
                    return target;
                }
            }

            throw new System.Exception($"{typeof(T).FullName}] をアタッチしたアニメーションが生成されていません。");
        }

        public bool GetIsExist<T>(out T animation)where T: IFullScreenAnimation<T>
        {
            for(int i=0;i<m_Parent.childCount;i++)
            {
                var target = m_Parent.GetChild(i).GetComponent<T>();
                if(target!=null)
                {
                    animation = target;
                    return true;
                }
            }

            animation = default;
            return false;
        }

        private T Create<T>(AnimationBase animBase)where T:IFullScreenAnimation<T>
        {
            var clone=Instantiate(animBase);
            clone.transform.SetParent(m_Parent);          // 親設定
            clone.transform.localPosition=Vector3.zero;   // 配置調整
            clone.transform.localScale=Vector3.one;       // サイズ調整
            return clone.GetComponent<T>().OnCreate();
        }

        public void Clear()
        {
            for(int i=m_Parent.childCount-1;0<=i;i--)
            {
                var target=m_Parent.GetChild(i).GetComponent<ICloseable>();
                if(target!=null)
                {
                    target.Close();
                }
            }
        }

        public void TryClose<T>()where T: IFullScreenAnimation<T>
        {
            if (GetIsExist<T>(out T animation))
                animation.Close();
        }
    }
}
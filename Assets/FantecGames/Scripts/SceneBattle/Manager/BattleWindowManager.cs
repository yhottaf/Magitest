using fantec.Battle.Ui.Window;
using UnityEngine;

namespace fantec.Battle.Manager
{
    public interface IBattleWindowManager:IRegistable
    {
        T Open<T>() where T : IWindow<T>;
        bool GetIs<T>(out T window)where T : IWindow<T>;
        T Create<T>()where T : IWindow<T>;
        void Close<T>() where T: IWindow<T>;
        void Clear();
    }

    public class BattleWindowManager : MonoBehaviour,IBattleWindowManager
    {
        [SerializeField] private Transform m_Parent;
        [SerializeField] private WindowBase[] m_Windows;

        public void Register()
        {
            Locator.Register<IBattleWindowManager>(this);
        }

        public T Open<T>()where T : IWindow<T>
        {
            T window;
            if(GetIs(out window))
            {
                return window;
            }
            else
            {
                return this.Create<T>();
            }
        }

        public bool GetIs<T>(out T window)where T : IWindow<T>
        {
            for(int i=0;i<m_Parent.childCount;i++)
            {
                var target = m_Parent.GetChild(i).GetComponent<T>();
                if(target!=null)
                {
                    window = target;
                    return true;
                }
            }

            window = default;
            return false;
        }

        public T Create<T>(WindowBase window) where T : IWindow<T>
        {
            var clone = Instantiate(window);
            clone.transform.SetParent(m_Parent);
            clone.transform.localPosition = Vector3.zero;
            clone.transform.localScale = Vector3.one;

            var rect = clone.GetComponent<RectTransform>();
            rect.offsetMax = Vector2.zero;
            rect.offsetMin = Vector2.zero;

            return clone.GetComponent<T>().OnCreate();
        }

        public T Create<T>() where T:IWindow<T>
        {
            foreach(var anim in m_Windows)
            {
                if(anim is T)
                {
                    return Create<T>(anim);
                }
            }

            throw new System.Exception($"[{typeof(T).FullName}]");
        }

        public void Close<T>()where T: IWindow<T>
        {
            if (GetIs(out T window))
                window.Close();
            else
                throw new System.Exception($"{typeof(T).FullName}");
        }

        public void Clear()
        {
            for (int i = m_Parent.childCount - 1; 0 <= i; i--)
            {
                var target=m_Parent.GetChild(i).GetComponent<ICloseable>();
                if(target!=null)
                {
                    target.Close();
                }
            }
        }
    }
}
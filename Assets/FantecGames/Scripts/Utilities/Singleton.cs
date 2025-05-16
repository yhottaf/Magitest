using UnityEngine;

namespace fantec.Utilities
{
    public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        private static T instance;

        public static T Instance
        {
            get
            {
                if(instance==null)
                {
                    instance=new GameObject(typeof(T).FullName).AddComponent<T>();
                }
                return instance;
            }
        }

        public static bool InstanceExists
        {
            get { return instance != null; }
        }

        protected virtual void Awake()
        {
            if(InstanceExists)
            {
                Destroy(gameObject);
            }
            else
            {
                instance = (T)this;
            }
        }

        protected virtual void OnDestroy()
        {
            if(instance==this)
            {
                instance = null;
            }
        }
    }
}
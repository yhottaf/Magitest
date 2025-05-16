namespace fantec.Utilities
{
    /// <summary>
    /// シーンをまたいで持続するシングルトン
    /// </summary>
    public class PersistentSingleton<T> : Singleton<T> where T : Singleton<T>
    {
        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);
        }
    }
}

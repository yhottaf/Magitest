using UniRx.Toolkit;
using UnityEngine;

namespace fantec.Battle.Manager.Pool
{
    public class IndexingPool<TObject> : UniRx.Toolkit.ObjectPool<AbstractIndexingBaseObject<TObject>>where TObject : Object
    {
        public readonly string key;
        private readonly AbstractIndexingBaseObject<TObject> m_PoolObject;
        private readonly Transform m_ParentTransform;
        private readonly int m_Index;

        public IndexingPool(Transform parentTransform,AbstractIndexingBaseObject<TObject>poolObject,int index)
        {
            this.key = poolObject.name;
            m_ParentTransform= parentTransform;
            m_PoolObject = poolObject;
            m_Index = index;
        }

        protected override AbstractIndexingBaseObject<TObject>CreateInstance()
        {
            var prefab = Object.Instantiate(m_PoolObject);
            prefab.SetIndex(m_Index);
            prefab.transform.SetParent(m_ParentTransform);
            return prefab;
        }
    }
}
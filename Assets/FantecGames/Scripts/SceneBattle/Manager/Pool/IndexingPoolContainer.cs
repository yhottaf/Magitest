using System;
using UnityEngine;
using UniRx;
using System.Linq;

namespace fantec.Battle.Manager.Pool
{
    [Serializable]
    public class IndexingPoolContainer<TObject>where TObject:AbstractIndexingBaseObject<TObject>
    {
        [SerializeField] private TObject[] m_PoolObjects;

        private IndexingPool<TObject>[] m_Pools;

        public void Initialize(Transform parentTransform)
        {
            m_Pools=new IndexingPool<TObject>[m_PoolObjects.Length];

            for(int i=0;i<m_PoolObjects.Length;i++)
            {
                var index = i;

                try
                {
                    var a = new IndexingPool<TObject>(parentTransform, m_PoolObjects[index], index);
                    m_Pools[index] = a;
                    m_Pools[index].PreloadAsync(3, 1).Subscribe();
                }
                catch (IndexOutOfRangeException) { throw new IndexOutOfRangeException($"[index : {index}] は配列の範囲外です。"); }
                catch (NullReferenceException) { throw new NullReferenceException($"[index : {index}] 内の要素が null です。"); }
            }
        }

        public TObject Rent(int index)
        {
            try { return m_Pools[index].Rent() as TObject; }
            catch { throw new IndexOutOfRangeException($"[index : {index}] はプールの範囲外です。"); }
        }

        public TObject Rent(string key)
        {
            try
            {
                var pool = m_Pools.First(x => x.key == key).Rent();
                if (pool is TObject)
                {
                    return pool as TObject;
                }
                else throw new InvalidCastException();
            }
            catch (InvalidCastException) { throw new InvalidCastException("キャストに失敗しました。"); }
            catch (InvalidOperationException) { throw new InvalidOperationException($"[key : {key}] が見つかりません。"); }
        }

        public void Return(TObject effectObject)
        {
            m_Pools[effectObject.Index].Return(effectObject);
        }
    }
}
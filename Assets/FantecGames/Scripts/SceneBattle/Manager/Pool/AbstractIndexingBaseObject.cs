using fantec.Battle.Utiles;
using UnityEngine;
using UnityEngine.Rendering;

namespace fantec.Battle.Manager.Pool
{
    [RequireComponent(typeof(SortingGroup))]
    public abstract class AbstractIndexingBaseObject<TObject> : SpeedableBehaviour where TObject : Object
    {
        [SerializeField] protected SortingGroup m_SortingGroup;
        [SerializeField] protected float m_LifeTime = 1.0f;
        [SerializeField] protected float m_Size = 1.0f;

        protected readonly SaveableSequence m_Sequence=new SaveableSequence();

        public int Index { get; private set; }

        protected virtual void Reset()
        {
            m_SortingGroup=this.GetComponent<SortingGroup>();  
        }

        public abstract void Return();

        public TObject SetPosition(Vector3 position)
        {
            this.transform.position = position;
            return this as TObject;
        }

        public TObject SetRandom(float range)
        {
            var randomOffset = Vector3.zero;
            randomOffset.x += Random.Range(-range, range);
            randomOffset.y+= Random.Range(-range, range);
            this.transform.localPosition += randomOffset;

            return this as TObject;
        }

        public TObject SetScale(float size)
        {
            this.transform.GetChild(0).localScale = Vector3.one * size;

            return this as TObject;
        }

        public void SetIndex(int index)
        {
            Index = index;
        }
    }
}
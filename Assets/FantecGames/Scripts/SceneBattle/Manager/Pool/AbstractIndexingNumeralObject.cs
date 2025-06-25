using UnityEngine;
using fantec.Utilities;
using UnityEngine.Rendering;

namespace fantec.Battle.Manager.Pool
{
    public abstract class AbstractIndexingNumeralObject<TObject> : AbstractIndexingBaseObject<TObject>where TObject : Object
    {
        [SerializeField] protected SpriteNumber m_SpriteNumber;

        protected override void Awake()
        {
            m_SortingGroup = GetComponent<SortingGroup>();
            m_SortingGroup.sortingLayerName = BD.SortingLayer.NAME_FIELD_DEFAULT;
            m_SortingGroup.sortingOrder = (int)BD.SortingLayer.FieldOrder.Numeral;

            base.Awake();
        }

        protected override void OnSetCurrentTimeScale(float speed)
        {
            base.m_Sequence.TimeScale=speed;
        }
    }
}
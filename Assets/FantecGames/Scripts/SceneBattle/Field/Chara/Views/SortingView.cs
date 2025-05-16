using UnityEngine;
using UnityEngine.Rendering;

namespace fantec.Battle.Field.Chara
{
    public class SortingView : MonoBehaviour,IInitializable,IReloadable
    {
        [SerializeField] SortingGroup m_SortingGroup;

        public void Initialize()
        {
            Reload();
        }

        public void Reload()
        {
            SetSortingNameDefault();
        }

        public void SetSortingNameDefault()
        {
            m_SortingGroup.sortingLayerName = BD.SortingLayer.NAME_FIELD_DEFAULT;
        }

        public void SetSortingNameBlack()
        {
            m_SortingGroup.sortingLayerName = BD.SortingLayer.NAME_FIELD_BLACKOUT;
        }

        public void SetSortingOrder(BD.SortingLayer.FieldOrder order)
        {
            m_SortingGroup.sortingOrder = (int)order;
        }
    }
}
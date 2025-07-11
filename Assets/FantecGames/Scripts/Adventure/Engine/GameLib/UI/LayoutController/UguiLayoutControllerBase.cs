using UnityEngine;
using UnityEngine.UI;

namespace fantec
{


    /// <summary>
    ///  LayoutControllerの拡張クラスを作る場合の基底クラス
    /// </summary>
    [ExecuteInEditMode]
    public abstract class UguiLayoutControllerBase : MonoBehaviour
    {
        public bool CheckTransformChanged
        {
            get { return checkTransformChanged; }
            set { checkTransformChanged = value; }
        }
        [SerializeField]
        bool checkTransformChanged = true;

        RectTransform cachedRectTransform;
        public RectTransform CachedRectTransform { get { if (this.cachedRectTransform == null) cachedRectTransform = GetComponent<RectTransform>(); return cachedRectTransform; } }

        protected DrivenRectTransformTracker tracker;

        protected virtual void OnEnable()
        {
            SetDirty();
        }

        protected virtual void OnDisable()
        {
            tracker.Clear();
        }


#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            SetDirty();
        }
#endif

        protected void SetDirty()
        {
            if (!this.gameObject.activeInHierarchy)
                return;

            LayoutRebuilder.MarkLayoutForRebuild(CachedRectTransform);
        }

        protected virtual void Update()
        {
            if (!CheckTransformChanged || CheckTransform())
            {
                SetDirty();
            }
        }


        bool CheckTransform()
        {
            bool hasChanged = false;

            if (CachedRectTransform.hasChanged)
            {
                CachedRectTransform.hasChanged = false;
                hasChanged = true;
            }
            int count = this.transform.childCount;
            for (int i = 0; i < count; ++i)
            {
                RectTransform childRect = this.transform.GetChild(i) as RectTransform;

                if (childRect == null) continue;
                if (childRect.hasChanged)
                {
                    childRect.hasChanged = false;
                    hasChanged = true;
                }
            }
            return hasChanged;
        }
    }
}

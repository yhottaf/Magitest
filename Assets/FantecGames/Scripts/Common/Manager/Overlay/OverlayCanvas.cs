using UnityEngine;
using fantec.Utilities;

namespace fantec.Common {

    public class OverlayCanvas : PersistentSingleton<OverlayCanvas>
    {
        [SerializeField] private RectTransform m_ContentRect;
        public RectTransform ContentRect { get { return m_ContentRect; } }
        private Canvas m_Canvas;

        protected override void Awake()
        {
            base.Awake();
            m_Canvas = this.GetComponent<Canvas>();
        }

        private void Update()
        {
            if (m_Canvas.worldCamera == null)
            {
                m_Canvas.worldCamera = Camera.main;
                m_Canvas.sortingLayerName = Define.SortingLayer.NAME_OVERLAY_CANVAS;
            }

            if (Input.GetKeyDown(KeyCode.Q)) Modal.Clear();
        }
    }
}
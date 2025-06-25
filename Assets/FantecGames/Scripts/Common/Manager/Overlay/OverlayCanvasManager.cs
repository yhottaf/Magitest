using Cysharp.Threading.Tasks;
using fantec.Extensions;
using fantec.Utilities;
using System.Threading;
using UnityEngine;

namespace fantec.Common
{
    public class OverlayCanvasManager:PersistentSingleton<OverlayCanvasManager>
    {
        private OverlayCanvas m_OverlayCanvas;

        private UniTaskCompletionSource<bool> _initTcs = new UniTaskCompletionSource<bool>();

        public UniTask WaitUntilInitializedAsync() => _initTcs.Task;


        protected override void Awake()
        {
            base.Awake();
            //UniTask.Void(async () =>
            //{
            //    GameObject go = await AssetManager.Instance.LoadAssetAsync<GameObject>(AssetPath.UiOverlayCanvas);
            //    OverlayCanvas view = go.GetComponent<OverlayCanvas>();
            //    m_OverlayCanvas =  Instantiate(view);
            //    _initTcs.TrySetResult(true);
            //});
            m_OverlayCanvas = Instantiate(AssetManager.Instance.LoadAsset<OverlayCanvas>(AssetPath.UiOverlayCanvas));
        }

        /// <summary>
        /// オーバーレイキャンパスの仲間として迎え入れる
        /// </summary>
        /// <param name="overlayObject">歓迎対象</param>
        public void Add(OverlayObject overlayObject)
        {
            RectTransform rectTransform=overlayObject.GetComponent<RectTransform>();
            if(rectTransform==null)rectTransform=overlayObject.gameObject.AddComponent<RectTransform>();
            rectTransform.SetParent(m_OverlayCanvas.ContentRect);
            rectTransform.FullStretch();
        }

        public async UniTask<T> CreateAsync<T>(string assetPath, CancellationToken cts = default) where T : OverlayObject
        {
            GameObject go = await AssetManager.Instance.LoadAssetAsync<GameObject>(assetPath, cts);
            T prefab = go.GetComponent<T>();
            var instance = Instantiate(prefab);
            Add(instance);
            return instance;
        }

        public void Remove<T>() where T : OverlayObject
        {
            var count = m_OverlayCanvas.ContentRect.childCount;
            if (count == 0) return;
            for (int i = count - 1; 0 <= i; i--)
            {
                var child = m_OverlayCanvas.ContentRect.GetChild(i).GetComponent<OverlayObject>();
                if (child is T)
                {
                    Destroy(child.gameObject);
                }
            }
        }

        public T Create<T>(string assetPath) where T : OverlayObject
        {
            var instance = Instantiate(AssetManager.Instance.LoadAsset<T>(assetPath));
            Add(instance);
            return instance as T;
        }
    }
}

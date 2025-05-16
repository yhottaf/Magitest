using Cysharp.Threading.Tasks;
using fantec.Utilities;
using UnityEngine;
using UniRx;
using DG.Tweening;

namespace fantec.Debugger
{
    public class DebuggerItemPresenter : ItemDuplicator<DebuggerItemPresenter>
    {
        [SerializeField] private DebuggerItemView m_View;
        [SerializeField] private DebuggerInItemPresenter m_InItemPresenter;

        private Vector2 m_OriginalSize;
        private Vector2 m_TargetSize;

        private void Awake()
        {
            // 複製用の為隠す
            m_InItemPresenter.gameObject.SetActive(false);
        }

        protected override void Release()
        {
        }

        protected override void Setup(ItemDuplicatorData data)
        {
            // データ取得
            var itemData = data as DebuggerItemData;

            // タイトル設定
            m_View.SetTitleText(itemData.titleText);

            // 子階層作成
            foreach(var inItem in itemData.inItemDatas)
            {
                m_InItemPresenter.Create(inItem);
            }

            // 開閉モーション作成
            var itemRect=GetComponent<RectTransform>();
            var initemRect=m_InItemPresenter.GetComponent<RectTransform>();
            m_OriginalSize = itemRect.sizeDelta;
            m_TargetSize= itemRect.sizeDelta;
            m_TargetSize.y=itemRect.sizeDelta.y+(initemRect.parent.childCount-1)*initemRect.sizeDelta.y;
            m_View.OnValueChangedToggleObservable.Subscribe(enabled =>
            {
                itemRect.DOSizeDelta(enabled ? m_TargetSize : m_OriginalSize, 0.3f).SetLink(this.gameObject);
            }).AddTo(this);
        }
    }


    public class DebuggerItemData:ItemDuplicatorData
    {
        public string titleText;
        public DebuggerInItemData[] inItemDatas;
    }
}
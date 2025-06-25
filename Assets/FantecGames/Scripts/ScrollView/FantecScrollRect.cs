using UnityEngine;

using System;
using System.Collections.Generic;
using EasingCore;
using fantec;

namespace FantecScrollView
{
    /// <summary>
    /// ScrollRect スタイルのスクロールビューを実装するための抽象基底クラス.
    /// 無限スクロールおよびスナップには対応していません.
    /// <see cref="FantecScrollView{TItemData, TContext}.Context"/> が不要な場合は
    /// 代わりに <see cref="FantecScrollRect{TItemData}"/> を使用します.
    /// </summary>
    /// <typeparam name="TItemData">アイテムのデータ型.</typeparam>
    /// <typeparam name="TContext"><see cref="FantecScrollView{TItemData, TContext}.Context"/> の型.</typeparam>
    [RequireComponent(typeof(Scroller))]
    public abstract class FantecScrollRect<TItemData, TContext> : FantecScrollView<TItemData, TContext>
        where TContext : class, IFantecScrollRectContext, new()
    {
        /// <summary>
        /// スクロール中にセルが再利用されるまでの余白のセル数.
        /// </summary>
        /// <remarks>
        /// <c>0</c> を指定するとセルが完全に隠れた直後に再利用されます.
        /// <c>1</c> 以上を指定すると, そのセル数だけ余分にスクロールしてから再利用されます.
        /// </remarks>
        [SerializeField] protected float reuseCellMarginCount = 0f;

        /// <summary>
        /// コンテンツ先頭の余白.
        /// </summary>
        [SerializeField] protected float paddingHead = 0f;

        /// <summary>
        /// コンテンツ末尾の余白.
        /// </summary>
        [SerializeField] protected float paddingTail = 0f;

        /// <summary>
        /// スクロール軸方向のセル同士の余白.
        /// </summary>
        [SerializeField] protected float spacing = 0f;

        /// <summary>
        /// セルのサイズ.
        /// </summary>
        protected abstract float CellSize { get; }

        /// <summary>
        /// スクロール可能かどうか.
        /// </summary>
        /// <remarks>
        /// アイテム数が十分少なくビューポート内に全てのセルが収まっている場合は <c>false</c>, それ以外は <c>true</c> になります.
        /// </remarks>
        protected virtual bool Scrollable => MaxScrollPosition > 0f;

        Scroller cachedScroller;

        /// <summary>
        /// スクロール位置を制御する <see cref="FantecScrollView.Scroller"/> のインスタンス.
        /// </summary>
        /// <remarks>
        /// <see cref="Scroller"/> のスクロール位置を変更する際は必ず <see cref="ToScrollerPosition(float)"/> を使用して変換した位置を使用してください.
        /// </remarks>
        protected Scroller Scroller => cachedScroller ?? (cachedScroller = GetComponent<Scroller>());

        float ScrollLength => 1f / Mathf.Max(cellInterval, 1e-2f) - 1f;

        float ViewportLength => ScrollLength - reuseCellMarginCount * 2f;

        float PaddingHeadLength => (paddingHead - spacing * 0.5f) / (CellSize + spacing);

        float MaxScrollPosition => ItemsSource.Count
            - ScrollLength
            + reuseCellMarginCount * 2f
            + (paddingHead + paddingTail - spacing) / (CellSize + spacing);

        /// <inheritdoc/>
        protected override void Initialize()
        {
            base.Initialize();

            Context.ScrollDirection = Scroller.ScrollDirection;
            Context.CalculateScrollSize = () =>
            {
                var interval = CellSize + spacing;
                var reuseMargin = interval * reuseCellMarginCount;
                var scrollSize = Scroller.ViewportSize + interval + reuseMargin * 2f;
                return (scrollSize, reuseMargin);
            };

            AdjustCellIntervalAndScrollOffset();
            Scroller.OnValueChanged(OnScrollerValueChanged);
        }

        /// <summary>
        /// <see cref="Scroller"/> のスクロール位置が変更された際の処理.
        /// </summary>
        /// <param name="p"><see cref="Scroller"/> のスクロール位置.</param>
        void OnScrollerValueChanged(float p)
        {
            base.UpdatePosition(ToFantecScrollViewPosition(Scrollable ? p : 0f));

            if (Scroller.Scrollbar)
            {
                if (p > ItemsSource.Count - 1)
                {
                    ShrinkScrollbar(p - (ItemsSource.Count - 1));
                }
                else if (p < 0f)
                {
                    ShrinkScrollbar(-p);
                }
            }
        }

        /// <summary>
        /// スクロール範囲を超えてスクロールされた量に基づいて, スクロールバーのサイズを縮小します.
        /// </summary>
        /// <param name="offset">スクロール範囲を超えてスクロールされた量.</param>
        void ShrinkScrollbar(float offset)
        {
            var scale = 1f - ToFantecScrollViewPosition(offset) / (ViewportLength - PaddingHeadLength);
            UpdateScrollbarSize((ViewportLength - PaddingHeadLength) * scale);
        }

        /// <inheritdoc/>
        protected override void Refresh()
        {
            AdjustCellIntervalAndScrollOffset();
            RefreshScroller();
            base.Refresh();
        }

        /// <inheritdoc/>
        protected override void Relayout()
        {
            AdjustCellIntervalAndScrollOffset();
            RefreshScroller();
            base.Relayout();
        }

        /// <summary>
        /// <see cref="Scroller"/> の各種状態を更新します.
        /// </summary>
        protected void RefreshScroller()
        {
            Scroller.Draggable = Scrollable;
            Scroller.ScrollSensitivity = ToScrollerPosition(ViewportLength - PaddingHeadLength);
            Scroller.Position = ToScrollerPosition(currentPosition);

            if (Scroller.Scrollbar)
            {
                Scroller.Scrollbar.gameObject.SetActive(Scrollable);
                UpdateScrollbarSize(ViewportLength);
            }
        }

        /// <inheritdoc/>
        protected override void UpdateContents(IList<TItemData> items)
        {
            AdjustCellIntervalAndScrollOffset();
            base.UpdateContents(items);

            Scroller.SetTotalCount(items.Count);
            RefreshScroller();
        }

        /// <summary>
        /// スクロール位置を更新します.
        /// </summary>
        /// <param name="position">スクロール位置.</param>
        protected new void UpdatePosition(float position)
        {
            Scroller.Position = ToScrollerPosition(position, 0.5f);
        }

        /// <summary>
        /// 指定したアイテムの位置までジャンプします.
        /// </summary>
        /// <param name="itemIndex">アイテムのインデックス.</param>
        /// <param name="alignment">ビューポート内におけるセル位置の基準. 0f(先頭) ~ 1f(末尾).</param>
        protected virtual void JumpTo(int itemIndex, float alignment = 0.5f)
        {
            Scroller.Position = ToScrollerPosition(itemIndex, alignment);
        }

        /// <summary>
        /// 指定したアイテムの位置まで移動します.
        /// </summary>
        /// <param name="index">アイテムのインデックス.</param>
        /// <param name="duration">移動にかける秒数.</param>
        /// <param name="alignment">ビューポート内におけるセル位置の基準. 0f(先頭) ~ 1f(末尾).</param>
        /// <param name="onComplete">移動が完了した際に呼び出されるコールバック.</param>
        protected virtual void ScrollTo(int index, float duration, float alignment = 0.5f, Action onComplete = null)
        {
            Scroller.ScrollTo(ToScrollerPosition(index, alignment), duration, onComplete);
        }

        /// <summary>
        /// 指定したアイテムの位置まで移動します.
        /// </summary>
        /// <param name="index">アイテムのインデックス.</param>
        /// <param name="duration">移動にかける秒数.</param>
        /// <param name="easing">移動に使用するイージング.</param>
        /// <param name="alignment">ビューポート内におけるセル位置の基準. 0f(先頭) ~ 1f(末尾).</param>
        /// <param name="onComplete">移動が完了した際に呼び出されるコールバック.</param>
        protected virtual void ScrollTo(int index, float duration, Ease easing, float alignment = 0.5f, Action onComplete = null)
        {
            Scroller.ScrollTo(ToScrollerPosition(index, alignment), duration, easing, onComplete);
        }

        /// <summary>
        /// ビューポートとコンテンツの長さに基づいてスクロールバーのサイズを更新します.
        /// </summary>
        /// <param name="viewportLength">ビューポートのサイズ.</param>
        protected void UpdateScrollbarSize(float viewportLength)
        {
            var contentLength = Mathf.Max(ItemsSource.Count + (paddingHead + paddingTail - spacing) / (CellSize + spacing), 1);
            Scroller.Scrollbar.size = Scrollable ? Mathf.Clamp01(viewportLength / contentLength) : 1f;
        }

        /// <summary>
        /// <see cref="Scroller"/> が扱うスクロール位置を <see cref="FantecScrollRect{TItemData, TContext}"/> が扱うスクロール位置に変換します.
        /// </summary>
        /// <param name="position"><see cref="Scroller"/> が扱うスクロール位置.</param>
        /// <returns><see cref="FantecScrollRect{TItemData, TContext}"/> が扱うスクロール位置.</returns>
        protected float ToFantecScrollViewPosition(float position)
        {
            return position / Mathf.Max(ItemsSource.Count - 1, 1) * MaxScrollPosition - PaddingHeadLength;
        }

        /// <summary>
        /// <see cref="FantecScrollRect{TItemData, TContext}"/> が扱うスクロール位置を <see cref="Scroller"/> が扱うスクロール位置に変換します.
        /// </summary>
        /// <param name="position"><see cref="FantecScrollRect{TItemData, TContext}"/> が扱うスクロール位置.</param>
        /// <returns><see cref="Scroller"/> が扱うスクロール位置.</returns>
        protected float ToScrollerPosition(float position)
        {
            return (position + PaddingHeadLength) / MaxScrollPosition * Mathf.Max(ItemsSource.Count - 1, 1);
        }

        /// <summary>
        /// <see cref="FantecScrollRect{TItemData, TContext}"/> が扱うスクロール位置を <see cref="Scroller"/> が扱うスクロール位置に変換します.
        /// </summary>
        /// <param name="position"><see cref="FantecScrollRect{TItemData, TContext}"/> が扱うスクロール位置.</param>
        /// <param name="alignment">ビューポート内におけるセル位置の基準. 0f(先頭) ~ 1f(末尾).</param>
        /// <returns><see cref="Scroller"/> が扱うスクロール位置.</returns>
        protected float ToScrollerPosition(float position, float alignment = 0.5f)
        {
            var offset = alignment * (ScrollLength - (1f + reuseCellMarginCount * 2f))
                + (1f - alignment - 0.5f) * spacing / (CellSize + spacing);
            return ToScrollerPosition(Mathf.Clamp(position - offset, 0f, MaxScrollPosition));
        }

        /// <summary>
        /// 指定された設定を実現するための
        /// <see cref="FantecScrollView{TItemData,TContext}.cellInterval"/> と
        /// <see cref="FantecScrollView{TItemData,TContext}.scrollOffset"/> を計算して適用します.
        /// </summary>
        protected void AdjustCellIntervalAndScrollOffset()
        {
            var totalSize = Scroller.ViewportSize + (CellSize + spacing) * (1f + reuseCellMarginCount * 2f);
            cellInterval = (CellSize + spacing) / totalSize;
            scrollOffset = cellInterval * (1f + reuseCellMarginCount);
        }

        protected virtual void OnValidate()
        {
            AdjustCellIntervalAndScrollOffset();

            if (loop)
            {
                loop = false;
                Debug.LogError("Loop is currently not supported in FantecScrollRect.");
            }

            if (Scroller.SnapEnabled)
            {
                Scroller.SnapEnabled = false;
                Debug.LogError("Snap is currently not supported in FantecScrollRect.");
            }

            if (Scroller.MovementType == MovementType.Unrestricted)
            {
                Scroller.MovementType = MovementType.Elastic;
                Debug.LogError("MovementType.Unrestricted is currently not supported in FantecScrollRect.");
            }
        }
    }

    /// <summary>
    /// ScrollRect スタイルのスクロールビューを実装するための抽象基底クラス.
    /// 無限スクロールおよびスナップには対応していません.
    /// </summary>
    /// <typeparam name="TItemData">アイテムのデータ型.</typeparam>
    /// <seealso cref="FantecScrollRect{TItemData, TContext}"/>
    public abstract class FantecScrollRect<TItemData> : FantecScrollRect<TItemData, FantecScrollRectContext> { }
}

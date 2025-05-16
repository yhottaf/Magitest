using fantec;
using UnityEngine;

namespace FantecScrollView
{
    /// <summary>
    /// <see cref="FantecScrollRect{TItemData, TContext}"/> のセルを実装するための抽象基底クラス.
    /// <see cref="FantecCell{TItemData, TContext}.Context"/> が不要な場合は
    /// 代わりに <see cref="FantecScrollRectCell{TItemData}"/> を使用します.
    /// </summary>
    /// <typeparam name="TItemData">アイテムのデータ型.</typeparam>
    /// <typeparam name="TContext"><see cref="FantecCell{TItemData, TContext}.Context"/> の型.</typeparam>
    public abstract class FantecScrollRectCell<TItemData, TContext> : FantecCell<TItemData, TContext>
        where TContext : class, IFantecScrollRectContext, new()
    {
        /// <inheritdoc/>
        public override void UpdatePosition(float position)
        {
            var (scrollSize, reuseMargin) = Context.CalculateScrollSize();

            var normalizedPosition = (Mathf.Lerp(0f, scrollSize, position) - reuseMargin) / (scrollSize - reuseMargin * 2f);

            var start = 0.5f * scrollSize;
            var end = -start;

            UpdatePosition(normalizedPosition, Mathf.Lerp(start, end, position));
        }

        /// <summary>
        /// このセルの位置を更新します.
        /// </summary>
        /// <param name="normalizedPosition">
        /// ビューポートの範囲で正規化されたスクロール位置.
        /// <see cref="FantecScrollRect{TItemData, TContext}.reuseCellMarginCount"/> の値に基づいて
        ///  <c>0.0</c> ~ <c>1.0</c> の範囲を超えた値が渡されることがあります.
        /// </param>
        /// <param name="localPosition">ローカル位置.</param>
        protected virtual void UpdatePosition(float normalizedPosition, float localPosition)
        {
            transform.localPosition = Context.ScrollDirection == ScrollDirection.Horizontal
                ? new Vector2(-localPosition, 0)
                : new Vector2(0, localPosition);
        }
    }

    /// <summary>
    /// <see cref="FantecScrollRect{TItemData}"/> のセルを実装するための抽象基底クラス.
    /// </summary>
    /// <typeparam name="TItemData">アイテムのデータ型.</typeparam>
    /// <seealso cref="FantecScrollRectCell{TItemData, TContext}"/>
    public abstract class FantecScrollRectCell<TItemData> : FantecScrollRectCell<TItemData, FantecScrollRectContext>
    {
        /// <inheritdoc/>
        public sealed override void SetContext(FantecScrollRectContext context) => base.SetContext(context);
    }
}

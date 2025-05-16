using fantec;
using UnityEngine;

namespace FantecScrollView
{
    /// <summary>
    /// <see cref="FantecGridView{TItemData, TContext}"/> のセルを実装するための抽象基底クラス.
    /// <see cref="FantecCell{TItemData, TContext}.Context"/> が不要な場合は
    /// 代わりに <see cref="FantecGridViewCell{TItemData}"/> を使用します.
    /// </summary>
    /// <typeparam name="TItemData">アイテムのデータ型.</typeparam>
    /// <typeparam name="TContext"><see cref="FancyCell{TItemData, TContext}.Context"/> の型.</typeparam>
    public abstract class FantecGridViewCell<TItemData, TContext> : FantecScrollRectCell<TItemData, TContext>
        where TContext : class, IFantecGridViewContext, new()
    {
        /// <inheritdoc/>
        protected override void UpdatePosition(float normalizedPosition, float localPosition)
        {
            var cellSize = Context.GetCellSize();
            var spacing = Context.GetStartAxisSpacing();
            var groupCount = Context.GetGroupCount();

            var indexInGroup = Index % groupCount;
            var positionInGroup = (cellSize + spacing) * (indexInGroup - (groupCount - 1) * 0.5f);

            transform.localPosition = Context.ScrollDirection == ScrollDirection.Horizontal
                ? new Vector2(-localPosition, -positionInGroup)
                : new Vector2(positionInGroup, localPosition);
        }
    }

    /// <summary>
    /// <see cref="FantecGridView{TItemData}"/> のセルを実装するための抽象基底クラス.
    /// </summary>
    /// <typeparam name="TItemData">アイテムのデータ型.</typeparam>
    /// <seealso cref="FantecGridViewCell{TItemData, TContext}"/>
    public abstract class FantecGridViewCell<TItemData> : FantecGridViewCell<TItemData, FantecGridViewContext>
    {
        /// <inheritdoc/>
        public sealed override void SetContext(FantecGridViewContext context) => base.SetContext(context);
    }
}

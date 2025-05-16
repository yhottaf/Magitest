using fantec;
using System;

namespace FantecScrollView
{
    /// <summary>
    /// <see cref="FantecScrollRect{TItemData, TContext}"/> のコンテキスト基底クラス.
    /// </summary>
    public class FantecScrollRectContext : IFantecScrollRectContext
    {
        ScrollDirection IFantecScrollRectContext.ScrollDirection { get; set; }
        Func<(float ScrollSize, float ReuseMargin)> IFantecScrollRectContext.CalculateScrollSize { get; set; }
    }
}

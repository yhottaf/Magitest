using fantec;
using System;
using UnityEngine;

namespace FantecScrollView
{
    /// <summary>
    /// <see cref="FantecGridView{TItemData, TContext}"/> のコンテキストインターフェース.
    /// </summary>
    public interface IFantecGridViewContext : IFantecScrollRectContext, IFantecCellGroupContext
    {
        Func<float> GetStartAxisSpacing { get; set; }
        Func<float> GetCellSize { get; set; }
    }

    public class FantecGridViewContext : IFantecGridViewContext
    {
        ScrollDirection IFantecScrollRectContext.ScrollDirection { get; set; }
        Func<(float ScrollSize, float ReuseMargin)> IFantecScrollRectContext.CalculateScrollSize { get; set; }
        GameObject IFantecCellGroupContext.CellTemplate { get; set; }
        Func<int> IFantecCellGroupContext.GetGroupCount { get; set; }
        Func<float> IFantecGridViewContext.GetStartAxisSpacing { get; set; }
        Func<float> IFantecGridViewContext.GetCellSize { get; set; }
    }
}
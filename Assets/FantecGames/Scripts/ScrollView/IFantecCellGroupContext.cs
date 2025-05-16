using UnityEngine;

using System;

namespace FantecScrollView
{
    /// <summary>
    /// <see cref="FantecCellGroup{TItemData, TContext}"/> のコンテキストインターフェース.
    /// </summary>
    public interface IFantecCellGroupContext
    {
        GameObject CellTemplate { get; set; }
        Func<int> GetGroupCount { get; set; }
    }
}

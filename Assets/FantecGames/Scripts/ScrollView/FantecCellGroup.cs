using UnityEngine;
using System.Linq;

namespace FantecScrollView
{
    /// <summary>
    /// 複数の <see cref="FantecCell{TItemData, TContext}"/> を持つセルグループ実装するための抽象基底クラス.
    /// </summary>
    /// <typeparam name="TItemData">アイテムのデータ型.</typeparam>
    /// <typeparam name="TContext"><see cref="FantecCell{TItemData, TContext}.Context"/> の型.</typeparam>
    public abstract class FantecCellGroup<TItemData, TContext> : FantecCell<TItemData[], TContext>
        where TContext : class, IFantecCellGroupContext, new()
    {
        /// <summary>
        /// このグループで表示するセルの配列.
        /// </summary>
        protected virtual FantecCell<TItemData, TContext>[] Cells { get; private set; }

        /// <summary>
        /// このグループで表示するセルの配列をインスタンス化します.
        /// </summary>
        /// <returns>このグループで表示するセルの配列.</returns>
        protected virtual FantecCell<TItemData, TContext>[] InstantiateCells()
        {
            return Enumerable.Range(0, Context.GetGroupCount())
                .Select(_ => Instantiate(Context.CellTemplate, transform))
                .Select(x => x.GetComponent<FantecCell<TItemData, TContext>>())
                .ToArray();
        }

        /// <inheritdoc/>
        public override void Initialize()
        {
            Cells = InstantiateCells();
            Debug.Assert(Cells.Length == Context.GetGroupCount());

            for (var i = 0; i < Cells.Length; i++)
            {
                Cells[i].SetContext(Context);
                Cells[i].Initialize();
            }
        }

        /// <inheritdoc/>
        public override void UpdateContent(TItemData[] contents)
        {
            var firstCellIndex = Index * Context.GetGroupCount();

            for (var i = 0; i < Cells.Length; i++)
            {
                Cells[i].Index = i + firstCellIndex;
                Cells[i].SetVisible(i < contents.Length);

                if (Cells[i].IsVisible)
                {
                    Cells[i].UpdateContent(contents[i]);
                }
            }
        }

        /// <inheritdoc/>
        public override void UpdatePosition(float position)
        {
            for (var i = 0; i < Cells.Length; i++)
            {
                Cells[i].UpdatePosition(position);
            }
        }
    }
}

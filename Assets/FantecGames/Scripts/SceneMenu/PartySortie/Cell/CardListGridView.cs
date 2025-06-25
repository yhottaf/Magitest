using UnityEngine;
using FantecScrollView;
using fantec.Common;
using System;
using EasingCore;

namespace fantec.Menu.Card
{
     class CardListGridView : FantecGridView<CardData,CardListContext> 
    {
        class CellGroup : DefaultCellGroup { }

        [SerializeField]
        private CardListCell cellPrefab = default;

        protected override void SetupCellTemplate() => Setup<CellGroup>(cellPrefab);

        public float PaddingTop
        {
            get => paddingHead;
            set
            {
                paddingHead = value;
                Relayout();
            }
        }

        public float PaddingBottom
        {
            get => paddingTail;
            set
            {
                paddingTail = value;
                Relayout();
            }
        }

        public float SpacingY
        {
            get => spacing;
            set
            {
                spacing = value;
                Relayout();
            }
        }

        public float SpacingX
        {
            get => startAxisSpacing;
            set
            {
                startAxisSpacing = value;
                Relayout();
            }
        }

        public void UpdateSelection(int index)
        {
            if (Context.SelectedIndex == index)
            {
                return;
            }

            Context.SelectedIndex = index;
            Refresh();
        }

        public void OnCellClicked(Action<int> callback)
        {
            Context.OnCellClicked = callback;
        }

        public void OnCellLongTaped(Action<int>callback)
        {
            Context.OnCellLongTaped = callback;
        }

        public void ScrollTo(int index, float duration, Ease easing)
        {
            UpdateSelection(index);
            ScrollTo(index, duration, easing, 0.5f);
        }

        public void JumpTo(int index)
        {
            UpdateSelection(index);
            JumpTo(index, 0.5f);
        }
    }

    class CardListContext:FantecGridViewContext
    {
        public int SelectedIndex = -1;
        public Action<int> OnCellClicked;
        public Action<int> OnCellLongTaped;
    }
}
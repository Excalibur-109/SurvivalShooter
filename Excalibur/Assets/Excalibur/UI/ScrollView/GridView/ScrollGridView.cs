using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Excalibur.EasingCore;

namespace Excalibur
{
    public abstract class ScrollGridView<TItemData, TContext> : ScrollRect<TItemData[], TContext>
        where TContext : class, IScrollGridViewContext, new ()
    {
        protected abstract class DefaultCellGroup : ScrollCellGroup<TItemData, TContext> { }

        [SerializeField] protected float startAxisSpacing = 0f;

        [SerializeField] protected int startAxisCellCount = 4;

        [SerializeField] protected Vector2 cellSize = new Vector2 (100f, 100f);

        protected sealed override GameObject CellPrefab => cellGroupTemplate;

        protected override float CellSize => Scroller.ScrollDirection == ScrollDirection.Horizontal
            ? cellSize.x
            : cellSize.y;

        public int DataCount { get; private set; }

        GameObject cellGroupTemplate;

        protected override void Initialize ()
        {
            base.Initialize ();

            Debug.Assert (startAxisCellCount > 0);

            Context.ScrollDirection = Scroller.ScrollDirection;
            Context.GetGroupCount = () => startAxisCellCount;
            Context.GetStartAxisSpacing = () => startAxisSpacing;
            Context.GetCellSize = () => Scroller.ScrollDirection == ScrollDirection.Horizontal
                ? cellSize.y
                : cellSize.x;

            SetupCellTemplate ();
        }

        protected abstract void SetupCellTemplate ();

        protected virtual void Setup<TGroup> (ScrollCell<TItemData, TContext> cellTemplate)
            where TGroup : ScrollCell<TItemData[], TContext>
        {
            Context.CellTemplate = cellTemplate.gameObject;

            cellGroupTemplate = new GameObject ("Group").AddComponent<TGroup> ().gameObject;
            cellGroupTemplate.transform.SetParent (cellContainer, false);
            cellGroupTemplate.SetActive (false);
        }

        public virtual void UpdateContents (IList<TItemData> items)
        {
            DataCount = items.Count;

            var itemGroups = items
                .Select ( (item, index) => (item, index))
                .GroupBy (
                    x => x.index / startAxisCellCount,
                    x => x.item)
                .Select (group => group.ToArray ())
                .ToArray ();

            UpdateContents (itemGroups);
        }

        protected override void JumpTo (int itemIndex, float alignment = 0.5f)
        {
            var groupIndex = itemIndex / startAxisCellCount;
            base.JumpTo (groupIndex, alignment);
        }

        protected override void ScrollTo (int itemIndex, float duration, float alignment = 0.5f, Action onComplete = null)
        {
            var groupIndex = itemIndex / startAxisCellCount;
            base.ScrollTo (groupIndex, duration, alignment, onComplete);
        }

        protected override void ScrollTo (int itemIndex, float duration, Ease easing, float alignment = 0.5f, Action onComplete = null)
        {
            var groupIndex = itemIndex / startAxisCellCount;
            base.ScrollTo (groupIndex, duration, easing, alignment, onComplete);
        }
    }

    public abstract class ScrollGridView<TItemData> : ScrollGridView<TItemData, ScrollGridViewContext> { }
}

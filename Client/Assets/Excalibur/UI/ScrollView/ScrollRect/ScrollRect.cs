using System;
using System.Collections.Generic;
using UnityEngine;
using Excalibur.EasingCore;

namespace Excalibur
{
    [RequireComponent (typeof (Scroller))]
    public abstract class ScrollRect<TItemData, TContext> : ScrollView<TItemData, TContext>
        where TContext : class, IScrollRectContext, new ()
    {
        [SerializeField] protected float reuseCellMarginCount = 0f;

        [SerializeField] protected float paddingHead = 0f;

        [SerializeField] protected float paddingTail = 0f;

        [SerializeField] protected float spacing = 0f;

        protected abstract float CellSize { get; }

        protected virtual bool Scrollable => MaxScrollPosition > 0f;

        Scroller cachedScroller;

        protected Scroller Scroller => cachedScroller ?? (cachedScroller = GetComponent<Scroller> ());

        float ScrollLength => 1f / Mathf.Max (cellInterval, 1e-2f) - 1f;

        float ViewportLength => ScrollLength - reuseCellMarginCount * 2f;

        float PaddingHeadLength => (paddingHead - spacing * 0.5f) / (CellSize + spacing);

        float MaxScrollPosition => ItemsSource.Count
            - ScrollLength
            + reuseCellMarginCount * 2f
            + (paddingHead + paddingTail - spacing) / (CellSize + spacing);

        protected override void Initialize ()
        {
            base.Initialize ();

            Context.ScrollDirection = Scroller.ScrollDirection;
            Context.CalculateScrollSize = () =>
            {
                var interval = CellSize + spacing;
                var reuseMargin = interval * reuseCellMarginCount;
                var scrollSize = Scroller.ViewportSize + interval + reuseMargin * 2f;
                return (scrollSize, reuseMargin);
            };

            AdjustCellIntervalAndScrollOffset ();
            Scroller.OnValueChanged (OnScrollerValueChanged);
        }

        void OnScrollerValueChanged (float p)
        {
            base.UpdatePosition (ToExcaliburPosition (Scrollable ? p : 0f));

            if (Scroller.Scrollbar)
            {
                if (p > ItemsSource.Count - 1)
                {
                    ShrinkScrollbar (p - (ItemsSource.Count - 1));
                }
                else if (p < 0f)
                {
                    ShrinkScrollbar (-p);
                }
            }
        }

        void ShrinkScrollbar (float offset)
        {
            var scale = 1f - ToExcaliburPosition (offset) / (ViewportLength - PaddingHeadLength);
            UpdateScrollbarSize ( (ViewportLength - PaddingHeadLength) * scale);
        }

        protected override void Refresh ()
        {
            AdjustCellIntervalAndScrollOffset ();
            RefreshScroller ();
            base.Refresh ();
        }

        protected override void Relayout ()
        {
            AdjustCellIntervalAndScrollOffset ();
            RefreshScroller ();
            base.Relayout ();
        }

        protected void RefreshScroller ()
        {
            Scroller.Draggable = Scrollable;
            Scroller.ScrollSensitivity = ToScrollerPosition (ViewportLength - PaddingHeadLength);
            Scroller.Position = ToScrollerPosition (currentPosition);

            if (Scroller.Scrollbar)
            {
                Scroller.Scrollbar.gameObject.SetActive (Scrollable);
                UpdateScrollbarSize (ViewportLength);
            }
        }

        protected override void UpdateContents (IList<TItemData> items)
        {
            AdjustCellIntervalAndScrollOffset ();
            base.UpdateContents (items);

            Scroller.SetTotalCount (items.Count);
            RefreshScroller ();
        }

        protected new void UpdatePosition (float position)
        {
            Scroller.Position = ToScrollerPosition (position, 0.5f);
        }

        protected virtual void JumpTo (int itemIndex, float alignment = 0.5f)
        {
            Scroller.Position = ToScrollerPosition (itemIndex, alignment);
        }

        protected virtual void ScrollTo (int index, float duration, float alignment = 0.5f, Action onComplete = null)
        {
            Scroller.ScrollTo (ToScrollerPosition (index, alignment), duration, onComplete);
        }

        protected virtual void ScrollTo (int index, float duration, Ease easing, float alignment = 0.5f, Action onComplete = null)
        {
            Scroller.ScrollTo (ToScrollerPosition (index, alignment), duration, easing, onComplete);
        }

        protected void UpdateScrollbarSize (float viewportLength)
        {
            var contentLength = Mathf.Max (ItemsSource.Count + (paddingHead + paddingTail - spacing) / (CellSize + spacing), 1);
            Scroller.Scrollbar.size = Scrollable ? Mathf.Clamp01 (viewportLength / contentLength) : 1f;
        }

        protected float ToExcaliburPosition (float position)
        {
            return position / Mathf.Max (ItemsSource.Count - 1, 1) * MaxScrollPosition - PaddingHeadLength;
        }

        protected float ToScrollerPosition (float position)
        {
            return (position + PaddingHeadLength) / MaxScrollPosition * Mathf.Max (ItemsSource.Count - 1, 1);
        }

        protected float ToScrollerPosition (float position, float alignment = 0.5f)
        {
            var offset = alignment * (ScrollLength - (1f + reuseCellMarginCount * 2f))
                + (1f - alignment - 0.5f) * spacing / (CellSize + spacing);
            return ToScrollerPosition (Mathf.Clamp (position - offset, 0f, MaxScrollPosition));
        }

        protected void AdjustCellIntervalAndScrollOffset ()
        {
            var totalSize = Scroller.ViewportSize + (CellSize + spacing) * (1f + reuseCellMarginCount * 2f);
            cellInterval = (CellSize + spacing) / totalSize;
            scrollOffset = cellInterval * (1f + reuseCellMarginCount);
        }

        protected virtual void OnValidate ()
        {
            AdjustCellIntervalAndScrollOffset ();

            if (loop)
            {
                loop = false;
                Debug.LogError ("Loop is currently not supported in ScrollRect.");
            }

            if (Scroller.SnapEnabled)
            {
                Scroller.SnapEnabled = false;
                Debug.LogError ("Snap is currently not supported in ScrollRect.");
            }

            if (Scroller.MovementType == MovementType.Unrestricted)
            {
                Scroller.MovementType = MovementType.Elastic;
                Debug.LogError ("MovementType.Unrestricted is currently not supported in ScrollRect.");
            }
        }
    }

    public abstract class ScrollRect<TItemData> : ScrollRect<TItemData, ScrollRectContext> { }
}

using UnityEngine;

namespace Excalibur
{
    public abstract class ScrollRectCell<TItemData, TContext> : ScrollCell<TItemData, TContext>
        where TContext : class, IScrollRectContext, new ()
    {
        public override void UpdatePosition (float position)
        {
            var (scrollSize, reuseMargin) = Context.CalculateScrollSize ();

            var normalizedPosition = (Mathf.Lerp (0f, scrollSize, position) - reuseMargin) / (scrollSize - reuseMargin * 2f);

            var start = 0.5f * scrollSize;
            var end = -start;

            UpdatePosition (normalizedPosition, Mathf.Lerp (start, end, position));
        }

        protected virtual void UpdatePosition (float normalizedPosition, float localPosition)
        {
            transform.localPosition = Context.ScrollDirection == ScrollDirection.Horizontal
                ? new Vector2 (-localPosition, 0)
                : new Vector2 (0, localPosition);
        }
    }

    public abstract class ScrollRectCell<TItemData> : ScrollRectCell<TItemData, ScrollRectContext>
    {
        public sealed override void SetContext (ScrollRectContext context) => base.SetContext (context);
    }
}

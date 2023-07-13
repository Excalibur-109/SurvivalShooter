using UnityEngine;

namespace Excalibur
{
    public abstract class ScrollGridViewCell<TItemData, TContext> : ScrollRectCell<TItemData, TContext>
        where TContext : class, IScrollGridViewContext, new ()
    {
        protected override void UpdatePosition (float normalizedPosition, float localPosition)
        {
            var cellSize = Context.GetCellSize ();
            var spacing = Context.GetStartAxisSpacing ();
            var groupCount = Context.GetGroupCount ();

            var indexInGroup = Index % groupCount;
            var positionInGroup = (cellSize + spacing) * (indexInGroup - (groupCount - 1) * 0.5f);

            transform.localPosition = Context.ScrollDirection == ScrollDirection.Horizontal
                ? new Vector2 (-localPosition, -positionInGroup)
                : new Vector2 (positionInGroup, localPosition);
        }
    }

    public abstract class ScrollGridViewCell<TItemData> : ScrollGridViewCell<TItemData, ScrollGridViewContext>
    {
        public sealed override void SetContext (ScrollGridViewContext context) => base.SetContext (context);
    }
}

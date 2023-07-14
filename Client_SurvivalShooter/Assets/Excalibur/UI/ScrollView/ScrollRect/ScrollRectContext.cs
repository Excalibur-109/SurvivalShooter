using System;

namespace Excalibur
{
    public class ScrollRectContext : IScrollRectContext
    {
        ScrollDirection IScrollRectContext.ScrollDirection { get; set; }
        Func< (float ScrollSize, float ReuseMargin)> IScrollRectContext.CalculateScrollSize { get; set; }
    }
}

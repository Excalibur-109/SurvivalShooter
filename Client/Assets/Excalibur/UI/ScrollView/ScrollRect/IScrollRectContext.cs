using System;

namespace Excalibur
{
    public interface IScrollRectContext
    {
        ScrollDirection ScrollDirection { get; set; }
        Func< (float ScrollSize, float ReuseMargin)> CalculateScrollSize { get; set; }
    }
}

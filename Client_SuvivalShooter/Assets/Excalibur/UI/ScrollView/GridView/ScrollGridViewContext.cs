using System;
using UnityEngine;

namespace Excalibur
{
    public class ScrollGridViewContext : IScrollGridViewContext
    {
        ScrollDirection IScrollRectContext.ScrollDirection { get; set; }
        Func< (float ScrollSize, float ReuseMargin)> IScrollRectContext.CalculateScrollSize { get; set; }
        GameObject IScrollCellGroupContext.CellTemplate { get; set; }
        Func<int> IScrollCellGroupContext.GetGroupCount { get; set; }
        Func<float> IScrollGridViewContext.GetStartAxisSpacing { get; set; }
        Func<float> IScrollGridViewContext.GetCellSize { get; set; }
    }
}

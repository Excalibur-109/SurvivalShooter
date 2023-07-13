using System;

namespace Excalibur
{
    public interface IScrollGridViewContext : IScrollRectContext, IScrollCellGroupContext
    {
        Func<float> GetStartAxisSpacing { get; set; }
        Func<float> GetCellSize { get; set ; }
    }
}

using System;
using UnityEngine;

namespace Excalibur
{
    public interface IScrollCellGroupContext
    {
        GameObject CellTemplate { get; set; }
        Func<int> GetGroupCount { get; set; }
    }
}

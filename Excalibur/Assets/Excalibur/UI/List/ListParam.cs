using System;
using UnityEngine;

namespace Excalibur
{
    public enum ListType { Grid, Page, Content }
    public enum ScrolllAxis { Horizontal, Vertical, Arbitrary }
    public enum ScrollType { Nothing, DragOnly, ScrollOnly, Everything }
    public enum StartCorner 
    { UpperLeft, UpperCenter, UpperRight, MiddleLeft, MiddleCenter, MiddleRight, LowerLeft, LowerCenter, LowerRight }

    [Serializable]
    public class ListParam
    {
        public ListType listType = ListType.Grid;
        public ScrolllAxis scrolllAxis = ScrolllAxis.Vertical;
        public StartCorner startCorner = StartCorner.UpperLeft;
        public ScrollType scrollType = ScrollType.Everything;
        public RectOffset padding;
        public Vector2 spacing;
        public bool defaultSelected, isLoop, allowMultiSelect;
        public bool fixedRow, fixedColumn;
        public int rowCount, columnCount;
        public float exceedOffsetFactor = 0.3f;
        public float
            dampTime = 0.75f, endDragDampFactor = 0.2f, elasticFactor = 0.1f,
            scrollAccelerationSpeed = 0.8f, scrollMaxSpeed = 10f;
    }
}
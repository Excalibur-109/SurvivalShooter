using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/*
 * ༭ǲģʽ£itemitemݻʽ̶
 * /ʼվ
 * /ĬϴϽǿʼУݻ򣬴ֱӴңϵ£ˮƽϵ£
 * ˮƽֱӦϳMaskڵitem⣬itemбλñ仯
 * ScrolllAxis.Horizontal       ˮƽ򻬶
 * ScrolllAxis.Vertical         ֱ򻬶
 * ScrolllAxis.Arbitrary        һcontent
 * 
 * Vertical     ѡ̶̶͸MaskԶ
 * Horizontal   ѡ̶̶͸MaskԶ
 * 
 * ScrollType.Nothing    ֻбܼitemжлȫMaskitemЧ
 */

namespace Excalibur
{
    public delegate void OnSelected (BaseData selectedData);

    [DisallowMultipleComponent, RequireComponent (typeof (RectTransform), typeof (Mask))]
    public class ExcaliburList : UIBehaviour,
        IInitializePotentialDragHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IScrollHandler,
        ISubject
    {
        private enum MoveType { Nothing, Dragging, DragDamp, Scrolling, ScrollDamp, Elastic, ScrollTo, ScrollBar }

        /// бȫӴ 1 /
        private int FULL_OUT_BOUND_COUNT = 2;

        [SerializeField] private BaseListItem _prefab;
        [SerializeField] private ListParam _params;
        [SerializeField] private Scrollbar
            _horizontalBar, _verticalBar;

        public OnSelected onSelected;

        private RectTransform _content;
        public RectTransform Content
        {
            get 
            {
                if (scrolllAxis == ScrolllAxis.Arbitrary && !_content)
                {
                    _content = new GameObject ("Content").AddComponent<RectTransform> ();
                    _content.pivot = Vector2.one * 0.5f;
                    _content.sizeDelta = rectTrans.sizeDelta;
                    _content.anchorMin = Vector2.up;
                    _content.anchorMax = Vector2.up;
                    _content.anchoredPosition = new Vector2 (rectTrans.sizeDelta.x * 0.5f, -rectTrans.sizeDelta.y * 0.5f);
                }
                return _content;
            }
        }

        #region param properties
        public ListType listType { get => _params.listType; set => _params.listType = value; }
        public ScrolllAxis scrolllAxis { get => _params.scrolllAxis; set => _params.scrolllAxis = value; }
        public StartCorner startCorner { get => _params.startCorner; set => _params.startCorner = value; }
        public ScrollType scrollType { get => _params.scrollType; set => _params.scrollType = value; }
        public RectOffset padding { get => _params.padding; set => _params.padding = value; }
        public Vector2 spacing { get => _params.spacing; set => _params.spacing = value; }
        public bool fixedRow { get => _params.fixedRow; set => _params.fixedRow = value; }
        public bool fixedColumn { get => _params.fixedColumn; set => _params.fixedColumn = value; }
        public int rowCount { get => _params.rowCount; set => _params.rowCount = value; }
        public int columnCount { get => _params.columnCount; set => _params.columnCount = value; }
        public bool defaultSelected { get => _params.defaultSelected; set => _params.defaultSelected = value; }
        public bool isLoop { get => _params.isLoop; set => _params.isLoop = value; }
        public bool allowMultiSelect { get => _params.allowMultiSelect; set => _params.allowMultiSelect = value; }
        public float exceedOffsetFactor { get => _params.exceedOffsetFactor; set => _params.exceedOffsetFactor = value; }
        public float dampTime { get => _params.dampTime; set => _params.dampTime = value; }
        public float endDragDampFactor { get => _params.endDragDampFactor; set => _params.endDragDampFactor = value; }
        public float elasticFactor { get => _params.elasticFactor; set => _params.elasticFactor = value; }
        public float scrollAccelerationSpeed { get => _params.scrollAccelerationSpeed; set => _params.scrollAccelerationSpeed = value; }
        public float scrollMaxSpeed { get => _params.scrollMaxSpeed; set => _params.scrollMaxSpeed = value; }
        #endregion

        private RectTransform _rectTrans;
        public RectTransform rectTrans { get { if (!_rectTrans) { _rectTrans = (RectTransform)transform; } return _rectTrans; } }

        private List<BaseListItem>
            _items = new List<BaseListItem> ();
        private List<BaseData> 
            _sources = new List<BaseData> ();
        private BaseData _interactData;
        /// <summary> ѡ /// </summary>
        private List<BaseData>
            _selections = new List<BaseData> ();
        private List<IObserver>
            _observers = new List<IObserver> ();
        private float
            _deltaTime, _dampTimer,
            _scrollSpeed, _dampScrollSpeed, _preScrollDelta;
        private Vector2
            _scrollPos, _maxScrollRange, _scrollExceedOffset, _maxScrollOffset,
            _velocity, _endDragVelocity, _prePointerDragPos, _pointerDragPos,
            _normalizedPosition, _barSetNormalPositon, _barSize, _scrollToBegin, _scrollToTarget;
        private MoveType 
            _moveType = MoveType.Nothing;
        private bool updateBarValue;
        private float edgeX => rectTrans.sizeDelta.x;
        private float edgeY => -rectTrans.sizeDelta.y;
        private float paddingX => padding.left - padding.right;
        private float paddingY => padding.bottom - padding.top;
        private bool isScrollEnable => (scrollType & ScrollType.Everything) != 0 && isActiveAndEnabled && _sources.Count > 0;
        public Vector2 maxScrollRange => _maxScrollRange;
        private Vector2 maxScrollOffset
        {
            get 
            {
                if (_maxScrollOffset == Vector2.zero)
                {
                    _maxScrollOffset = rectTrans.sizeDelta * exceedOffsetFactor;
                }
                return _maxScrollOffset;
            }
        }

        public BaseData this[int index]
        {
            get
            {
                return IsIndexValidate (index) ? _sources[index] : default (BaseData);
            }
        }

        public float horizontalNormalizedPosition
        {
            get => _normalizedPosition.x;
            set => SetHorizontalNormalizedPostion (value);
        }

        public float verticalNormalizedPosition
        {
            get => _normalizedPosition.y;
            set => SetVerticalNormalizedPostion (value);
        }

        public Vector2 normalizedPosition
        {
            get => _normalizedPosition;
            set { horizontalNormalizedPosition = value.x; verticalNormalizedPosition = value.y; }
        }

        protected override void Awake ()
        {
            if (_prefab)
            {
                _prefab.rectTrans.pivot = Vector2.one * 0.5f;
                _prefab.rectTrans.anchorMin = Vector2.up;
                _prefab.rectTrans.anchorMax = Vector2.up;
            }
            AttachChilds ();
        }

        protected override void Start ()
        {
            if (_horizontalBar)
            {
                _horizontalBar.onValueChanged.AddListener (OnHorizontalBarValueChange);
            }

            if (_verticalBar)
            {
                _verticalBar.onValueChanged.AddListener (OnVerticalBarValueChange);
            }
            SetBarState ();
        }

        #region Item Data Operate Methods

        /// <summary>
        /// Ѿݵɾݣıڵʾλãrelayout = false
        /// </summary>
        public void SurveilDatas<T> (IEnumerable<T> dataSrc, bool relayout = true) where T : BaseData
        {
            if (!_prefab) { Debug.Log ("бԤΪ"); }

            if (listType == ListType.Content || scrolllAxis == ScrolllAxis.Arbitrary)
            {
                Debug.Log ("бΪContentҲ⻬");
                return;
            }

            if (_sources.Count > 0) { _sources.Clear (); }
            if (dataSrc != null) { foreach (T data in dataSrc) { _sources.Add (data); } }
            if (relayout)
            {
                if (_selections.Count > 0) { _selections.Clear (); }
                LayoutItems ();
                for (int i = 0; i < _items.Count; ++i)
                {
                    _items[i].SetIndex (i);
                }
            }
            else
            {
                for (int i = 0; i < _items.Count; ++i)
                {
                    _items[i].SetDirty ();
                }
            }
            if (defaultSelected && relayout)
            {
                SetSelected (0);
            }
            CalcScrollRange ();
            if (relayout)
            {
                _scrollPos = Vector2.zero;
                updateBarValue = true;
                UpdateNormalizedPositions ();
            }
        }

        public bool IsIndexValidate (int index)
        {
            if (_sources.Count == 0) { return false; }
            return index >= 0 && index < _sources.Count;
        }

        private void InvokeSelectedEvent ()
        {
            onSelected?.Invoke (_interactData);
        }

        public void SetSelected (int index)
        {
            if (!IsIndexValidate (index)) { return; }

            BaseData current = _sources[index];
            if (_selections.Contains (current))
            {
                if (!allowMultiSelect)
                {
                    if (_selections.Count > 1) { _selections.Clear (); _selections.Add (null); }
                    else { _selections[0] = null; }
                    _interactData = null;
                    InvokeSelectedEvent ();
                }
                else
                {
                    if (_selections.Count == 1) { _selections[0] = null; }
                    else { _selections.Remove (current); }
                    
                    if (_interactData == current)
                    {
                        if (_selections.Count > 0)
                        {
                            _interactData = _selections[_selections.Count - 1];
                        }
                        else
                        {
                            _interactData = null;
                        }
                        InvokeSelectedEvent ();
                    }
                }
            }
            else
            {
                if (!allowMultiSelect)
                {
                    if (_selections.Count != 1)
                    {
                        if (_selections.Count > 1) { _selections.Clear (); }
                        _selections.Add (null);
                    }

                    _selections[0] = current;
                }
                else
                {
                    _selections.Add (current);
                }
                _interactData = current;
                InvokeSelectedEvent ();
            }
            Notify ();
        }

        public bool IsSelectedIndex (BaseData data)
        {
            if (data == null) { return false; }

            if (_selections.Count > 0)
            {
                return _selections.Contains (data);
            }

            return false;
        }

        public void OnRefresh (BaseData data)
        {
            if (_interactData == data) { InvokeSelectedEvent (); }
            for (int i = 0; i < _sources.Count; ++i)
            {
                if (ReferenceEquals (_sources[i], data))
                {
                    _sources[i] = data;
                    for (int j = 0; j < _items.Count; ++j)
                    {
                        if (_items[j].dataIndex == i)
                        {
                            _items[j].SetDirty ();
                            break;
                        }
                    }
                    break;
                }
            }
        }

        /// ӻ޸
        public void OnRefresh (IEnumerable<BaseData> datas)
        {
            foreach (var data in datas)
            {
                OnRefresh (data);
            }
        }

        /// <summary> Ƴ /// </summary>
        public void OnRemove (BaseData data)
        {
            if (_selections.Count > 0 && _selections.Contains (data))
            {
                _selections.Remove (data);
                if (_interactData == data) 
                { _interactData = _selections.Count > 0 ? _selections[_selections.Count - 1] : null; InvokeSelectedEvent (); }
            }
            if (_sources.Count > 0 && _sources.Contains (data))
            {
                _sources.Remove (data);
            }
            for (int i = 0; i < _items.Count; ++i)
            {
                _items[i].SetDirty ();
            }
            CalcScrollRange ();
            TransitionMove (MoveType.Elastic);
        }

        /// <summary> Ƴ /// </summary>
        public void OnRemove (IEnumerable<BaseData> datas)
        {
            foreach (BaseData data in datas)
            {
                if (_selections.Count > 0 && _selections.Contains (data))
                {
                    _selections.Remove (data);
                }
                if (_sources.Count > 0 && _sources.Contains (data))
                {
                    _sources.Remove (data);
                }
            }
            if (_selections.Count > 0)
            {
                if (!_selections.Contains (_interactData))
                { _interactData = _selections[_selections.Count - 1]; InvokeSelectedEvent (); }
            }
            else { _interactData = null; InvokeSelectedEvent (); }
            for (int i = 0; i < _items.Count; ++i)
            {
                _items[i].SetDirty ();
            }
            CalcScrollRange ();
            TransitionMove (MoveType.Elastic);
        }

        public T GetInteractData<T> () where T : BaseData
        {
            return _interactData == null ? null : (T)_interactData;
        }

        public List<T> GetSelections<T> () where T : BaseData
        {
            List<T> selections = new List<T> ();
            for (int i = 0; i < _selections.Count; ++i)
            {
                if (_selections[i] != null)
                {
                    selections.Add ( (T)_selections[i]);
                }
            }
            return selections;
        }

        public void AttachObserver (IObserver observer)
        {
            _observers.Add (observer);
        }

        public void DetachObserver (IObserver observer)
        {
            _observers.Remove (observer);
        }

        public void Notify ()
        {
            for (int i = 0; i < _observers.Count; ++i)
            {
                _observers[i].Execute ();
            }
        }

        public void UpdateItemStateOnScroll (Vector2 velocity)
        {
            Vector2 pos;
            int index;
            for (int i = 0; i < _items.Count; ++i)
            {
                BaseListItem item = _items[i];
                index = item.dataIndex;
                pos = item.anchoredPos;
                switch (scrolllAxis)
                {
                    case ScrolllAxis.Horizontal:
                        {
                            if (velocity.x > 0f)
                            {
                                if (item.edgeXLeft > edgeX)
                                {
                                    pos.x -= (item.width + spacing.x) * columnCount;
                                    index -= _items.Count;
                                    if (isLoop)
                                    {
                                        index %= _sources.Count;
                                        index = index < 0 ? index + _sources.Count : index;
                                    }
                                }
                            }
                            else if (velocity.x < 0f)
                            {
                                if (item.edgeXRight < 0f)
                                {
                                    pos.x += (item.width + spacing.x) * columnCount;
                                    index += _items.Count;
                                    if (isLoop)
                                    {
                                        index %= _sources.Count;
                                        index = index < 0 ? index + _sources.Count : index;
                                    }
                                }
                            }
                        }
                        break;
                    case ScrolllAxis.Vertical:
                        {
                            if (velocity.y < 0f)
                            {
                                if (item.edgeYTop < edgeY)
                                {
                                    int straddle = Mathf.Max (Mathf.CeilToInt ( (-velocity.y) / (item.height + spacing.y)), rowCount);
                                    pos.y += (item.height + spacing.x) * rowCount;
                                    if (straddle != rowCount) { pos.y += -velocity.y; }
                                    index -= straddle * columnCount; // _items.Count;
                                    if (isLoop)
                                    {
                                        index %= _sources.Count;
                                        index = index < 0 ? index + _sources.Count : index;
                                    }
                                }
                            }
                            else if (velocity.y > 0f)
                            {
                                if (item.edgeYBottom > 0f)
                                {
                                    int straddle = Mathf.Max (Mathf.CeilToInt (velocity.y / (item.height + spacing.y)), rowCount);
                                    pos.y -= (item.height + spacing.x) * rowCount;
                                    if (straddle != rowCount) { pos.y += -velocity.y; }
                                    index += straddle * columnCount; //_items.Count;
                                    if (isLoop)
                                    {
                                        index %= _sources.Count;
                                        index = index < 0 ? index + _sources.Count : index;
                                    }
                                }
                            }
                        }
                        break;
                }
                if (index != item.dataIndex) { item.SetIndex (index); }
                if (item.anchoredPos != pos) { item.anchoredPos = pos; }
            }
        }

        #endregion

        private void LateUpdate ()
        {
            CalcVelocity ();
        }

        #region Scroll Methods

        private void CalcVelocity ()
        {
            if (!isScrollEnable || _moveType == MoveType.Nothing) { return; }

            _deltaTime = Time.unscaledDeltaTime;

            switch (_moveType)
            {
                case MoveType.Dragging:
                    {
                        if (_prePointerDragPos != _pointerDragPos)
                        {
                            ScrollItems (_velocity);
                            _prePointerDragPos = _pointerDragPos;
                        }
                    }
                    break;
                case MoveType.DragDamp:
                    {
                        if (_endDragVelocity != Vector2.zero)
                        {
                            _dampTimer += _deltaTime;
                            _velocity = Vector2.Lerp (_endDragVelocity, Vector2.zero, _dampTimer / dampTime) * endDragDampFactor;
                            ScrollItems (_velocity);
                            if (_dampTimer > dampTime) { TransitionMove (MoveType.Nothing); }
                            if (CheckElastic (_scrollPos)) { _dampTimer = 0f; TransitionMove (MoveType.Elastic); }
                        }
                        else { TransitionMove (MoveType.Nothing); }
                    }
                    break;
                case MoveType.Scrolling:
                    {
                        ScrollItems (_velocity);
                        _dampTimer = 0f;
                        TransitionMove (MoveType.ScrollDamp);
                    }
                    break;
                case MoveType.ScrollDamp:
                    {
                        _dampTimer += _deltaTime;
                        _dampScrollSpeed = Mathf.Lerp (_scrollSpeed, 0f, _dampTimer / dampTime);
                        _velocity = Vector2.one * _dampScrollSpeed;
                        ScrollItems (_velocity);
                        if (CheckElastic (_scrollPos)) { _dampTimer = 0f; TransitionMove (MoveType.Elastic); }
                        else if (_dampTimer > dampTime)
                        {
                            _scrollSpeed = 0f;
                            TransitionMove (MoveType.Nothing);
                        }
                    }
                    break;
                case MoveType.Elastic:
                    {
                        if (CheckElastic (_scrollPos))
                        {
                            Vector2 target = Vector2.zero;
                            if (scrolllAxis != ScrolllAxis.Vertical && -_scrollPos.x > maxScrollRange.x) { target.x = -maxScrollRange.x; }
                            if (scrolllAxis != ScrolllAxis.Horizontal && _scrollPos.y > maxScrollRange.y) { target.y = maxScrollRange.y; }

                            _dampTimer += _deltaTime;
                            //Vector2 ret = Vector2.SmoothDamp (_scrollPos, target, ref _velocity, elasticFactor, float.MaxValue, _deltaTime);
                            Vector2 ret = Vector2.Lerp (_scrollPos, target, _dampTimer / dampTime);
                            Vector2 v = ret - _scrollPos;
                            ScrollItems (v);
                        }
                        else
                        {
                            TransitionMove (MoveType.Nothing);
                        }
                    }
                    break;
                case MoveType.ScrollTo:
                    {
                        //Vector2 ret = Vector2.SmoothDamp (_scrollPos, _scrollToTarget, ref _velocity, elasticFactor, float.MaxValue, _deltaTime);
                        if (Vector2.Distance (_scrollPos, _scrollToTarget) < 0.1f)
                        {
                            ScrollItems (_scrollToTarget - _scrollPos);
                            TransitionMove (MoveType.Nothing);
                            return;
                        }
                        _dampTimer += _deltaTime;
                        Vector2 ret = Vector2.Lerp (_scrollPos, _scrollToTarget, _dampTimer / dampTime);
                        Vector2 v = ret - _scrollPos;
                        if (_dampTimer > dampTime) { TransitionMove (MoveType.Nothing); }
                        ScrollItems (v);
                    }
                    break;
                case MoveType.ScrollBar:
                    {
                        Vector2 delta = _barSetNormalPositon - normalizedPosition;
                        _velocity = new Vector2 (-maxScrollRange.x * delta.x, maxScrollRange.y * delta.y);
                        ScrollItems (_velocity);
                        _normalizedPosition = _barSetNormalPositon;
                        TransitionMove (MoveType.Nothing);
                    }
                    break;
            }
        }

        private bool CheckExeceedHorizontal (Vector2 velocity)
        {
            if (scrolllAxis == ScrolllAxis.Vertical) { return false; }
            Vector2 scrollPos = _scrollPos + velocity;
            _scrollExceedOffset.x = 0f;
            if (scrollPos.x < -maxScrollRange.x)
            {
                // 󻬶
                _scrollExceedOffset.x = scrollPos.x + maxScrollRange.x;
                return true;
            }
            else if (scrollPos.x > 0f)
            {
                // һ
                _scrollExceedOffset.x = scrollPos.x;
                return true;
            }
            return false;
        }


        private bool CheckExeceedVertical (Vector2 velocity)
        {
            if (scrolllAxis == ScrolllAxis.Horizontal) { return false; }
            Vector2 scrollPos = _scrollPos + velocity;
            _scrollExceedOffset.y = 0f;
            if (scrollPos.y > maxScrollRange.y)
            {
                _scrollExceedOffset.y = scrollPos.y - maxScrollRange.y;
                return true;
            }
            else if (scrollPos.y < 0f)
            {
                _scrollExceedOffset.y = scrollPos.y;
                return true;
            }

            return false;
        }

        private void TransitionMove (MoveType moveType)
        {
            if (_moveType != moveType)
            {
                _moveType = moveType;
                updateBarValue = _moveType != MoveType.ScrollBar;
            }
        }

        public void OnInitializePotentialDrag (PointerEventData eventData)
        {
            if (!isScrollEnable || eventData.button != PointerEventData.InputButton.Left) { return; }

            _velocity = Vector2.zero;
            _scrollExceedOffset = Vector2.zero;
            TransitionMove (MoveType.Nothing);
            _scrollSpeed = 0f;
            _preScrollDelta = 0;
        }

        public void OnBeginDrag (PointerEventData eventData)
        {
            if (scrollType == ScrollType.ScrollOnly) { return; }
            if (!isScrollEnable || eventData.button != PointerEventData.InputButton.Left) { return; }
            TransitionMove (MoveType.Dragging);
            _prePointerDragPos = Vector2.zero;
            RectTransformUtility.
                ScreenPointToLocalPointInRectangle (rectTrans, eventData.position, eventData.pressEventCamera, out _prePointerDragPos);
        }

        public void OnDrag (PointerEventData eventData)
        {
            if (scrollType == ScrollType.ScrollOnly) { return; }
            if (!isScrollEnable || eventData.button != PointerEventData.InputButton.Left) { return; }

            _pointerDragPos = Vector2.zero;
            if (RectTransformUtility.
                ScreenPointToLocalPointInRectangle (rectTrans, eventData.position, eventData.pressEventCamera, out _pointerDragPos))
            {
                _velocity = _pointerDragPos - _prePointerDragPos;
            }
        }

        public void OnEndDrag (PointerEventData eventData)
        {
            if (scrollType == ScrollType.ScrollOnly) { return; }
            if (!isScrollEnable || eventData.button != PointerEventData.InputButton.Left) { return; }

            _pointerDragPos = Vector2.zero;
            if (RectTransformUtility.
                ScreenPointToLocalPointInRectangle (rectTrans, eventData.position, eventData.pressEventCamera, out _pointerDragPos))
            {
                _endDragVelocity = _pointerDragPos - _prePointerDragPos;
            }
            _dampTimer = 0f;
            TransitionMove (CheckElastic (_scrollPos) ? MoveType.Elastic : MoveType.DragDamp);
        }

        public void OnScroll (PointerEventData eventData)
        {
            if (scrollType == ScrollType.DragOnly) { return; }
            if (!isScrollEnable || !eventData.IsScrolling ()) { return; }

            float delta = eventData.scrollDelta.y;
            switch (scrolllAxis)
            {
                case ScrolllAxis.Vertical:
                    delta *= -1;
                    break;
            }
            if (_preScrollDelta != delta)
            {
                _scrollSpeed = 0f;
            }
            _dampTimer = 0.01f;
            _scrollSpeed += delta * scrollAccelerationSpeed;
            _scrollSpeed = Mathf.Clamp (_scrollSpeed, -scrollMaxSpeed, scrollMaxSpeed);
            _velocity = Vector2.one * _scrollSpeed;
            Debug.Log (_scrollSpeed);
            _preScrollDelta = delta;
            TransitionMove (MoveType.Scrolling);
        }

        private void ScrollItems (Vector2 velocity)
        {
            switch (scrolllAxis)
            {
                case ScrolllAxis.Horizontal:
                    velocity.y = 0f;
                    break;
                case ScrolllAxis.Vertical:
                    velocity.x = 0f;
                    break;
            }

            if (!isLoop)
            {
                velocity = CheckExceedRangeOnScroll (velocity);
            }
            _scrollPos += velocity;
            UpdateBarSizeOnScroll ();
            UpdateNormalizedPositions ();

            float v;
            Vector2 pos;
            switch (listType)
            {
                case ListType.Grid:
                case ListType.Page:
                    {
                        if (scrolllAxis != ScrolllAxis.Arbitrary)
                        {
                            BaseListItem temp;
                            for (int i = 0; i < 2; ++i)
                            {
                                v = velocity[i];
                                for (int j = 0; j < _items.Count; ++j)
                                {
                                    if (v != 0f)
                                    {
                                        temp = _items[j];
                                        pos = temp.anchoredPos;
                                        pos[i] += v;
                                        temp.anchoredPos = pos;
                                    }
                                }
                            }

                            UpdateItemStateOnScroll (velocity);
                        }
                    }
                    break;
                case ListType.Content:
                    {
                        pos = Content.anchoredPosition;
                        pos += velocity;
                        Content.anchoredPosition = pos;
                    }
                    break;
            }
        }

        private Vector2 CheckExceedRangeOnScroll (Vector2 velocity)
        {
            Vector2 nextPos = _scrollPos + velocity;
            float offset;
            if (CheckExeceedHorizontal (nextPos))
            {
                offset = nextPos.x > 0f
                    ? Mathf.Clamp01 (nextPos.x / maxScrollOffset.x) : Mathf.Clamp01 (- (nextPos.x + maxScrollRange.x) / maxScrollOffset.x);
                velocity.x *= (1f - offset) / exceedOffsetFactor;
            }

            if (CheckExeceedVertical (nextPos))
            {
                offset = nextPos.y < 0f
                    ? Mathf.Clamp01 (-nextPos.y / maxScrollOffset.y) : Mathf.Clamp01 ( (nextPos.y - maxScrollRange.y) / maxScrollOffset.y);
                velocity.y *= (1f - offset) / exceedOffsetFactor;
            }
            return velocity;
        }

        private bool CheckElastic (Vector2 scrollPos)
        {
            if (CheckElasticHorizontal (scrollPos)) { return true; }
            if (CheckElasticVertical (scrollPos)) { return true; }
            return false;
        }

        private bool CheckElasticHorizontal (Vector2 scrollPos)
        {
            if (scrolllAxis != ScrolllAxis.Vertical)
            {
                if (scrollPos.x > 0f || scrollPos.x < -maxScrollRange.x) { return true; }
            }
            return false;
        }

        private bool CheckElasticVertical (Vector2 scrollPos)
        {
            if (scrolllAxis != ScrolllAxis.Horizontal)
            {
                if (scrollPos.y < 0f || scrollPos.y > maxScrollRange.y) { return true; }
            }
            return false;
        }

        /// <summary> ɻΪӴĲ /// </summary>
        private void CalcScrollRange ()
        {
            _maxScrollRange = Vector2.zero;
            if (listType != ListType.Content)
            {
                if (_prefab == null) { return; }
                switch (scrolllAxis)
                {
                    case ScrolllAxis.Horizontal:
                        _maxScrollRange.x = (_prefab.width + spacing.x) * Mathf.CeilToInt ( (float)_sources.Count / rowCount) - spacing.x - rectTrans.sizeDelta.x + padding.left;
                        _maxScrollRange.x = Mathf.Max (_maxScrollRange.x, 0f);
                        break;
                    case ScrolllAxis.Vertical:
                        _maxScrollRange.y = (_prefab.height + spacing.y) * Mathf.CeilToInt ( (float)_sources.Count / columnCount) - spacing.y - rectTrans.sizeDelta.y + padding.top;
                        _maxScrollRange.y = Mathf.Max (_maxScrollRange.y, 0f);
                        break;
                }
            }
            else
            {
                _maxScrollRange = Content.sizeDelta;
            }
            SetBarState ();
        }

        private void SetBarState ()
        {
            _barSize = Vector2.one;
            _barSize.x = (rectTrans.sizeDelta.x - padding.left) / (maxScrollRange.x + rectTrans.sizeDelta.x - padding.left);
            _barSize.y = (rectTrans.sizeDelta.y - padding.top) / (maxScrollRange.y + rectTrans.sizeDelta.y - padding.top);
            if (_horizontalBar)
            {
                if (maxScrollRange.x > 0f && scrolllAxis != ScrolllAxis.Vertical)
                {
                    _horizontalBar.size = _barSize.x;
                    if (!_horizontalBar.gameObject.activeSelf) { _horizontalBar.gameObject.SetActive (true); }
                }
                else if (_horizontalBar.gameObject.activeSelf) { _horizontalBar.gameObject.SetActive (false); }
            }
            if (_verticalBar)
            {
                if (maxScrollRange.y > 0f && scrolllAxis != ScrolllAxis.Horizontal)
                {
                    _verticalBar.size = _barSize.y;
                    if (!_verticalBar.gameObject.activeSelf) { _verticalBar.gameObject.SetActive (true); }
                }
                else if (_verticalBar.gameObject.activeSelf) { _verticalBar.gameObject.SetActive (false); }
            }
        }

        private void UpdateBarSizeOnScroll ()
        {
            if (_moveType != MoveType.ScrollBar)
            {
                float delta = 0f;
                if (_horizontalBar)
                {
                    if (CheckElasticHorizontal (_scrollPos))
                    {
                        if (maxScrollRange.x > 0f && (_scrollPos.x > 0f || -_scrollPos.x < maxScrollRange.x))
                        {
                            delta = _scrollPos.x > 0f ? _scrollPos.x : - (_scrollPos.x + maxScrollRange.x);
                        }
                        _horizontalBar.size = (rectTrans.sizeDelta.x - padding.left) / (maxScrollRange.x + rectTrans.sizeDelta.x - padding.left + delta);
                    }
                    else if (_horizontalBar.size != _barSize.x) { _verticalBar.size = _barSize.x; }
                }
                delta = 0f;
                if (_verticalBar)
                {
                    if (CheckElasticVertical (_scrollPos))
                    {
                        if (maxScrollRange.y > 0f && (_scrollPos.y < 0f || _scrollPos.y > maxScrollRange.y))
                        {
                            delta = _scrollPos.y < 0f ? -_scrollPos.y : _scrollPos.y - maxScrollRange.y;
                        }
                        _verticalBar.size = (rectTrans.sizeDelta.y - padding.top) / (maxScrollRange.y + rectTrans.sizeDelta.y - padding.top + delta);
                    }
                    else if (_verticalBar.size != _barSize.y) { _verticalBar.size = _barSize.y; }
                }
            }
        }

        private void UpdateNormalizedPositions ()
        {
            horizontalNormalizedPosition = maxScrollRange.x > 0f ? _scrollPos.x / maxScrollRange.x : 0f;
            verticalNormalizedPosition = maxScrollRange.y > 0f ? _scrollPos.y / maxScrollRange.y : 0f;
        }

        public void ScrollTo (int dataIndex) { ScrollTo (dataIndex, true); }
        public void ScrollTo (int dataIndex, bool animated) { }
        /// <param name="axis"><=0ˮƽ>=1ֱ</param>
        public void ScrollTo (float position, int axis) { ScrollTo (position, axis, false); }
        /// <param name="axis"><=0ˮƽ>=1ֱ</param>
        public void ScrollTo (float position, int axis, bool animated)
        {
            if (axis < 0) { axis = 0; }
            else if (axis > 1) { axis = 1; }
            position = Mathf.Clamp (position, 0f, maxScrollRange[axis]);
            if (axis == 0) { position = -position; }
            _scrollToTarget[axis] = position;
            if (animated)
            {
                _scrollToBegin = _scrollPos;
                _dampTimer = 0f;
                TransitionMove (MoveType.ScrollTo);
            }
            else
            {
                _scrollPos[axis] = position;
            }
        }
        public void ScrollTo (Vector2 position) { ScrollTo (position, false); }
        public void ScrollTo (Vector2 position, bool animated)
        {
            for (int i = 0; i < 2; ++i)
            {
                ScrollTo (position[i], i, animated);
            }
        }

        #endregion

        public void CreateItems ()
        {
            if (!_prefab)
            {
                throw new NullReferenceException ("List Prefab Ϊ");
            }

            if (listType == ListType.Grid && scrolllAxis != ScrolllAxis.Arbitrary)
            {
                int row = CalcRow (), column = CalcColumn ();
                int count = row * column;
                Debug.Log ($"row: {row}, column: {column}");
                for (int i = 0; i < count; ++i)
                {
                    BaseListItem item = Instantiate (_prefab, transform);
                    item.name = _prefab.name;
                    _items.Add (item);
                }
            }
        }

        public void LayoutItems ()
        {
            if (listType == ListType.Grid && scrolllAxis != ScrolllAxis.Arbitrary && _items.Count > 0)
            {
                Vector2 startPos = CalcStartPos ();
                Vector2[] poses = new Vector2[_items.Count];
                int index = 0, row = 0, column = 0, maxRow = CalcRow (), maxColumn = CalcColumn ();
                float x = 0f, y = 0f;
                while (index < _items.Count)
                {
                    switch (startCorner)
                    {
                        case StartCorner.UpperLeft:
                            x = startPos.x + (_prefab.width + spacing.x) * column;
                            y = startPos.y - (_prefab.height + spacing.y) * row;
                            break;
                        case StartCorner.UpperCenter:
                            break;
                        case StartCorner.UpperRight:
                            break;
                        case StartCorner.MiddleLeft:
                            break;
                        case StartCorner.MiddleCenter:
                            break;
                        case StartCorner.MiddleRight:
                            break;
                        case StartCorner.LowerLeft:
                            break;
                        case StartCorner.LowerCenter:
                            break;
                        case StartCorner.LowerRight:
                            break;
                    }
                    switch (scrolllAxis)
                    {
                        case ScrolllAxis.Horizontal:
                            if (++row == maxRow) { row = 0; ++column; }
                            break;
                        case ScrolllAxis.Vertical:
                            if (++column == maxColumn) { column = 0; ++row; }
                            break;
                    }
                    poses[index++] = new Vector2 (x, y);
                }
                for (int i = 0; i < _items.Count; ++i)
                {
                    _items[i].anchoredPos = poses[i];
                }
            }
        }

        public void DestroyItemsOnEdit ()
        {
#if UNITY_EDITOR
            if (!EditorApplication.isPlaying && scrolllAxis != ScrolllAxis.Arbitrary)
            {
                if (_items.Count > 0)
                {
                    for (int i = 0; i < _items.Count; ++i)
                    {
                        DestroyImmediate (_items[i].gameObject);
                    }
                    _items.Clear ();
                }
            }
#endif
        }

        public int CalcRow ()
        {
            if (fixedRow || listType == ListType.Content) { return rowCount; }

            rowCount = Mathf.FloorToInt ( (rectTrans.sizeDelta.y - paddingY - spacing.y) / (_prefab.height + spacing.y));
            if ( (scrollType & ScrollType.Everything) != 0 && scrolllAxis == ScrolllAxis.Vertical) { rowCount += FULL_OUT_BOUND_COUNT; }
            return rowCount;
        }

        public int CalcColumn ()
        {
            if (fixedColumn || listType == ListType.Content) { return columnCount; }

            columnCount = Mathf.FloorToInt ( (rectTrans.sizeDelta.x - paddingX - spacing.x) / (_prefab.width + spacing.x));
            if ( (scrollType & ScrollType.Everything) != 0 && scrolllAxis == ScrolllAxis.Horizontal) { rowCount += FULL_OUT_BOUND_COUNT; }
            return columnCount;
        }

        private Vector2 CalcStartPos ()
        {
            if (!_prefab) { return Vector2.zero; }
            Vector2 pos = Vector2.zero;
            float x = 0f, y = 0f;
            switch (startCorner)
            {
                case StartCorner.UpperLeft:
                    x = paddingX + _prefab.width * 0.5f;
                    y = paddingY - _prefab.height * 0.5f;
                    break;
                case StartCorner.UpperCenter:
                    break;
                case StartCorner.UpperRight:
                    break;
                case StartCorner.MiddleLeft:
                    break;
                case StartCorner.MiddleCenter:
                    break;
                case StartCorner.MiddleRight:
                    break;
                case StartCorner.LowerLeft:
                    break;
                case StartCorner.LowerCenter:
                    break;
                case StartCorner.LowerRight:
                    break;
            }
            pos = new Vector2 (x, y);
            return pos;
        }

        public void AttachChilds ()
        {
            if (listType == ListType.Grid)
            {
                if (_items.Count > 0) { _items.Clear (); _observers.Clear (); }
                for (int i = 0; i < transform.childCount; ++i)
                {
                    _items.Add (transform.GetChild (i).GetComponent<BaseListItem> ());
                    AttachObserver (_items[i]);
                }
                CalcStartPos ();
            }
        }

        private void OnHorizontalBarValueChange (float value)
        {
            _barSetNormalPositon.x = value;
            TransitionMove (MoveType.ScrollBar);
        }

        private void OnVerticalBarValueChange (float value)
        {
            _barSetNormalPositon.y = value;
            TransitionMove (MoveType.ScrollBar);
        }

        public void SetHorizontalNormalizedPostion (float value)
        {
            if (updateBarValue)
            {
                _normalizedPosition.x = Mathf.Clamp01 (value);
                if (_horizontalBar)
                {
                    _horizontalBar.SetValueWithoutNotify (value);
                }
            }
        }

        public void SetVerticalNormalizedPostion (float value)
        {
            if (updateBarValue)
            {
                _normalizedPosition.y = Mathf.Clamp01 (value);
                if (_verticalBar)
                {
                    _verticalBar.SetValueWithoutNotify (value);
                }
            }
        }
    }
}

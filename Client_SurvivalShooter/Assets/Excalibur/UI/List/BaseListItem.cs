using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Excalibur;
using System;
using System.Reflection;

[DisallowMultipleComponent, RequireComponent (typeof (RectTransform))]
public abstract class BaseListItem : DirtyBehaviour,
    IPointerDownHandler, IPointerUpHandler, IPointerClickHandler,
    IObserver
{
    [SerializeField] private GameObject _selectedObject;

    private ExcaliburList _list;
    private RectTransform _rectTrans;
    [SerializeField] private int _dataIndex;
    public int dataIndex => _dataIndex;
    private bool isSelected;
    protected BaseData data;
    public float width => rectTrans.sizeDelta.x;
    public float height => rectTrans.sizeDelta.y;
    public float edgeXLeft => rectTrans.anchoredPosition.x - width * 0.5f;
    public float edgeXRight => rectTrans.anchoredPosition.x + width * 0.5f;
    public float edgeYTop => rectTrans.anchoredPosition.y + height * 0.5f;
    public float edgeYBottom => rectTrans.anchoredPosition.y - height * 0.5f;
    public Vector2 anchoredPos { get => rectTrans.anchoredPosition; set => rectTrans.anchoredPosition = value; }
    public RectTransform rectTrans { get { if (!_rectTrans) { _rectTrans = (RectTransform)transform; } return _rectTrans; } }

    protected override void Awake ()
    {
        _list = GetComponentInParent<ExcaliburList> ();
        base.Awake ();
    }

    protected T GetData<T> () where T : BaseData
    {
        return data != null ? (T)data : default (T);
    }

    public void SetIndex (int index)
    {
        _dataIndex = index;
        SetDirty ();
    }

    protected virtual void SetSelectedEffect ()
    {
        if (gameObject.activeSelf && _selectedObject.activeSelf != isSelected)
        {
            _selectedObject.SetActive (isSelected);
        }
    }

    protected override void Dirty ()
    {
        if (_list.IsIndexValidate (_dataIndex))
        {
            data = _list[_dataIndex];
            isSelected = _list.IsSelectedIndex (data);
            OnIndexValidate ();
            RefreshUI ();
        }
        else
        {
            OnIndexInvalidate ();
        }
        SetSelectedEffect ();
    }

    public override void Execute ()
    {
        isSelected = _list.IsSelectedIndex (data);
        SetSelectedEffect ();
    }

    protected virtual void OnIndexValidate ()
    {
        if (!gameObject.activeSelf) { gameObject.SetActive (true); }
    }

    protected virtual void OnIndexInvalidate ()
    {
        if (gameObject.activeSelf) { gameObject.SetActive (false); }
    }

    protected abstract void RefreshUI ();

    protected void OnSinglyClickedEvent ()
    {
        Debug.Log ("Singly Clicked");
    }

    protected void OnDoublyClickedEvent ()
    {
        Debug.Log ("Doubly Clicked");
    }

    protected void OnLongPressedEvent ()
    {
        Debug.Log ("Long Pressed");
    }

    public virtual void OnPointerDown (PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            Debug.Log ("Pointer Down");
        }
    }

    public virtual void OnPointerUp (PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            Debug.Log ("Pointer Up"); 
        }
    }

    public virtual void OnPointerClick (PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            Debug.Log ($"Pointer Click + {eventData.clickCount}");
            _list.SetSelected (_dataIndex);
            if (eventData.clickCount == 1) { OnSinglyClickedEvent (); }
            else if (eventData.clickCount == 2) { OnDoublyClickedEvent (); }
        }
    }
}

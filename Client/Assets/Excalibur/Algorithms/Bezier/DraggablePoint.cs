using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraggablePoint : MonoBehaviour
{
    public GameObject curveContainer; // 贝塞尔曲线容器对象

    private Vector3 offset;

    void OnMouseDonw ()
    {
        offset = transform.position - Camera.main.ScreenToWorldPoint (Input.mousePosition);
    }

    void OnMouseDrag ()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
        mousePos.z = 0f;
        transform.position = mousePos + offset;

        curveContainer.GetComponent<BezierCurveEditor> ().ComputeBezierCurve ();
    }
}

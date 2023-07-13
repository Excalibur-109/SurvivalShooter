using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierCurveEditor : MonoBehaviour
{
    public GameObject startPoint; // 起点对象
    public GameObject endPoint;   // 结束点对象
    public GameObject pointPrefab; // 拖动点的预制体

    private List<GameObject> controlPoints; // 控制点集合
    private List<List<Vector3>> curveSegments; // 曲线段集合

    void Start ()
    {
        controlPoints = new List<GameObject> ();
        curveSegments = new List<List<Vector3>> ();
    }

    void Update ()
    {
        if (Input.GetMouseButtonDown (0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
            mousePos.z = 0f;

            if (controlPoints.Count == 0)
            {
                CreateControlPoint (mousePos, true);
            }
            else if (controlPoints.Count == 1)
            {
                CreateControlPoint (mousePos, false);
            }
            else if (controlPoints.Count == 2)
            {
                CreateControlPoint (mousePos, true);
                ComputeBezierCurve ();
            }
            else
            {
                CreateControlPoint (mousePos, false);
                ComputeBezierCurve ();
            }
        }
    }

    private void CreateControlPoint (Vector3 position, bool isStartPoint)
    {
        GameObject go = Instantiate (pointPrefab, transform.parent);
        go.transform.position = position;
        go.GetComponent<DraggablePoint> ().curveContainer = gameObject;
    }

    public void ComputeBezierCurve ()
    {
        int n = controlPoints.Count - 1; // 曲线阶段数
        curveSegments.Clear ();
        for (int i = 0; i < n; ++i)
        {
            List<Vector3> segment = new List<Vector3> ();
            float t = 0f;

            while (t <= 1f)
            {
                Vector3 point = ComputeBezierPoint (controlPoints[i], controlPoints[i + 1], t);
                segment.Add (point);
                t += 0.01f;
            }

            curveSegments.Add (segment);
        }

        UpdateLineRenderer ();
    }

    private Vector3 ComputeBezierPoint (GameObject p0, GameObject p1, float t)
    {
        Vector3 p = new Vector3 ();
        p.x = (1 - t) * p0.transform.position.x + t * p1.transform.position.x;
        p.y = (1 - t) * p0.transform.position.y + t * p1.transform.position.y;
        p.z = (1 - t) * p0.transform.position.z + t * p1.transform.position.z;

        return p;
    }

    public void UpdateLineRenderer ()
    {
        LineRenderer lineRenderer = GetComponent<LineRenderer> ();
        lineRenderer.positionCount = 0;
        foreach (GameObject p in controlPoints)
        {
            ++lineRenderer.positionCount;
            lineRenderer.SetPosition (lineRenderer.positionCount - 1, p.transform.position);
        }

        foreach (List<Vector3> segment in curveSegments)
        {
            lineRenderer.positionCount += segment.Count;
            lineRenderer.SetPositions (segment.ToArray ());
        }
    }
}

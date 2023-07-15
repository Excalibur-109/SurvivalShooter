using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiOrderBezierCurve : MonoBehaviour
{
    public LineRenderer lineRenderer;

    private List<Vector3> controlPoints;
    private List<List<Vector3>> curveSegment;

    void Start ()
    {
        controlPoints = new List<Vector3> ();
        curveSegment = new List<List<Vector3>> ();
    }

    void Update ()
    {
        if (Input.GetMouseButtonDown (0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
            mousePos.z = 0f;
            controlPoints.Add (mousePos);

            if (controlPoints.Count > 1)
            {
                lineRenderer.positionCount = controlPoints.Count;
                lineRenderer.SetPositions (controlPoints.ToArray ());
            }

            if (controlPoints.Count > 2)
            {
                ComputeMultiOrderBezierCurve ();
            }
        }
    }

    private void ComputeMultiOrderBezierCurve ()
    {
        int n = controlPoints.Count - 1;
        int m = n / 3; // 曲线段数

        if (n % 3 != 0)
        {
            Debug.Log ("Error: Control points number should be a multiple of 3.");
            return;
        }

        curveSegment.Clear ();

        for (int i = 0; i < m; ++i)
        {
            List<Vector3> segment = new List<Vector3> ();
            float t = 0;

            while (t <= 1f)
            {
                Vector3 point = ComputeBezierPoint (controlPoints[i * 3], controlPoints[i * 3 + 1], controlPoints[i * 3 + 2], controlPoints[i * 3 + 3], t);
                segment.Add (point);
                t += 0.01f;
            }

            curveSegment.Add (segment);
        }

        lineRenderer.positionCount = 0;

        foreach (Vector3 p in controlPoints)
        {
            ++lineRenderer.positionCount;
            lineRenderer.SetPosition (lineRenderer.positionCount - 1, p);
        }

        foreach (List<Vector3> segment in curveSegment)
        {
            lineRenderer.positionCount += segment.Count;
            lineRenderer.SetPositions (segment.ToArray ());
        }
    }

    private Vector3 ComputeBezierPoint (Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        float t2 = t * t;
        float t3 = t2 * t;
        float u = 1 - t;
        float u2 = u * u;
        float u3 = u2 * u;

        Vector3 p = new Vector3 ();
        p.x = u3 * p0.x + 3 * u2 * t * p1.x + 3 * u * t2 * p2.x + t3 * p3.x;
        p.y = u3 * p0.y + 3 * u2 * t * p1.y + 3 * u * t2 * p2.y + t3 * p3.y;
        p.z = u3 * p0.z + 3 * u2 * t * p1.z + 3 * u * t2 * p2.z + t3 * p3.z;

        return p;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawBorder : MonoBehaviour
{
    LineRenderer lr;
    float lineWidth = 0.4f;

    float[] bounds;

    private void Start()
    {
        bounds = GetComponent<SimulationController>().bounds;

        lr = transform.Find("Border").GetComponent<LineRenderer>();
        lr.startWidth = lineWidth;
        lr.endWidth = lineWidth;

        float buffer = lineWidth + 0.1f;

        lr.SetPosition(0, new Vector3(bounds[0] - buffer, bounds[3] + buffer));
        lr.SetPosition(1, new Vector3(bounds[1] + buffer, bounds[3] + buffer));
        lr.SetPosition(2, new Vector3(bounds[1] + buffer, bounds[2] - buffer));
        lr.SetPosition(3, new Vector3(bounds[0] - buffer, bounds[2] - buffer));
    }
}

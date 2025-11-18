using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConcentricCircleFunction : MonoBehaviour
{
    public float radius = 5f; // 반지름
    public int segments = 36; // 세그먼트 수 (정밀도)
    public Color color = Color.cyan; // 색상

    void Update()
    {
        DrawCircle(transform.position, radius, segments, color);
        DrawVerticalCircle(transform.position, radius, segments, color);
    }

    void DrawCircle(Vector3 center, float radius, int segments, Color color)
    {
        float angleStep = 360f / segments;
        for (int i = 0; i < segments; i++)
        {
            float currentAngle = Mathf.Deg2Rad * angleStep * i;
            float nextAngle = Mathf.Deg2Rad * angleStep * (i + 1);

            Vector3 pointA = center + new Vector3(Mathf.Cos(currentAngle), 0, Mathf.Sin(currentAngle)) * radius;
            Vector3 pointB = center + new Vector3(Mathf.Cos(nextAngle), 0, Mathf.Sin(nextAngle)) * radius;

            Debug.DrawLine(pointA, pointB, color);
        }
    }

    void DrawVerticalCircle(Vector3 center, float radius, int segments, Color color)
    {
        float angleStep = 360f / segments;
        for (int i = 0; i < segments; i++)
        {
            float currentAngle = Mathf.Deg2Rad * angleStep * i;
            float nextAngle = Mathf.Deg2Rad * angleStep * (i + 1);

            Vector3 pointA = center + new Vector3(Mathf.Cos(currentAngle), Mathf.Sin(currentAngle), 0) * radius;
            Vector3 pointB = center + new Vector3(Mathf.Cos(nextAngle), Mathf.Sin(nextAngle), 0) * radius;

            Debug.DrawLine(pointA, pointB, color);
        }
    }


}

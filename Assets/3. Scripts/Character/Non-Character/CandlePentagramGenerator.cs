using UnityEngine;
using System.Collections.Generic;

public class CandlePentagramGenerator : MonoBehaviour
{
    float Radius = 30f;
    float EdgeAngle = 120f; // 5 edge
    int NumbOfBatching = 18;


    void Start()
    {
        float CurEdge = 0f;
        List<Vector3> Edges = new List<Vector3>();
        List<Vector3> CandlePosList = new List<Vector3>();
        for (int idx = 0; idx < 5; idx++)
        {
            CurEdge = idx * EdgeAngle;
            Vector3 EdgeVector = new Vector3(Mathf.Cos(CurEdge), Mathf.Sin(CurEdge), 0f);
            Edges.Add(EdgeVector);
        }

        for (int idx = 0; idx < 5; idx++)
        {
            int targetIDX = idx + 2;

            if (idx == 4)
            {
                targetIDX = 0;
            }

            Vector3 CurVec = Edges[idx];
            Vector3 TargetVec = Edges[targetIDX];

            for (int jdx = 0; jdx < NumbOfBatching; jdx++)
            {
                Vector3 candidate = CurVec + (TargetVec - CurVec) * (1f / NumbOfBatching);

                bool neighbor = false;
                foreach (Vector3 comp in CandlePosList)
                {
                    if (Mathf.Abs((candidate - comp).magnitude) < 1f)
                    {
                        neighbor = true;
                        break;
                    }
                }

                if (neighbor) CandlePosList.Add(candidate);
            }
        }
    }
}

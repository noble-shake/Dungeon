using System.Collections.Generic;
using System.Linq;
using Unity.Behavior;
using Unity.Netcode;
using UnityEngine;

public class SummonillusionSelf : MonoBehaviour
{
    [SerializeField] private GameObject summonPrefab;
    [SerializeField] private GameObject illusionPrefab;
    [SerializeField] private GameObject PentagramGroup;
    [SerializeField] private Vector3 illusionPoint;
    float Radius = 15f;
    [SerializeField] float EdgeAngle = 72f; // 5 edge
    [SerializeField] int NumbOfBatching = 20;
    [SerializeField] List<Vector3> Edges;
    [SerializeField] List<Vector3> CandlePosList;


    void CandlePositionGenerate()
    {
        float CurEdge = 0f;
        for (int idx = 0; idx < 5; idx++)
        {
            CurEdge = -180f + idx * EdgeAngle;
            Vector3 EdgeVector = new Vector3(Mathf.Cos(CurEdge * Mathf.Deg2Rad) * Radius, -0.3f, Mathf.Sin(CurEdge * Mathf.Deg2Rad) * Radius);
            Edges.Add(EdgeVector);
            CandlePosList.Add(EdgeVector);
        }

        for (int idx = 0; idx < 5; idx++)
        {
            int targetIDX = idx + 2;

            if (idx == 4)
            {
                targetIDX = 1;
            }
            if (idx == 3)
            {
                targetIDX = 0;
            }

            Vector3 CurVec = Edges[idx];
            Vector3 TargetVec = Edges[targetIDX];

            for (int jdx = 1; jdx < NumbOfBatching - 1; jdx++)
            {
                Vector3 candidate = CurVec + (TargetVec - CurVec) * (jdx / (float)NumbOfBatching);

                //bool neighbor = false;

                ////중첩 체크
                //foreach (Vector3 comp in CandlePosList)
                //{
                //    if (Mathf.Abs((candidate - comp).magnitude) < 0.1f)
                //    {
                //        neighbor = true;
                //        break;
                //    }
                //}

                // if (neighbor == false) 
                CandlePosList.Add(candidate);
            }
        }
    }


    private void Start()
    {
        CandlePositionGenerate();
        illusionPoint = CandlePosList[0];
    }

    public void SummonIllusionNetworkObject()
    {
        if (GameManager.Instance.IsOwner == false) return;


        // SubGraph가 동적으로 사용되지 않으면, 의미가 없는 코드. Shared를 사용하는게 더 바람직해보이긴한다..
        if (GetComponent<BehaviorGraphAgent>().GetVariable("Pattern3SubGraph", out BlackboardVariable<BehaviorGraph> sg))
        {
            sg.Value.BlackboardReference.SetVariableValue("CandleGenList", CandlePosList);
        }

        GameObject illusion = ObjectPoolManager.Singleton.GetObject(illusionPrefab);
        illusion.transform.position = PentagramGroup.transform.position + illusionPoint;
        illusion.transform.position += new Vector3(0f, -illusion.transform.position.y, 0f);

        List<Vector3> RouteList = new List<Vector3>();
        for (int i = 0; i < 5; i++)
        {
            RouteList.Add(PentagramGroup.transform.position + CandlePosList[i]);
        }
        illusion.GetComponent<BehaviorGraphAgent>().BlackboardReference.SetVariableValue("EdgeList", RouteList);
        illusion.GetComponent<BehaviorGraphAgent>().BlackboardReference.SetVariableValue("Summoner", this.gameObject);
        illusion.GetComponent<BehaviorGraphAgent>().BlackboardReference.SetVariableValue("AI", illusion.GetComponent<AICharacterManager>());
        illusion.GetComponent<NetworkObject>().Spawn();

        for (int i = 0; i < CandlePosList.Count; i++)
        {
            GameObject summonObject = ObjectPoolManager.Singleton.GetObject(summonPrefab);
            summonObject.transform.position = PentagramGroup.transform.position + CandlePosList[i];
            summonObject.GetComponent<NetworkObject>().Spawn();
        }


    }

}

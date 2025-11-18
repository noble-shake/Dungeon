using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using Unity.Services.Lobbies.Models;
using Unity.Netcode;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Pattern2MoveToMapCenter", story: "Move [Self] To Map [MapCenter] And [AI] , [RotationSpeed] Initialize", category: "Action", id: "19d58ca47f912540c52402b8520157cb")]
public partial class Pattern2MoveToMapCenterAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<Vector3> MapCenter;
    [SerializeReference] public BlackboardVariable<AICharacterManager> AI;
    [SerializeReference] public BlackboardVariable<float> RotationSpeed;
    private void SubGraphInitialize()
    {
        foreach (ulong playerObjectID in NetworkManager.Singleton.SpawnManager.SpawnedObjects.Keys)
        {
            if (NetworkManager.Singleton.SpawnManager.SpawnedObjects[playerObjectID].GetComponent<SummonillusionSelf>() != null)
            {
                Self.Value = NetworkManager.Singleton.SpawnManager.SpawnedObjects[playerObjectID].gameObject;
            }
        }



        // AI.Value = Self.Value.GetComponent<AICharacterManager>();

    }


    protected override Status OnStart()
    {
        SubGraphInitialize();
        BlackboardVariable<float> rtSpd;
        if (Self.Value.GetComponent<BehaviorGraphAgent>().BlackboardReference.GetVariable("MapCenter", out BlackboardVariable<Vector3> originPos))
        {
            MapCenter.Value = originPos.Value;
        }

        //if (Self.Value.GetComponent<BehaviorGraphAgent>().GetVariable<float>("RotationSpeed", out rtSpd))
        //{
        //    if (Self.Value.GetComponent<BehaviorGraphAgent>().GetVariable("Pattern3SubGraph", out BlackboardVariable<BehaviorGraph> sg)) // SubGraph�� TargetPlayer �Ҵ� �۾�.
        //    {
        //        sg.Value.BlackboardReference.SetVariableValue("RotationSpeed", rtSpd.Value);
        //        // RotationSpeed.Value = rtSpd.Value;
        //    }

        //    // Self.Value.GetComponent<BehaviorGraphAgent>().BlackboardReference.SetVariableValue("RotationSpeed", rtSpd);
        //}

        AI.Value = Self.Value.GetComponent<AICharacterManager>();
        AI.Value.animator.SetFloat("Vertical", 0);
        // Self.Value.GetComponent<AIBossCharacterNetworkManager>().TeleportDisappearServerRpc(true);
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (GameManager.Instance == null)
        {
            return Status.Running;
        }
        else
        {
            GameManager.Instance.SendMessageServerRpc("PATTERN 3 시작이다 !");
            return Status.Success;
        }
    }

    protected override void OnEnd()
    {

    }
}


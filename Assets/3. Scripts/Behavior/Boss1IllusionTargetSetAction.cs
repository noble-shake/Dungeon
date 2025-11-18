using System;
using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using Unity.Netcode;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Boss1_Illusion_TargetSet", story: "Initialize All Variables, [Self] and [TargetEdge] and [EdgeList]", category: "Action", id: "a67eadc001ef3b31d22a71b2043eb7a7")]
public partial class Boss1IllusionTargetSetAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<Vector3> TargetEdge;
    [SerializeReference] public BlackboardVariable<List<Vector3>> EdgeList;
    protected override Status OnStart()
    {
        // how to illusion check..?
        //foreach (ulong illusionID in NetworkManager.Singleton.SpawnManager.SpawnedObjects.Keys)
        //{
        //    if (NetworkManager.Singleton.SpawnManager.SpawnedObjects[illusionID].GetComponent<illusionIdentifier>() != null)
        //    {
        //        NetworkManager.Singleton.SpawnManager.SpawnedObjects[illusionID].GetComponent<BehaviorGraphAgent>().BlackboardReference.SetVariableValue("Self", NetworkManager.Singleton.SpawnManager.SpawnedObjects[illusionID].gameObject);
        //        NetworkManager.Singleton.SpawnManager.SpawnedObjects[illusionID].GetComponent<BehaviorGraphAgent>().BlackboardReference.SetVariableValue("AI", NetworkManager.Singleton.SpawnManager.SpawnedObjects[illusionID].GetComponent<AICharacterManager>());
        //        break;
        //    }

        //}
         
        


        TargetEdge.Value = EdgeList.Value[2];

        return Status.Success;
    }
}


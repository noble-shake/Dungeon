using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Invulnearble And BuffCount per BuffTerm Pattern", story: "[AI] Invulnearble And [BuffCount] per [BuffTerm] Update", category: "Action", id: "27d7e49b12f9aa786fb01b197b1c85bb")]
public partial class InvulnearbleAndBuffCountPerBuffTermPatternAction : Action
{
    [SerializeReference] public BlackboardVariable<AICharacterManager> AI;
    [SerializeReference] public BlackboardVariable<int> BuffCount;
    [SerializeReference] public BlackboardVariable<float> BuffTerm;
    private float BuffTime;
    
    protected override Status OnStart()
    {
        BuffTime = BuffTerm.Value;
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        AI.Value.GetComponent<AIBossCharacterCombatManager>().EnableIsInvulnerable();

        BuffTime -= Time.deltaTime;
        if (BuffTime < 0f && BuffCount < 5)
        {
            BuffCount.Value++;
            BuffTime = BuffTerm.Value;

            Debug.Log("Buff Count Up");
            // Buff Func.

        }
        
        return Status.Running;



        // return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}


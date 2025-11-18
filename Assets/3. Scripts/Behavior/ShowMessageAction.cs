using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "ShowMessage", story: "[Boss] Shows Pop Up Message [Text]", category: "Action", id: "2f27282bdefe076bd311c7f7a1d9ea4c")]
public partial class ShowMessageAction : Action
{
    [SerializeReference] public BlackboardVariable<AIWarriorGolemCharacterManager> Boss;
    [SerializeReference] public BlackboardVariable<string> Text;
    protected override Status OnStart()
    {
        GameManager.Instance.SendMessageServerRpc(Text.Value);
        return Status.Success;
    }

}


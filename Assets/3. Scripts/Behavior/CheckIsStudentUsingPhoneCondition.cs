using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "CheckIsStudentUsingPhone", story: "[Student] is using phone", category: "Conditions", id: "ae80f28c61415a0c1eeac6d001ae2f61")]
public partial class CheckIsStudentUsingPhoneCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> Student;

    public override bool IsTrue()
    {
        return true;
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}

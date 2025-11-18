using System.Collections;
using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;

public class Test : MonoBehaviour
{
    public GameObject test;


    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    private void Start()
    {

        int stateHash = Animator.StringToHash("State");
        test.GetComponent<BehaviorGraphAgent>().BlackboardReference.SetVariableValue("TestBool", true);
        test.GetComponent<BehaviorGraphAgent>().BlackboardReference.SetVariableValue("State", EnemyState.StanceBreak);

    }

}

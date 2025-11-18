using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "WaveIndicatorTimer", story: "[Self] , [IndicatorTimer] Wave Ready", category: "Action", id: "a7d5573c9f6c916dae49b709b5d9fec8")]
public partial class WaveIndicatorTimerAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<float> IndicatorTimer;
    private WarriorGolemPattern3Component WaveIndicatorComp;
    private float elapsedTime = 0f;
    protected override Status OnStart()
    {
        WaveIndicatorComp = Self.Value.GetComponent<WarriorGolemPattern3Component>();
        WaveIndicatorComp.indicatorActive.Value = true;
        WaveIndicatorComp.IndicatorWave.FillProgress = 1f;
        WaveIndicatorComp.indicatorFillProgress.Value = WaveIndicatorComp.IndicatorWave.FillProgress;
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
            if (elapsedTime < IndicatorTimer.Value)
            {
                WaveIndicatorComp.IndicatorWave.FillProgress = (elapsedTime / IndicatorTimer.Value);
                WaveIndicatorComp.indicatorFillProgress.Value = WaveIndicatorComp.IndicatorWave.FillProgress;
                elapsedTime += Time.deltaTime + (Time.deltaTime * 0.5f);

                return Status.Running;
            }
            else
            {
                WaveIndicatorComp.IndicatorWave.FillProgress = 1f;
                WaveIndicatorComp.indicatorFillProgress.Value = WaveIndicatorComp.IndicatorWave.FillProgress;

                elapsedTime = 0f;
                Self.Value.GetComponent<AICharacterManager>().aiCharacterAnimatorManager.PlayAnimationOnNetwork(AnimationType.Attack, "Boss1_Force_Execute", true);
                return Status.Success;
            }
    }

    protected override void OnEnd()
    {
    }
}


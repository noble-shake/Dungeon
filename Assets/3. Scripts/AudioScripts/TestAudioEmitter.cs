using FMODUnity;
using FMOD.Studio;
using UnityEngine;
using Unity.Netcode;
using FMOD;

public class TestAudioEmitter : NetworkBehaviour
{
    // [EventRef] public string thiefAttackEventPath = "event:/Character/Thief/Attack";

    public EventReference thiefAttackEventPathRef;
    private EventInstance attackInstance;

    public void PlayAttackSound(int comboStep)
    {
        if (IsOwner == false) return;

        comboStep = Mathf.Clamp(comboStep, 1, 4);

        attackInstance = RuntimeManager.CreateInstance(thiefAttackEventPathRef);

        attackInstance.setParameterByName("Thief_Combo", comboStep);

        attackInstance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject));

        attackInstance.start();

        // 5. 재생 후 자동 해제
        attackInstance.release();
    }
}
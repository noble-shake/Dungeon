using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Cysharp.Threading.Tasks;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class WakeBossTriggerZone : NetworkBehaviour
{
    [SerializeField] private AICharacterSpawner[] spawnerList;
    [SerializeField] private List<AIBossCharacterManager> bossList = new();
    [SerializeField] private CinemachineBrain cinemachineBrain;
    [SerializeField] private CinemachineVirtualCamera cutSceneCamera;
    [SerializeField] private CinemachineDollyCart dollyCart;
    [SerializeField] private float dollyCartSpeed = 0.15f;
    private NetworkVariable<bool> isTriggered = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    private void Start()
    {
        StartCoroutine(WaitAndGetInstantiatedObject());
    }

    private IEnumerator WaitAndGetInstantiatedObject()
    {
        foreach (var spawner in spawnerList)
        {
            while (true)
            {
                if (spawner.GetInstantiatedObject() == null)
                    yield return null;
                bossList.Add(spawner.GetInstantiatedObject().GetComponent<AIBossCharacterManager>());
                break;
            }
        }
    }

    // 보스 존 근처에 플레이어가 들어오면 컷신 이벤트를 발동시킨다.
    private void OnTriggerEnter(Collider other)
    {

        // 서버RPC 호출해서 인식해야 할 에너미 리스트를 보냄.
        // 서버에서 강제로 걔네의 타겟을 영역에 발을 들인 플레이어로 인식하게끔 수정.
        // 이 영역은 한번 사용되면 두번 다시 사용 되면 안 됨.
        // 기존 AI는 자기 자리 개념이 있음. 멀리 벗어나면 돌아감. -> behavior 트리 나중에 추가? 
        NetworkObject player = other.GetComponent<NetworkObject>();
        if (player != null && player.IsLocalPlayer && player.gameObject.tag == "Player")
        {
            WakeUpBossAIServerRpc(player.NetworkObjectId);
        }

    }

    // 

    [ServerRpc(RequireOwnership = false)]
    private void WakeUpBossAIServerRpc(ulong networkObjectId)
    {
        GameManager.Instance.InitializeBoss();
        if (isTriggered.Value == true) return;
        else
        {
            isTriggered.Value = true;
        }

        WakeUpBossAIClientRpc(networkObjectId);
    }

    // 페이드 아웃 해주고
    // 페이드 인 해주고
    // 카메라 옮겨주고 (컷으로)
    // 카메라 애니메이션 시작하고
    // 보스 캐릭터를 움직이고
    // 움직임 끝나면 다시 페이드 아웃 해주고
    // 카메라 돌려주고
    // 페이드 인 해주고 
    [ClientRpc]
    private void WakeUpBossAIClientRpc(ulong networkObjectId)
    {
        StartCoroutine(WakeUpBossAI());

    }

    private IEnumerator WakeUpBossAI()
    {
        // 플레이어 인풋 제한
        // 1. 추가로 키 입력을 막아야 함.
        // 2. 무브 인풋 등 남아있는 값들을 순차적으로 0으로 만들어준다.
        InputManager.instance.BlockLInput(); 

        // 화면 암전 
        yield return SceneLoadManager.Instance.FadeOut(1f);
        HUD_UIManager.instance.screenUIManager.ZoomInScreen();
        // 카메라 세팅
        var originBlend = cinemachineBrain.m_DefaultBlend;
        cinemachineBrain.m_DefaultBlend = new CinemachineBlendDefinition(CinemachineBlendDefinition.Style.Cut, 0f);
        cutSceneCamera.Priority = 20;

        // 화면 오픈 
        yield return SceneLoadManager.Instance.FadeIn(1f);
        // 컷신 카메라 출발 
        cutSceneCamera.GetComponent<Animator>().SetTrigger("CutSceneStart");
        dollyCart.m_Speed = dollyCartSpeed;

        // 컷신 시간동안 대기 
        yield return new WaitForSeconds(9f);
        HUD_UIManager.instance.screenUIManager.ZoomOutScreen();
        foreach (var boss in bossList)
        {
            boss.PlayCutScene();
        }
        InputManager.instance.UnBlockInput(); 

        yield return new WaitForSeconds(7f);
        yield return SceneLoadManager.Instance.FadeOut(1f);
        cutSceneCamera.Priority = -1;
        cinemachineBrain.m_DefaultBlend = originBlend;
        StartCoroutine(SceneLoadManager.Instance.FadeIn(1f));
        yield return new WaitForSeconds(1f);
        FindAnyObjectByType<AIWarriorGolemCharacterManager>().behaviorGraphAgent.BlackboardReference.SetVariableValue("State", EnemyState.Chase);

        foreach (PlayerManager player in GameManager.Instance.connectedPlayerList)
        {
            player.playerEmitter.SFXCutsceneVoice();
        }
    }
}

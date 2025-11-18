using DG.Tweening;
using System;
using System.Collections;
using System.Threading;
using Unity.Netcode;
using UnityEngine;

public class HeavyAttackAction_Parrying : NetworkBehaviour
{
    private PlayerManager player;

    public bool canParrying = false;
    public float parryingTime = 0.5f;
    public bool homingBoss = false;
    public float characterStiffTime = 0.25f;
    public float characterAttackingStiffTime = 0.15f;
    public float coolTime = 10f;
    public bool skillReady = true;

    // 최적화 문제 때문에 Unitask로 변경될 여지 있음. 
    private Coroutine parryingCoroutine;
    private Coroutine cooldownCoroutine;



    public CharacterManager homingTarget;
    public float maxSpeed = 10f; // 최대 속도
    public float minSpeed = 1f;  // 최소 속도
    public float moveDistance = 5f; // 이동할 거리
    public float duration = 1f; // 이동하는 데 걸리는 시간

    [Header("Debug")]
    public bool playParryingVFX = false;

    void Awake()
    {
        player = GetComponent<PlayerManager>();
    }

    void Update()
    {
        if (playParryingVFX)
        {
            playParryingVFX = false;
            PlayParryingVFX();
        }
    }

    public void AttempToPerformParrying()
    {
        if (CanParrying())
        {
            if (parryingCoroutine != null)
                StopCoroutine(parryingCoroutine);
            // parryingTime 동안 패링이 가능한 상태로 만든다.
            parryingCoroutine = StartCoroutine(SetParryingReady(parryingTime));
            // 패링 쿨타임을 시작한다. 패링 성공시 쿨타임이 초기화 된다. 
            cooldownCoroutine = StartCoroutine(ParryingCooldown(coolTime));
        }
    }

    // 쿨타임 관련 부분만 체크한다.현재 플레이어의 상태는 WeaponAction에서 체크한다.
    private bool CanParrying()
    {
        return skillReady;
    }


    // 이 코드를 실행했을 때, 패링 타임을 5초로 하든, 50초로 하든 
    private IEnumerator SetParryingReady(float parryingTime)
    {
        float elapsedTime = 0f;
        // 데미지 콜라이더에서 canParrying이 true인지 체크 후 패링이 발동된다. 
        canParrying = true;
        while (elapsedTime < parryingTime)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        canParrying = false;
    }

    private IEnumerator ParryingCooldown(float coolTime)
    {
        skillReady = false;
        yield return new WaitForSeconds(coolTime);
        skillReady = true;
    }

    // 공격해온 캐릭터와 내 캐릭터의 애니메이션 속도를 잠깐 0으로 만듦.
    // 그 후 RPC로 똑같은 일을 전파.
    public void Parrying(CharacterManager characterAttacking)
    {
        InitializeCoolTime();

        if (characterAttacking != null)
            player.playerLocomotionManager.PlayerRotationToWardsInputDirection(characterAttacking.transform.position - player.transform.position);

        player.playerAnimatorManager.PlayAnimationOnNetwork(AnimationType.Parrying, "Parry_Action_01", true, transitionNomalizedTime: 0f);

        // 얘를 RPC로 무빙 처리해야 되나? 
        MoveInDirection(-transform.forward, moveDistance);
        // TODO : 이제 보니 패링 후 경직이 RPC가 없네? 
        // 패링한 캐릭터 역경직
        GetStiff(player, characterStiffTime);
        // 패링 당한 캐릭터 경직
        GetStiff(characterAttacking, characterAttackingStiffTime);
    }


    public void PlayParryingVFX()
    {
        GameObject pooledObject = ObjectPoolManager.Singleton.GetObject(ObjectPoolType.Hit, 10);
        pooledObject.transform.position = player.playerCombatManager.parryingEffectTransform.position;
        pooledObject.transform.rotation = player.playerCombatManager.parryingEffectTransform.rotation;
        // parryingEffectPosition;
        ParticleSystem effect = pooledObject.GetComponent<ParticleSystem>();
        effect.Play();

        ObjectPoolManager.Singleton.WaitAndReturnVFX(pooledObject, ObjectPoolManager.Singleton.GetPrefab(ObjectPoolType.Hit, 10)).Forget();
    }

    public void GetStiff(CharacterManager character, float stopDuration)
    {
        StartCoroutine(HitStiffness(character, stopDuration));
        GetStiffServerRpc(player.OwnerClientId, character.NetworkObjectId, stopDuration);
    }

    [ServerRpc]
    private void GetStiffServerRpc(ulong clientId, ulong networkObjectId, float stopDuration)
    {
        GetStiffClientRpc(clientId, networkObjectId, stopDuration);
    }

    [ClientRpc]
    private void GetStiffClientRpc(ulong clientId, ulong networkObjectId, float stopDuration)
    {
        CharacterManager character = NetworkManager.Singleton.SpawnManager.SpawnedObjects[networkObjectId].GetComponent<CharacterManager>();
        // 반응성을 위해 클라이언트에선 이미 실행해줬음. 
        if (clientId != NetworkManager.Singleton.LocalClientId)
            StartCoroutine(HitStiffness(character, stopDuration));
    }

    public void MoveInDirection(Vector3 direction, float distance)
    {
        player.coroutineInProcess = StartCoroutine(MoveInDirectionCoroutine(direction.normalized, distance));
    }

    private IEnumerator MoveInDirectionCoroutine(Vector3 direction, float distance)
    {
        float movedDistance = 0f;
        float elapsedTime = 0f;

        while (movedDistance < distance)
        {
            float t = Mathf.Clamp01(elapsedTime / duration); // 진행 정도 (0~1)
            float speedFactor = (1f - t) * (1f - t); // 처음 빠르고 점점 느려짐 (Ease-out)
            float speed = Mathf.Lerp(minSpeed, maxSpeed, speedFactor);

            Vector3 moveStep = direction * speed * Time.deltaTime;
            player.characterController.Move(moveStep);
            movedDistance += moveStep.magnitude;
            elapsedTime += Time.deltaTime;

            yield return null;
        }
    }

    // 쿨타임 초기화 코루틴을 반드시 중지해야 됨. 
    private void InitializeCoolTime()
    {
        if (cooldownCoroutine != null)
        {
            StopCoroutine(cooldownCoroutine);
            cooldownCoroutine = null;
        }
        skillReady = true;
        InputManager.instance.UnBlockRightClick();
    }

    public IEnumerator HitStiffness(CharacterManager character, float stopDuration)
    {
        character.animator.speed = 0;
        yield return new WaitForSeconds(stopDuration);
        character.animator.speed = 1;
    }

    public void ReflectionParrying(CharacterManager characterAttacking, ObjectPoolType objectPoolType, int index, bool canAttackInGimmickProgres = false)
    {
        InitializeCoolTime();
        Vector3 direction = Camera.main.transform.forward;
        direction.y = 0;
        direction.Normalize();
        player.playerLocomotionManager.PlayerRotationToWardsInputDirection(direction);

        player.playerAnimatorManager.PlayAnimationOnNetwork(AnimationType.Parrying, "Parry_Action_01", true, transitionNomalizedTime: 0f);
        MoveInDirection(-transform.forward, moveDistance);
        GetStiff(player, characterStiffTime);
        if (characterAttacking != null)
            GetStiff(characterAttacking, characterAttackingStiffTime);

        // ReflectProjectileServerRpc(objectPoolType, index);
    }


    [ServerRpc]
    public void ReflectProjectileServerRpc(ObjectPoolType objectPoolType, int index)
    {
        ReflectProjectileClientRpc(objectPoolType, index);
    }

    [ClientRpc]
    public void ReflectProjectileClientRpc(ObjectPoolType objectPoolType, int index)
    {
        GameObject projectileObject = ObjectPoolManager.Singleton.GetObject(objectPoolType, index);
        ProjectileObject projectile = projectileObject.GetComponent<ProjectileObject>();

        projectile.SetPrefab(ObjectPoolManager.Singleton.GetPrefab(objectPoolType, index));
        projectile.SetMove(new RayProjectile(projectile));
        // projectile.Speed = BakedProjectiles[idx].Speed;
        projectile.Speed = -20;
        projectile.PenetratedCounter = 3;

        projectile.GetComponent<ProjectileCollider>().characterAttacking = GetComponent<CharacterManager>();
        projectile.GetComponent<ProjectileCollider>().isParryable = false;
        projectile.GetComponent<ProjectileCollider>().isReflectable = false;

        WorldUtilityManager.instance.CheckThisObjectCanAttackWhenInvulnerable(objectPoolType, index, out bool canAttackWhenInvincibleState);
        projectile.GetComponent<ProjectileCollider>().canAttackWhenInvulnerableState = canAttackWhenInvincibleState;

        projectile.GetComponent<ProjectileCollider>().physicalDamage = 10;
        projectile.GetComponent<ProjectileCollider>().groggyDamage = 10;

        projectile.transform.position = player.transform.position + Vector3.up + player.transform.forward;
        Vector3 reflectDirection = Camera.main.transform.forward;
        reflectDirection.y = 0;
        projectile.transform.rotation = Quaternion.LookRotation(-reflectDirection);
        // projectile.GetComponent<ProjectileCollider>().isParryable = BakedProjectiles[idx].isParryable;
        // projectile.GetComponent<ProjectileCollider>().isReflectable = BakedProjectiles[idx].isReflectable;
        // projectile.GetComponent<ProjectileCollider>().physicalDamage = BakedProjectiles[idx].AttackDamage;
        // projectile.GetComponent<ProjectileCollider>().groggyDamage = BakedProjectiles[idx].AttackGroggyDamage;

        projectile.gameObject.SetActive(true);

    }

}

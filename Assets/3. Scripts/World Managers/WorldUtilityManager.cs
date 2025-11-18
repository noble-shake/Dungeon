using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class WorldUtilityManager : MonoBehaviour
{
    public static WorldUtilityManager instance;

    [Header("Layers")]
    [SerializeField] LayerMask characterLayers;
    [SerializeField] LayerMask environmentLayers;

    private Dictionary<(int, int), bool> originalMatrix = new();
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        SaveCollisionMatrix();
    }

    public LayerMask GetCharacterLayers()
    {
        return characterLayers;
    }

    public LayerMask GetEnvironmentLayers()
    {
        return environmentLayers;
    }

    public bool CanIDamageThisTarget(CharacterType attackingCharacter, CharacterType targetCharacter)
    {
        if (attackingCharacter == CharacterType.Player)
        {
            switch (targetCharacter)
            {
                case CharacterType.Player: return false;
                case CharacterType.Monster: return true;
                default:
                    break;
            }
        }
        else if (attackingCharacter == CharacterType.Monster)
        {
            switch (targetCharacter)
            {
                case CharacterType.Player: return true;
                case CharacterType.Monster: return false;
                default:
                    break;
            }
        }
        return false;
    }

    public float GetAngleOfTarget(Transform characterTransform, Vector3 targetDirection)
    {
        targetDirection.y = 0;
        float viewableAngle = Vector3.Angle(characterTransform.forward, targetDirection);
        Vector3 cross = Vector3.Cross(characterTransform.forward, targetDirection);

        if (cross.y < 0)
        {
            viewableAngle = -viewableAngle;
        }
        return viewableAngle;
    }

    public DamageIntensity GetDamageIntensityBasedOnPoiseDamage(float poiseDamage)
    {
        DamageIntensity damageIntensity = DamageIntensity.Light;

        if (poiseDamage < 70)
            damageIntensity = DamageIntensity.Light;

        if (poiseDamage >= 70)
            damageIntensity = DamageIntensity.Heavy;


        return damageIntensity;
    }



    private void SaveCollisionMatrix()
    {
        originalMatrix.Clear();
        for (int i = 0; i < 32; i++)
        {
            for (int j = 0; j < 32; j++)
            {
                originalMatrix[(i, j)] = Physics.GetIgnoreLayerCollision(i, j);
            }
        }
    }

    // 모든 충돌을 비활성화
    public void DisableAllCollisions()
    {
        for (int i = 0; i < 32; i++)
        {
            for (int j = 0; j < 32; j++)
            {
                Physics.IgnoreLayerCollision(i, j, true);
            }
        }
    }

    // 특정 레이어가 다른 모든 레이어와의 충돌을 비활성화
    public void DisableLayerCollisions(int layer)
    {
        for (int i = 0; i < 32; i++)
        {
            originalMatrix[(layer, i)] = Physics.GetIgnoreLayerCollision(layer, i);
            Physics.IgnoreLayerCollision(layer, i, true);
        }
    }

    // 특정 레이어의 충돌 매트릭스를 원래대로 복원
    public void RestoreLayerCollisions(int layer)
    {
        for (int i = 0; i < 32; i++)
        {
            if (originalMatrix.ContainsKey((layer, i)))
            {
                Physics.IgnoreLayerCollision(layer, i, originalMatrix[(layer, i)]);
            }
        }
    }

    // 원래 충돌 매트릭스로 복원
    public void RestoreCollisionMatrix()
    {
        foreach (var pair in originalMatrix)
        {
            Physics.IgnoreLayerCollision(pair.Key.Item1, pair.Key.Item2, pair.Value);
        }
    }

    public void CheckThisObjectCanAttackWhenInvulnerable(ObjectPoolType objectPoolType, int index, out bool canAttackWhenInvulnerable)
    {
        switch (objectPoolType)
        {
            case ObjectPoolType.Projectile:
                switch (index)
                {
                    case 0:
                        canAttackWhenInvulnerable = true;
                        return;
                }
                break;
        }
        canAttackWhenInvulnerable = false;
    }
}

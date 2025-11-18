using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Character Spawn/Spawn Point")]
public class PlayerSpawnPoint : MonoBehaviour
{
    [Header("World Scene Index")]
    [SerializeField] private int worldSceneIndex = 1;

    [Header("World Coordinates")]
    public float xPosition;
    public float yPosition;
    public float zPosition;

    
}

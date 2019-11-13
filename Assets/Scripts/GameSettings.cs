using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettings : MonoBehaviour
{
    [SerializeField] private float droneSpeed = 2f;
    [SerializeField] private float aggroRadius = 4f;
    [SerializeField] private float attackRange = 3f;
    [SerializeField] private GameObject droneProjectilePrefab;

    public static GameSettings Instance { get; private set; }

    public static float DroneSpeed => Instance.droneSpeed;
    public static float AggroRadius => Instance.aggroRadius;
    public static float AttackRange => Instance.attackRange;
    public static GameObject DroneProjectilePrefab => Instance.droneProjectilePrefab;


    private void Awake()
    {
        if (Instance != null)
            Destroy(gameObject);
        else
            Instance = this;
    }
}

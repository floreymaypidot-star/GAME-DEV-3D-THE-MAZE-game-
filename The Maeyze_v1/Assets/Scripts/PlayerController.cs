using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] public GameObject enemyPrefab;
    [SerializeField] public Transform[] spawnPoints;     // CHANGED: array, so we can have 3 spawn points
    [SerializeField] public float spawnDistance = 5f;    // ADDED: distance trigger (was the fixed 5f)
    [SerializeField] public int maxEnemies = 8;          // ADDED: spawn limit
    [SerializeField] public float cooldown = 1f;         // ADDED: seconds between spawns

    private float nextSpawnTime = 0f;                    // ADDED
    private bool[] hasSpawned;                           // ADDED: spawn once per visit, not every frame

    void Start()
    {
        hasSpawned = new bool[spawnPoints.Length];

        // ADDED: messages in the Console tell you what is missing
        if (enemyPrefab == null)
            Debug.LogError("PlayerController: Enemy Prefab is EMPTY. Drag the enemy prefab into the slot.");
        if (spawnPoints.Length == 0)
            Debug.LogError("PlayerController: Spawn Points is EMPTY. Set Size to 3 and drag the spawn points in.");
    }

    void Update()
    {
        if (enemyPrefab == null || spawnPoints.Length == 0) return;

        // KEYPRESS: press F to spawn an enemy 8 units in front of the player
        if (Input.GetKeyDown(KeyCode.F) && CanSpawn())
        {
            Vector3 pos = transform.position + transform.forward * 8f;

            // make sure the spot is on the NavMesh
            if (UnityEngine.AI.NavMesh.SamplePosition(pos, out UnityEngine.AI.NavMeshHit navHit, 3f, UnityEngine.AI.NavMesh.AllAreas))
            {
                Instantiate(enemyPrefab, navHit.position, Quaternion.identity);
                nextSpawnTime = Time.time + cooldown;
            }
        }

        // DISTANCE: Instantiate enemyPrefab if the player distance to a spawnPoint is < spawnDistance
        // Distance (Point A, Point B)
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            if (spawnPoints[i] == null) continue;

            if (Vector3.Distance(transform.position, spawnPoints[i].position) < spawnDistance)
            {
                // ADDED: only once per visit, and only if cooldown and limit allow it
                if (!hasSpawned[i] && CanSpawn())
                {
                    Instantiate(enemyPrefab, spawnPoints[i].position, Quaternion.identity);
                    hasSpawned[i] = true;
                    nextSpawnTime = Time.time + cooldown;
                }
            }
            else
            {
                hasSpawned[i] = false;   // player walked away, this spawn point can trigger again
            }
        }
    }

    // ADDED: cooldown + maximum number of enemies
    bool CanSpawn()
    {
        if (Time.time < nextSpawnTime) return false;
        return FindObjectsOfType<AIEnemyController>().Length < maxEnemies;
    }
}
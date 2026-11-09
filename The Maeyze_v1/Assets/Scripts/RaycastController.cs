using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RayCastController : MonoBehaviour
{
    [SerializeField] public GameObject enemyPrefab;
    [SerializeField] public Camera playerCamera;
    [SerializeField] public float rayLength = 100f;      // CHANGED: longer ray (was 10)
    [SerializeField] public int maxEnemies = 8;          // ADDED: spawn limit
    [SerializeField] public float cooldown = 1f;         // ADDED: seconds between spawns

    private float nextSpawnTime = 0f;                    // ADDED

    void Start()
    {
        // ADDED: messages in the Console tell you what is missing
        if (enemyPrefab == null)
            Debug.LogError("RayCastController: Enemy Prefab is EMPTY. Drag the enemy prefab into the slot.");
        if (playerCamera == null)
            Debug.LogError("RayCastController: Player Camera is EMPTY. Drag the PlayerCamera into the slot.");
    }

    void Update()
    {
        if (enemyPrefab == null || playerCamera == null) return;

        //Ray cast Line(start position, direction, length, color)
        //Raycast() = TRUE | FALSE
        Debug.DrawRay(playerCamera.transform.position, playerCamera.transform.forward * rayLength, Color.red);

        //start. direction. raycasthit. length
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit hit, rayLength))
        {
            // LEFT MOUSE CLICK: Instantiate the enemy where the ray hit
            if (Input.GetMouseButtonDown(0) && CanSpawn())
            {
                // ADDED: make sure the spot is on the NavMesh so the enemy can move
                if (NavMesh.SamplePosition(hit.point, out NavMeshHit navHit, 2f, NavMesh.AllAreas))
                {
                    Instantiate(enemyPrefab, navHit.position, Quaternion.identity);
                    nextSpawnTime = Time.time + cooldown;
                }
                else
                {
                    Debug.LogWarning("No NavMesh near that spot. Click on the blue area.");
                }
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
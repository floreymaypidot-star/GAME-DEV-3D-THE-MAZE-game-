using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// Put this on an EMPTY GameObject (e.g. "EnemySpawner") in the scene.
// The enemy spawns BY ITSELF, no key or click needed:
//   - nothing spawns during the grace period at the start
//   - then it spawns at random times
//   - always on the NavMesh, far from the player, and never where the player can see it
public class RandomEnemySpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject enemyPrefab;        // the krasue PREFAB
    [SerializeField] private Transform player;              // leave empty to find the Player tag

    [Header("Timing")]
    [SerializeField] private float gracePeriod = 20f;       // GDD: 20 seconds with no enemy at the start
    [SerializeField] private float minInterval = 15f;       // random wait between spawns
    [SerializeField] private float maxInterval = 40f;
    [SerializeField] private int maxEnemies = 2;            // how many can exist at the same time

    [Header("Where it can appear")]
    [SerializeField] private float minSpawnDistance = 20f;  // never closer than this to the player
    [SerializeField] private float maxSpawnDistance = 40f;  // never farther than this
    [SerializeField] private bool avoidPlayerView = true;   // do not pop up in front of the player's eyes
    [SerializeField] private int maxAttempts = 30;          // tries to find a good spot each time

    [Header("Optional: use your Spawnpoint objects instead of random spots")]
    [SerializeField] private bool useSpawnPoints = false;
    [SerializeField] private Transform[] spawnPoints;

    [Header("Despawn (so enemies do not pile up)")]
    [SerializeField] private bool useDespawn = true;
    [SerializeField] private float despawnDistance = 50f;   // farther than this from the player...
    [SerializeField] private float farDespawnDelay = 8f;    // ...for this many seconds = it disappears
    [SerializeField] private float maxLifetime = 90f;       // disappears after this many seconds (0 = off)...
    [SerializeField] private float safeDistance = 20f;      // ...but ONLY if farther than this and out of view

    private class Tracked
    {
        public GameObject obj;
        public float spawnTime;
        public float farTime;
    }

    private readonly List<Tracked> tracked = new List<Tracked>();
    private float cleanupTimer;
    private float nextSpawnTime;
    private Camera cam;

    void Start()
    {
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }
        if (player == null) Debug.LogError("RandomEnemySpawner: no Player found. Set the Player tag.");
        if (enemyPrefab == null) Debug.LogError("RandomEnemySpawner: Enemy Prefab is EMPTY.");

        cam = Camera.main;
        nextSpawnTime = Time.time + gracePeriod + Random.Range(minInterval, maxInterval) * 0.25f;
    }

    void Update()
    {
        if (player == null || enemyPrefab == null) return;

        if (useDespawn) HandleDespawn();

        if (Time.time < nextSpawnTime) return;

        if (FindObjectsOfType<AIEnemyController>().Length >= maxEnemies)
        {
            nextSpawnTime = Time.time + 5f;     // full, check again soon
            return;
        }

        if (TryFindSpawnPosition(out Vector3 pos))
        {
            GameObject enemy = Instantiate(enemyPrefab, pos, Quaternion.identity);
            tracked.Add(new Tracked { obj = enemy, spawnTime = Time.time, farTime = 0f });
            nextSpawnTime = Time.time + Random.Range(minInterval, maxInterval);
        }
        else
        {
            nextSpawnTime = Time.time + 2f;     // no good spot right now, try again in 2s
        }
    }

    // Removes enemies that are far away for a while, or that lived too long
    // (never while the player can see them, and never when they are close).
    private void HandleDespawn()
    {
        cleanupTimer -= Time.deltaTime;
        if (cleanupTimer > 0f) return;
        float step = 0.5f;
        cleanupTimer = step;

        for (int i = tracked.Count - 1; i >= 0; i--)
        {
            Tracked t = tracked[i];

            // already destroyed by something else (e.g. a different script)
            if (t.obj == null) { tracked.RemoveAt(i); continue; }

            float dist = Vector3.Distance(t.obj.transform.position, player.position);
            bool visible = IsVisibleToPlayer(t.obj.transform.position);

            // 1) far away for too long
            if (dist > despawnDistance) t.farTime += step;
            else t.farTime = 0f;

            bool tooFar = t.farTime >= farDespawnDelay;

            // 2) lived too long, but only vanish when it is safe (not close, not in view)
            bool tooOld = maxLifetime > 0f
                          && Time.time - t.spawnTime >= maxLifetime
                          && dist > safeDistance
                          && !visible;

            if ((tooFar && !visible) || tooOld)
            {
                Destroy(t.obj);
                tracked.RemoveAt(i);
            }
        }
    }

    private bool TryFindSpawnPosition(out Vector3 result)
    {
        for (int i = 0; i < maxAttempts; i++)
        {
            Vector3 candidate;

            if (useSpawnPoints && spawnPoints != null && spawnPoints.Length > 0)
            {
                Transform sp = spawnPoints[Random.Range(0, spawnPoints.Length)];
                if (sp == null) continue;
                candidate = sp.position;
            }
            else
            {
                Vector2 circle = Random.insideUnitCircle.normalized;
                float dist = Random.Range(minSpawnDistance, maxSpawnDistance);
                candidate = player.position + new Vector3(circle.x, 0f, circle.y) * dist;
            }

            // must be on the NavMesh
            if (!NavMesh.SamplePosition(candidate, out NavMeshHit hit, 4f, NavMesh.AllAreas)) continue;

            // must be far enough from the player
            if (Vector3.Distance(hit.position, player.position) < minSpawnDistance) continue;

            // must not be where the player is looking
            if (avoidPlayerView && IsVisibleToPlayer(hit.position)) continue;

            result = hit.position;
            return true;
        }

        result = Vector3.zero;
        return false;
    }

    private bool IsVisibleToPlayer(Vector3 worldPos)
    {
        if (cam == null) return false;

        Vector3 eyeTarget = worldPos + Vector3.up * 1.5f;
        Vector3 vp = cam.WorldToViewportPoint(eyeTarget);

        bool inFrontOfCamera = vp.z > 0f && vp.x > -0.1f && vp.x < 1.1f && vp.y > -0.1f && vp.y < 1.1f;
        if (!inFrontOfCamera) return false;

        // in the camera's view, but is a wall blocking it? (a hit means blocked = not visible)
        return !Physics.Linecast(cam.transform.position, eyeTarget);
    }
}
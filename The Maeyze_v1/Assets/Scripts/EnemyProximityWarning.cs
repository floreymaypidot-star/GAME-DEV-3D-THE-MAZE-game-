using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// Put this on the Player.
// Uses the existing ElectricTorchOnOff.SetDrainMultiplier(), so no duplicate battery system.
// When a krasue (AIEnemyController) is within dangerRadius:
//   - the torch drains faster
//   - a red overlay pulses (faster + stronger the closer it is)
//   - optional warning text and heartbeat sound
public class EnemyProximityWarning : MonoBehaviour
{
    [Header("Torch")]
    [SerializeField] private ElectricTorchOnOff torch;      // drag the torch (the object with ElectricTorchOnOff)

    [Header("Detection")]
    [SerializeField] private float dangerRadius = 15f;      // GDD: about 15 m
    [SerializeField] private float drainMultiplier = 4f;    // normal drain x this when enemy is near
    [SerializeField] private float scanInterval = 0.2f;

    [Header("Warning Sign (all optional)")]
    [SerializeField] private CanvasGroup warningOverlay;    // full-screen red Image + CanvasGroup
    [SerializeField] private TMP_Text warningText;          // e.g. "SOMETHING IS NEAR..."
    [SerializeField] private AudioSource heartbeat;         // looping heartbeat clip

    private float scanTimer;
    private float nearestDistance = Mathf.Infinity;

    void Start()
    {
        if (torch == null) torch = FindObjectOfType<ElectricTorchOnOff>();
        if (torch == null) Debug.LogError("EnemyProximityWarning: Torch is EMPTY and none was found in the scene.");

        if (warningOverlay != null) warningOverlay.alpha = 0f;
        if (warningText != null) warningText.alpha = 0f;
        if (heartbeat != null)
        {
            heartbeat.loop = true;
            heartbeat.volume = 0f;
            if (!heartbeat.isPlaying) heartbeat.Play();
        }
    }

    void Update()
    {
        scanTimer -= Time.deltaTime;
        if (scanTimer <= 0f)
        {
            scanTimer = scanInterval;
            nearestDistance = FindNearestEnemyDistance();
        }

        bool near = nearestDistance <= dangerRadius;

        // faster battery drain
        if (torch != null) torch.SetDrainMultiplier(near ? drainMultiplier : 1f);

        // 0 = edge of radius, 1 = right next to the enemy
        float closeness = near ? 1f - Mathf.InverseLerp(0f, dangerRadius, nearestDistance) : 0f;

        float speed = Mathf.Lerp(2f, 9f, closeness);
        float pulse = Mathf.Sin(Time.time * speed) * 0.5f + 0.5f;
        float targetAlpha = near ? pulse * Mathf.Lerp(0.2f, 0.6f, closeness) : 0f;

        if (warningOverlay != null)
            warningOverlay.alpha = Mathf.Lerp(warningOverlay.alpha, targetAlpha, Time.deltaTime * 10f);

        if (warningText != null)
            warningText.alpha = Mathf.Lerp(warningText.alpha, near ? 1f : 0f, Time.deltaTime * 6f);

        if (heartbeat != null)
        {
            heartbeat.volume = Mathf.Lerp(heartbeat.volume, closeness, Time.deltaTime * 5f);
            heartbeat.pitch = Mathf.Lerp(0.9f, 1.5f, closeness);
        }
    }

    private float FindNearestEnemyDistance()
    {
        float best = Mathf.Infinity;
        AIEnemyController[] enemies = FindObjectsOfType<AIEnemyController>();
        for (int i = 0; i < enemies.Length; i++)
        {
            float d = Vector3.Distance(transform.position, enemies[i].transform.position);
            if (d < best) best = d;
        }
        return best;
    }

    void OnDisable()
    {
        if (torch != null) torch.SetDrainMultiplier(1f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, dangerRadius);
    }
}
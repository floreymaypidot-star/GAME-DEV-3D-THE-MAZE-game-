using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Put this on the KRASUE PREFAB (edit the prefab, not the Clone).
// INVERTED AUDIO:
//   enemy FAR  -> click is LOUD
//   enemy NEAR -> click is QUIET (but never fully silent)
// Also pans left/right toward the enemy's direction.
[RequireComponent(typeof(AudioSource))]
public class EnemyClickAudio : MonoBehaviour
{
    [Header("Sound")]
    [SerializeField] private AudioClip clickClip;           // dry looping clicking sound
    [SerializeField, Range(0f, 1f)] private float minVolume = 0.1f;  // volume when enemy is right next to you
    [SerializeField, Range(0f, 1f)] private float maxVolume = 1f;    // volume when enemy is far

    [Header("Distance Range")]
    [SerializeField] private float nearDistance = 3f;       // at or below this = minVolume
    [SerializeField] private float farDistance = 30f;       // at or above this = maxVolume

    [Header("Panning")]
    [SerializeField] private bool usePan = true;
    [SerializeField] private float panSmooth = 8f;

    private AudioSource source;
    private Transform player;
    private Transform listener;

    void Awake()
    {
        source = GetComponent<AudioSource>();
        source.clip = clickClip != null ? clickClip : source.clip;
        source.loop = true;
        source.spatialBlend = 0f;     // 2D, so Unity's own distance fall-off does not undo the effect
        source.playOnAwake = false;
    }

    void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
        else Debug.LogError("EnemyClickAudio: no object with the Player tag was found.");

        if (Camera.main != null) listener = Camera.main.transform;

        if (source.clip == null)
            Debug.LogError("EnemyClickAudio: Click Clip is EMPTY. Drag a click sound into the slot.");
        else
            source.Play();
    }

    void Update()
    {
        if (player == null || source.clip == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        // INVERTED: 0 when near, 1 when far
        float t = Mathf.InverseLerp(nearDistance, farDistance, distance);
        source.volume = Mathf.Lerp(minVolume, maxVolume, t);

        if (usePan && listener != null)
        {
            Vector3 dir = transform.position - listener.position;
            dir.y = 0f;
            if (dir.sqrMagnitude > 0.01f)
            {
                // -1 = enemy on the left, +1 = enemy on the right
                float side = Vector3.Dot(listener.right, dir.normalized);
                source.panStereo = Mathf.Lerp(source.panStereo, Mathf.Clamp(side, -1f, 1f), Time.deltaTime * panSmooth);
            }
        }
    }
}
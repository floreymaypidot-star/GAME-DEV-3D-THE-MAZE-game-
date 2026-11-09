using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class AIEnemyController : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] public Transform Target;
    [SerializeField] public NavMeshAgent Agent;

    [Header("Detection Settings")]
    [SerializeField] private float detectionRadius = 15f;   // CHANGED: bigger (was 10f)

    
    // ADDED: a prefab cannot keep a scene object in Target (it always becomes None),
    // so every enemy finds the Player by itself the moment it is spawned.
    void Start()
    {
        if (Agent == null) Agent = GetComponent<NavMeshAgent>();

        if (Target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) Target = player.transform;
            else Debug.LogError("AIEnemyController: no object with the Player tag was found.");
        }
    }

    void Update()
    {
        // CHANGED: also stop if the enemy is not standing on the NavMesh
        if (Target == null || Agent == null || !Agent.isOnNavMesh)
            return;

        // Check the distance between enemy and player
        float distance = Vector3.Distance(
            transform.position,
            Target.position
        );

        // Chase the player only when inside the radius
        if (distance <= detectionRadius)
        {
            Agent.SetDestination(Target.position);
        }
        else
        {
            // Stop chasing when the player leaves the radius
            Agent.ResetPath();
        }
    }


    // Show the detection radius in the Scene view
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
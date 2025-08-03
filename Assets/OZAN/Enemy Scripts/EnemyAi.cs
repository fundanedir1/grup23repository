using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Simple ground‑zombie AI:
/// • Prioritises targets in this order
///   1. Player if inside <senseRadius>
///   2. Nearest TilePlot that currently has a crop
///   3. Player (fallback)
/// • Uses NavMeshAgent for path‑finding.
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(EnemyHealth))]
[AddComponentMenu("Game/Enemy/Enemy AI")]
public class EnemyAi : MonoBehaviour
{
    [Header("Sensing")]
    [Tooltip("Radius in which the player will always be prioritised")]
    [SerializeField] private float senseRadius = 12f;

    [Header("Attack")]
    [Tooltip("How close before triggering attack anim (agent.stoppingDistance is used for actual nav)")]
    [SerializeField] private float attackTriggerDistance = 1.8f;

    private NavMeshAgent agent;
    private Transform    player;
    private TilePlot      targetPlot;
    private Animator     anim;

    // Cached sqr radius for cheap distance check
    private float senseRadiusSqr;

    private static readonly int AnimMove = Animator.StringToHash("MoveSpeed");
    private static readonly int AnimHit  = Animator.StringToHash("Hit");

    private void Awake()
    {
        agent           = GetComponent<NavMeshAgent>();
        anim            = GetComponentInChildren<Animator>();
        player          = GameObject.FindGameObjectWithTag("Player")?.transform;
        senseRadiusSqr  = senseRadius * senseRadius;
    }

    private void Update()
    {
        if (!player) return;      // safety

        // 1) Player inside sense radius?  -> Player target wins
        if ((player.position - transform.position).sqrMagnitude <= senseRadiusSqr)
        {
            targetPlot = null;
            SetDestination(player.position);
        }
        else // 2) Try nearest growing/ripe plot
        {
            if (targetPlot == null || !targetPlot.HasCrop)
                targetPlot = CropRegistry.GetNearestOccupied(transform.position);

            if (targetPlot)
                SetDestination(targetPlot.transform.position);
            else // 3) fallback to player
                SetDestination(player.position);
        }

        UpdateAnimation();
    }

    private void SetDestination(Vector3 worldPos)
    {
        if (!agent.isOnNavMesh) return; // Could log nav error
        agent.SetDestination(worldPos);
    }

    // ─────────────────── Animation / Attack ───────────────────
    private void UpdateAnimation()
    {
        if (!anim) return;
        float speed = agent.velocity.magnitude / agent.speed; // normalised 0‑1
        anim.SetFloat(AnimMove, speed);

        // Attack trigger (simple): if close enough & agent cannot move further
        if (agent.remainingDistance <= attackTriggerDistance && agent.pathPending == false)
            anim.SetTrigger(AnimHit);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, senseRadius);
    }
#endif
}

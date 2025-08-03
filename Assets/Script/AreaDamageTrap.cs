using UnityEngine;

/// <summary>
/// AreaDamageTrap – every <damageInterval> seconds damages all “Enemy” colliders
/// inside <radius>. Works with EnemyHealth.GetDamage(float).
/// If <lifetime> > 0 the trap destroys itself after that many seconds.
/// </summary>
[AddComponentMenu("Game/Traps/Area Damage Trap")]
public class AreaDamageTrap : MonoBehaviour
{
    [Header("Damage Settings")]
    public float damage = 10f;
    public float damageInterval = 1f;
    public float radius = 3f;

    [Header("Lifetime (seconds)")]
    [Tooltip("≤ 0 = infinite")] public float lifetime = 0f;

    [Header("FX (optional)")]
    public ParticleSystem pulseFx;
    public AudioSource pulseSound;

    // Pre‑allocated collider buffer for non‑alloc overlap (size 32 should be plenty)
    private static readonly Collider[] buffer = new Collider[32];

    private void OnEnable()
    {
        if (damageInterval < 0.01f) damageInterval = 0.5f;
        InvokeRepeating(nameof(DealDamageOnce), 0f, damageInterval);

        if (lifetime > 0f)
            Destroy(gameObject, lifetime);
    }

        private void DealDamageOnce()
    {
        if (pulseFx) pulseFx.Play();
        if (pulseSound) pulseSound.Play();

        int count = Physics.OverlapSphereNonAlloc(transform.position, radius, buffer, ~0);
        for (int i = 0; i < count; i++)
        {
            var col = buffer[i];
            if (col == null) continue;

            // 1) Doğrudan EnemyHealth bileşeni arar
            if (!col.TryGetComponent(out EnemyHealth hp))
            {
                // 2) Rigidbody üzerindeyse
                if (col.attachedRigidbody && col.attachedRigidbody.TryGetComponent(out hp)) { }
                // 3) Parent‑lerinde olabilir (çocuk collider modeli)
                else hp = col.GetComponentInParent<EnemyHealth>();
            }

            if (hp == null) continue;   // düşman değil

            hp.GetDamage(damage);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0f, 0f, 0.4f);
        Gizmos.DrawSphere(transform.position, radius);
    }
#endif
}

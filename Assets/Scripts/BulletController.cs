using UnityEngine;

public class BulletController : MonoBehaviour
{
    [Header("Bullet Settings")]
    [SerializeField] private float bulletLifeTime = 5f;
    private int damage = 10;

    private Vector3 originalScale;
    private bool isCharged;

    void Awake()
    {
        originalScale = transform.localScale;
    }

    void OnEnable()
    {
        // Deactivate the bullet after its lifetime expires
        Invoke(nameof(DeactivateBullet), bulletLifeTime);
    }

    void OnDisable()
    {
        // Important with pooling: cancel any pending Invoke
        // so a reused bullet isn't deactivated too early
        CancelInvoke();
    }

    // Called by PlayerShooting when the bullet is taken from the pool
    public void SetDamage(int newDamage)
    {
        damage = newDamage;
    }

    // Called by PlayerShooting right after SetDamage.
    // Always called, so a reused pooled bullet gets its normal size back.
    public void SetCharged(bool charged, float scaleMultiplier)
    {
        isCharged = charged;
        transform.localScale = charged ? originalScale * scaleMultiplier : originalScale;
    }

    private void DeactivateBullet()
    {
        // Return the bullet to the pool
        gameObject.SetActive(false);
    }

    private void EnemiesCollision(Collider enemyCollider)
    {
        // Look for the health component on the enemy that was hit
        EnemiesHealth enemyHealth = enemyCollider.GetComponent<EnemiesHealth>();

        if (enemyHealth != null)
        {
            // Apply damage and pass the impact position for the particles
            enemyHealth.TakeDamage(damage, transform.position);
        }

        // Normal bullets disappear on impact; the charged bullet goes through enemies
        if (!isCharged)
            DeactivateBullet();
    }

    void OnTriggerEnter(Collider other)
    {
        // Only react to objects tagged as "Enemies"
        if (other.CompareTag("Enemies"))
        {
            EnemiesCollision(other);
        }
    }
}
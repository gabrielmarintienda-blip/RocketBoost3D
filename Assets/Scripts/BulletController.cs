using UnityEngine;

public class BulletController : MonoBehaviour
{
    [Header("Bullet Settings")]
    [SerializeField] private float bulletLifeTime = 5f;
    private int damage = 10;

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

        // The bullet disappears on impact
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
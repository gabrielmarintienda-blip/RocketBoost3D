using DG.Tweening;
using UnityEngine;

public class EnemiesHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private EnemyData enemyData;
    private float currentHealth;

    [Header("Hit Feedback")]
    [SerializeField] private ParticleSystem hitParticlesPrefab;

    private void OnEnable()
    {
        currentHealth = enemyData.health;
    }

    // Called by the bullet when it hits the enemy
    public void TakeDamage(float damage, Vector3 hitPoint)
    {
        currentHealth -= damage;

        // Spawn hit particles at the impact point
        SpawnParticles(hitPoint);

        // Destroy the enemy when its health reaches zero
        if (currentHealth <= 0)
        {
            gameObject.SetActive(false);
        }
    }

    private void SpawnParticles(Vector3 position)
    {
        // Do nothing if no particle prefab has been assigned in the Inspector
        if (hitParticlesPrefab == null) return;

        // Instantiate the particles separately so they don't disappear if the enemy dies
        ParticleSystem ps = Instantiate(hitParticlesPrefab, position, Quaternion.identity);

        // Clean up the particle object once it has finished playing
        Destroy(ps.gameObject, ps.main.duration + ps.main.startLifetime.constantMax);
    }

    void OnDestroy()
    {
        // Kill any DOTween animation on this enemy to avoid "target is missing/null" warnings
        transform.DOKill();
    }
}
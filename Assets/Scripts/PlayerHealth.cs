using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int health = 100;
    public int currentHealth;

    [Header("Hit Feedback")]
    [SerializeField] private Material hitMaterial;
    [SerializeField] private float hitColorDuration = 0.5f;
    [SerializeField] private float invincibilityDuration = 1f;

    private bool isInvincible;
    private Renderer[] renderers;
    private Material[] originalMaterials;

    void Awake()
    {
        currentHealth = health;

        renderers = GetComponentsInChildren<Renderer>();
        originalMaterials = new Material[renderers.Length];

        // Save the original material of each renderer
        for (int i = 0; i < renderers.Length; i++)
            originalMaterials[i] = renderers[i].sharedMaterial;
    }

    internal void TakeDamage(int damage)
    {
        if (isInvincible) return;

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Debug.Log("Muerto");

            GetComponent<PlayerMovement>().enabled = false;
            return;
        }

        StartCoroutine(HitRoutine());
    }

    private IEnumerator HitRoutine()
    {
        isInvincible = true;

        // Swap to the red material (skipping particles)
        foreach (Renderer r in renderers)
            if (r is not ParticleSystemRenderer) r.sharedMaterial = hitMaterial;

        yield return new WaitForSeconds(hitColorDuration);

        // Restore the original materials
        for (int i = 0; i < renderers.Length; i++)
            if (renderers[i] is not ParticleSystemRenderer) renderers[i].sharedMaterial = originalMaterials[i];

        yield return new WaitForSeconds(invincibilityDuration - hitColorDuration);

        isInvincible = false;
    }
}
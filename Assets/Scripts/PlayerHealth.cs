using System.Collections;
using TMPro;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Lives")]
    [SerializeField] private int maxLives = 3;
    [SerializeField] private TextMeshProUGUI livesText;
    public int currentLives;

    [Header("Hit Feedback")]
    [SerializeField] private Material hitMaterial;
    [SerializeField] private float hitColorDuration = 0.5f;
    [SerializeField] private float invincibilityDuration = 1f;

    [Header("Death")]
    [SerializeField] private DeathScreen deathScreen;

    private bool isInvincible;
    private bool isDead;
    private Renderer[] renderers;
    private Material[] originalMaterials;

    public bool IsDead => isDead;

    void Awake()
    {
        currentLives = maxLives;

        renderers = GetComponentsInChildren<Renderer>();
        originalMaterials = new Material[renderers.Length];

        // Save the original material of each renderer
        for (int i = 0; i < renderers.Length; i++)
            originalMaterials[i] = renderers[i].sharedMaterial;
    }

    void Start()
    {
        UpdateLivesUI();
    }

    // Returns true only if the hit actually counted
    // (the damage value is ignored: every hit costs one life)
    internal bool TakeDamage(int damage)
    {
        if (isInvincible || isDead) return false;

        currentLives--;
        UpdateLivesUI();

        if (currentLives <= 0)
        {
            isDead = true;
            Debug.Log("Muerto");
            GetComponent<PlayerMovement>().enabled = false;

            if (deathScreen != null) deathScreen.Show();
            return true;
        }

        StartCoroutine(HitRoutine());
        return true;
    }

    public bool AddLife(int amount = 1)
    {
        if (isDead) return false;                  // muerto: no se puede curar
        if (currentLives >= maxLives) return false; // vidas completas: no se activa

        currentLives = Mathf.Min(currentLives + amount, maxLives);
        UpdateLivesUI();
        return true;
    }

    private void UpdateLivesUI()
    {
        if (livesText != null)
            livesText.text = "" + currentLives;
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
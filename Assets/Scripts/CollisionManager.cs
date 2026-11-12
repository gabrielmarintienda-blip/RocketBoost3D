using UnityEngine;
using UnityEngine.SceneManagement;

public class CollisionManager : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioClip explosionAudio;
    [SerializeField] private AudioClip landingAudio;

    [Header("Particles")]
    [SerializeField] private ParticleSystem explosionParticles;
    [SerializeField] private ParticleSystem landingParticles;

    [Header("UI")]
    [SerializeField] private VictoryScreen victoryScreen;

    AudioSource audioSource;
    PlayerHealth health;

    bool isControllable = true;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        health = GetComponent<PlayerHealth>();
    }

    void OnCollisionEnter(Collision collision)
    {
        // Ignore everything once the player is dead (no sounds while falling)
        if (!isControllable || health.IsDead) return;

        switch (collision.gameObject.tag)
        {
            case "Launchpad":
                Debug.Log("Launchpad");
                break;
            case "LandingPad":
                LandingPadCollision();
                break;
            case "Explosion":
                ExplosionCollision();
                break;
            case "Enemies":
                EnemiesCollision();
                break;
            case "Barrel":
                BarrelCollision(collision.gameObject);
                break;
        }
    }

    // Applies the hit and plays feedback only if the hit counted
    private void ApplyHit()
    {
        if (!health.TakeDamage(1)) return; // invincible or dead: no sound, no particles

        audioSource.PlayOneShot(explosionAudio);
        explosionParticles.Play();
    }

    public void ExplosionCollision()
    {
        Debug.Log("Explosion");
        ApplyHit();
    }

    private void BarrelCollision(GameObject barrel)
    {
        Debug.Log("Barrel");
        ApplyHit();
        Destroy(barrel);
    }

    private void EnemiesCollision()
    {
        Debug.Log("Enemies");
        ApplyHit();
    }

    private void LandingPadCollision()
    {
        Debug.Log("LandingPad");
        isControllable = false;
        audioSource.Stop();
        audioSource.PlayOneShot(landingAudio);
        landingParticles.Play();
        GetComponent<PlayerMovement>().enabled = false;

        if (victoryScreen != null) victoryScreen.Show();
    }
}
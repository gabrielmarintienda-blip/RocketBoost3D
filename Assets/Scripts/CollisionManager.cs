using UnityEngine;
using UnityEngine.SceneManagement;

public class CollisionManager : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioSource sfxSource;     // Solo efectos (explosión, aterrizaje). Nunca se llama a Stop() sobre él.
    [SerializeField] private AudioSource engineSource;  // El del propulsor (el mismo que use PlayerMovement). Es el único que se para.
    [SerializeField] private AudioClip explosionAudio;
    [SerializeField] private AudioClip landingAudio;

    [Header("Particles")]
    [SerializeField] private ParticleSystem explosionParticles;
    [SerializeField] private ParticleSystem landingParticles;

    [Header("UI")]
    [SerializeField] private VictoryScreen victoryScreen;

    PlayerHealth health;
    PlayerMovement movement;

    bool isControllable = true;

    void Start()
    {
        health = GetComponent<PlayerHealth>();
        movement = GetComponent<PlayerMovement>();

        // Si no asignas sfxSource en el Inspector, intenta usar el AudioSource del objeto
        if (sfxSource == null) sfxSource = GetComponent<AudioSource>();
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

    // Out of fuel + touching anything except the landing pad = lose
    void OnCollisionStay(Collision collision)
    {
        if (!isControllable || health.IsDead) return;

        if (movement.IsOutOfFuel && !collision.gameObject.CompareTag("LandingPad"))
        {
            Debug.Log("Sin combustible");
            isControllable = false;

            PlayExplosionSound();
            explosionParticles.Play();

            health.Die();
        }
    }

    // Applies the hit and plays feedback only if the hit counted
    private void ApplyHit()
    {
        if (!health.TakeDamage(1)) return; // invincible or dead: no sound, no particles

        PlayExplosionSound();
        explosionParticles.Play();
    }

    // Reproduce la explosión sin que se corte aunque el jugador se destruya o desactive
    private void PlayExplosionSound()
    {
        if (explosionAudio == null) return;

        // PlayClipAtPoint crea un objeto temporal, así el sonido sobrevive a Die()/Destroy
        // (pierdes el control del mixer/volumen del AudioSource; si lo necesitas, usa sfxSource.PlayOneShot)
        AudioSource.PlayClipAtPoint(explosionAudio, transform.position);
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

        // Solo se para el motor; los efectos que estén sonando no se tocan
        if (engineSource != null) engineSource.Stop();

        if (landingAudio != null) sfxSource.PlayOneShot(landingAudio);
        landingParticles.Play();
        movement.enabled = false;

        if (victoryScreen != null) victoryScreen.Show();
    }
}
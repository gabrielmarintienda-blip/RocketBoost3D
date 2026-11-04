using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class CollisionManager : MonoBehaviour
{
   [Header("Audio")]
   [SerializeField] private AudioClip explosionAudio;
   [SerializeField] private AudioClip landingAudio;

   [Header("Particles")]
   [SerializeField] private ParticleSystem explosionParticles;
   [SerializeField] private ParticleSystem landingParticles;

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
        if (!isControllable) return;
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
            default:

            break;
      }
   }

    public void ExplosionCollision()
    {
        Debug.Log("Explosion");
        if(health.currentHealth >= 0){
        isControllable = true;
        audioSource.PlayOneShot(explosionAudio);
        explosionParticles.Play();
        health.TakeDamage(50);
        }
        else
        {
            audioSource.Stop();
        }
    }
        private void BarrelCollision(GameObject barrel)
        {
            Debug.Log("Barrel");
            if (health.currentHealth >= 0)
            {
                isControllable = true;
                audioSource.PlayOneShot(explosionAudio);
                explosionParticles.Play();
                health.TakeDamage(50);
                Destroy(barrel);
            }
        }
    private void EnemiesCollision()
    {
        Debug.Log("Enemies");
        if(health.currentHealth >= 0){
        isControllable = true;
        audioSource.PlayOneShot(explosionAudio);
        explosionParticles.Play();
        health.TakeDamage(50);
        }
    }

    private void LandingPadCollision()
    {
        Debug.Log("LandingPad");
        isControllable = false;
        audioSource.Stop();
        audioSource.PlayOneShot(landingAudio);
        landingParticles.Play();
        GetComponent<PlayerMovement>().enabled = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    
    }
    private void ManageCollision(AudioClip audioClip, ParticleSystem particleSystem)
    {
        isControllable = false;
        audioSource.Stop();
        audioSource.PlayOneShot(audioClip);
        particleSystem.Play();
        GetComponent<PlayerMovement>().enabled = false;
    }
}

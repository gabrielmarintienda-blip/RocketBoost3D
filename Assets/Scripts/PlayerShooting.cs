using UnityEngine;
using TMPro;

public class PlayerShooting : MonoBehaviour
{
    [Header("Ammo Settings")]
    [SerializeField] private int currentAmmo = 10;
    [SerializeField] private int maxAmmo = 10;
    [SerializeField] private bool InfiniteAmmoDuration = false;
    [SerializeField] private TextMeshProUGUI ammoText;

    [Header("Shooting Settings")]
    [SerializeField] private float bulletSpeed = 20f;
    [SerializeField] private float bulletPerSecond = 0.25f;
    [SerializeField] public int bulletDamage = 10;

    [Header("Charged Shot")]
    [SerializeField] private float chargedBulletScale = 3f;
    [SerializeField] private int chargedBulletDamage = 9999;

    [Header("Ammo HUD")]
    [SerializeField] private GameObject[] ammoBars; // Left to right: the rightmost ones disappear first

    [Header("Audio")]
    [SerializeField] private AudioClip shootSound;
    [SerializeField] private AudioClip chargedShootSound; // Optional: falls back to shootSound if empty
    [SerializeField, Range(0f, 1f)] private float shootVolume = 0.7f;

    private Transform shootingPoint;
    private ObjectPool objectPool;
    private AudioSource audioSource;
    private float lastShotTime = 0f;

    void Start()
    {
        objectPool = GetComponent<ObjectPool>();
        shootingPoint = transform.Find("ShootPoint");

        // Use the existing AudioSource or create one if there is none
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f; // 2D sound, heard the same across the whole screen

        // Never start with more ammo than the maximum
        currentAmmo = Mathf.Min(currentAmmo, maxAmmo);
        UpdateAmmoUI();
    }

    // Checks if enough time has passed since the last shot and if there is ammo (or infinite ammo)
    public bool CanShoot()
    {
        bool cooldownReady = Time.time - lastShotTime >= bulletPerSecond;
        bool hasAmmo = InfiniteAmmoDuration || currentAmmo > 0;
        bool chargedReady = ScoreManager.Instance != null && ScoreManager.Instance.ChargedShotReady;

        return cooldownReady && (hasAmmo || chargedReady);
    }

    /// <summary>
    /// Adds ammo without exceeding maxAmmo. Returns true if any ammo was actually added.
    /// </summary>
    public bool AddAmmo(int amount)
    {
        // Already full: nothing to add
        if (currentAmmo >= maxAmmo) return false;

        currentAmmo = Mathf.Min(currentAmmo + amount, maxAmmo);
        UpdateAmmoUI();
        return true;
    }

    /// <summary>
    /// Shoots a bullet from the object pool if the player can shoot.
    /// </summary>
    public void Shoot()
    {
        if (!CanShoot()) return;

        // Get a bullet from the object pool
        GameObject bullet = objectPool.GetGameObjectFromPool();
        if (bullet == null) return; // Safety check, don't waste ammo if there is no bullet

        // Only consumed now that the shot really happens
        bool charged = ScoreManager.Instance != null && ScoreManager.Instance.ConsumeChargedShot();

        lastShotTime = Time.time;

        // Play the shoot sound (charged shots can have their own sound)
        AudioClip clip = (charged && chargedShootSound != null) ? chargedShootSound : shootSound;
        if (clip != null)
            audioSource.PlayOneShot(clip, shootVolume);

        // Charged shots are free: they don't spend ammo
        if (!charged && !InfiniteAmmoDuration)
        {
            currentAmmo--;
            UpdateAmmoUI();
        }

        bullet.transform.position = shootingPoint.position;
        bullet.transform.rotation = shootingPoint.rotation;

        BulletController bulletController = bullet.GetComponent<BulletController>();
        bulletController.SetDamage(charged ? chargedBulletDamage : bulletDamage);
        bulletController.SetCharged(charged, chargedBulletScale);

        // Set the bullet's velocity to shoot it forward
        Rigidbody bulletRigidbody = bullet.GetComponent<Rigidbody>();
        bulletRigidbody.linearVelocity = shootingPoint.up * bulletSpeed;
    }

    private void UpdateAmmoUI()
    {
        // Each bar is visible while its position is below the current ammo,
        // so the rightmost ones disappear first
        if (ammoBars != null)
        {
            for (int i = 0; i < ammoBars.Length; i++)
            {
                if (ammoBars[i] != null)
                    ammoBars[i].SetActive(InfiniteAmmoDuration || i < currentAmmo);
            }
        }

        if (ammoText == null) return;

        ammoText.text = InfiniteAmmoDuration
            ? "∞"
            : "" + currentAmmo + " / " + maxAmmo;
    }
}
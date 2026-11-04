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

    private Transform shootingPoint;
    private ObjectPool objectPool;
    private float lastShotTime = 0f;

    void Start()
    {
        objectPool = GetComponent<ObjectPool>();
        shootingPoint = transform.Find("ShootPoint");

        // Never start with more ammo than the maximum
        currentAmmo = Mathf.Min(currentAmmo, maxAmmo);
        UpdateAmmoUI();
    }

    // Checks if enough time has passed since the last shot and if there is ammo (or infinite ammo)
    public bool CanShoot()
    {
        return (Time.time - lastShotTime >= bulletPerSecond) && (InfiniteAmmoDuration || currentAmmo > 0);
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

        // Update the last shot time and reduce ammo
        lastShotTime = Time.time;
        if (!InfiniteAmmoDuration)
        {
            currentAmmo--;
            UpdateAmmoUI();
        }

        bullet.transform.position = shootingPoint.position;
        bullet.transform.rotation = shootingPoint.rotation;
        bullet.GetComponent<BulletController>().SetDamage(bulletDamage);

        // Set the bullet's velocity to shoot it forward
        Rigidbody bulletRigidbody = bullet.GetComponent<Rigidbody>();
        bulletRigidbody.linearVelocity = shootingPoint.up * bulletSpeed;
    }

    private void UpdateAmmoUI()
    {
        if (ammoText == null) return;

        ammoText.text = InfiniteAmmoDuration
            ? "∞"
            : "" + currentAmmo + " / " + maxAmmo;
    }
}
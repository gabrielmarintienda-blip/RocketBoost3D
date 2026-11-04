using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerMovement : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputAction thrust;
    [SerializeField] private InputAction rotation;
    [SerializeField] private InputAction shoot;

    [Header("Movement")]
    [SerializeField] private float moveForce = 10f;
    [SerializeField] private float rotationForce = 2.5f;

    [Header("Fuel")]
    [SerializeField] private float fuel = 100f;
    [SerializeField] private float maxFuel = 100f;
    [SerializeField] private float fuelConsumptionPerSecond = 5f; // Fuel spent per second of thrust
    [SerializeField] private TextMeshProUGUI fuelText;

    [Header("Particles")]
    [SerializeField] private ParticleSystem thrustParticles;
    [SerializeField] private ParticleSystem leftthrustParticles;
    [SerializeField] private ParticleSystem rightthrustParticles;

    [Header("Audio")]
    [SerializeField] private AudioClip thrustAudio;

    AudioSource audioSource;
    Rigidbody rb;
    PlayerShooting playerShooting;

    void OnEnable()
    {
        thrust.Enable();
        rotation.Enable();
        shoot.Enable();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        playerShooting = GetComponent<PlayerShooting>();

        // Never start with more fuel than the maximum
        fuel = Mathf.Min(fuel, maxFuel);
        UpdateFuelUI();
    }

    void FixedUpdate()
    {
        ProcessThurst();
        ProcessRotation();
        ProcessShooting();
    }

    private void ProcessRotation()
    {
        rb.freezeRotation = true; // Take manual control of rotation

        float rotationValue = rotation.ReadValue<float>();
        if (rotationValue < 0)
        {
            RotateRight();
        }
        else if (rotationValue > 0)
        {
            RotateLeft();
        }
        else
        {
            StopRotating();
        }
    }

    private void StopRotating()
    {
        leftthrustParticles.Stop();
        rightthrustParticles.Stop();
    }

    private void RotateLeft()
    {
        transform.Rotate(Vector3.back * rotationForce);
        if (!leftthrustParticles.isPlaying)
        {
            leftthrustParticles.Play();
            rightthrustParticles.Stop();
        }
    }

    private void RotateRight()
    {
        transform.Rotate(Vector3.forward * rotationForce);
        if (!rightthrustParticles.isPlaying)
        {
            rightthrustParticles.Play();
            leftthrustParticles.Stop();
        }
    }

    void ProcessThurst()
    {
        if (thrust.IsPressed())
        {
            StartThrusting();
        }
        else
        {
            StopThrusting();
        }
    }

    /// <summary>
    /// Applies thrust force, plays particles and audio, and consumes fuel over time.
    /// </summary>
    private void StartThrusting()
    {
        // No fuel left: behave as if the thrust key was released
        if (fuel <= 0)
        {
            StopThrusting();
            return;
        }

        rb.AddRelativeForce(Vector3.up * moveForce);

        if (!thrustParticles.isPlaying)
        {
            thrustParticles.Play();
        }
        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(thrustAudio);
        }

        // Spend fuel per second, not per frame
        fuel -= fuelConsumptionPerSecond * Time.fixedDeltaTime;
        fuel = Mathf.Max(fuel, 0f);
        UpdateFuelUI();
    }

    /// <summary>
    /// Stops the thrusting action, including stopping the particle effects and audio.
    /// </summary>
    private void StopThrusting()
    {
        thrustParticles.Stop();
        audioSource.Stop();
    }

    /// <summary>
    /// Adds fuel without exceeding maxFuel. Returns true if any fuel was actually added.
    /// </summary>
    public bool AddFuel(float amount)
    {
        // Already full: nothing to add
        if (fuel >= maxFuel) return false;

        fuel = Mathf.Min(fuel + amount, maxFuel);
        UpdateFuelUI();
        return true;
    }

    private void UpdateFuelUI()
    {
        if (fuelText == null) return;

        // CeilToInt avoids showing decimals (and shows 1 until it is really 0)
        fuelText.text = "" + Mathf.CeilToInt(fuel) + " / " + Mathf.CeilToInt(maxFuel);
    }

    private void ProcessShooting()
    {
        if (shoot.IsPressed())
        {
            playerShooting.Shoot();
        }
    }
}
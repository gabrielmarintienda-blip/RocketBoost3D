using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [Header("Score")]
    [SerializeField] private int pointsPerKill = 100;
    [SerializeField] private TextMeshProUGUI scoreText;

    [Header("Charged shot")]
    [SerializeField] private int killsForChargedShot = 5;
    [SerializeField] private TextMeshProUGUI chargeText;
    [SerializeField] private AudioClip chargeReadySound;

    public int Score { get; private set; }
    public bool ChargedShotReady { get; private set; }

    private int killsProgress;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        UpdateUI();
    }

    // Called by EnemiesHealth when an enemy dies
    public void RegisterKill()
    {
        Score += pointsPerKill;

        // While a charged shot is waiting, progress is paused (no stacking)
        if (!ChargedShotReady)
        {
            killsProgress++;

            if (killsProgress >= killsForChargedShot)
            {
                killsProgress = 0;
                ChargedShotReady = true;

                if (chargeReadySound != null && Camera.main != null)
                    AudioSource.PlayClipAtPoint(chargeReadySound, Camera.main.transform.position);
            }
        }

        UpdateUI();
    }

    // Called by PlayerShooting when the shot really happens.
    // Returns true if this shot must be the charged one.
    public bool ConsumeChargedShot()
    {
        if (!ChargedShotReady) return false;

        ChargedShotReady = false;
        UpdateUI();
        return true;
    }

    private void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = "" + Score;

        if (chargeText != null)
            chargeText.text = ChargedShotReady
                ? "¡DISPARO CARGADO LISTO!"
                : "Carga: " + killsProgress + "/" + killsForChargedShot;
    }
}
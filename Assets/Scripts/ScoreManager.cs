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
    [SerializeField] private GameObject[] killDiamonds; // Left to right

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

        UpdateDiamonds();
    }

    private void UpdateDiamonds()
    {
        if (killDiamonds == null) return;

        // While charging, one diamond per kill. When the shot is ready all of them stay lit
        // (progress resets to 0 at that point), and they all hide once the shot is used.
        int visible = ChargedShotReady ? killsForChargedShot : killsProgress;

        for (int i = 0; i < killDiamonds.Length; i++)
        {
            if (killDiamonds[i] != null)
                killDiamonds[i].SetActive(i < visible);
        }
    }
}
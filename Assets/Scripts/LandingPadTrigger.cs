using UnityEngine;

public class LandingPadTrigger : MonoBehaviour
{
    [SerializeField] private VictoryScreen victoryScreen;
    [SerializeField] private string playerTag = "Player";

    private bool triggered;

    void Awake()
    {
        // Fallback: find it in the scene if it wasn't assigned (includes inactive objects)
        if (victoryScreen == null)
            victoryScreen = FindFirstObjectByType<VictoryScreen>(FindObjectsInactive.Include);
    }

    // Used when the pad's collider has "Is Trigger" enabled
    void OnTriggerEnter(Collider other)
    {
        TryWin(other.gameObject);
    }

    // Used when the pad's collider is a regular (non-trigger) collider
    void OnCollisionEnter(Collision collision)
    {
        TryWin(collision.gameObject);
    }

    private void TryWin(GameObject other)
    {
        if (triggered) return;

        // Check the object and its root, in case the collider is on a child of the ship
        if (!other.CompareTag(playerTag) && !other.transform.root.CompareTag(playerTag)) return;

        triggered = true;
        victoryScreen.Show();
    }
}
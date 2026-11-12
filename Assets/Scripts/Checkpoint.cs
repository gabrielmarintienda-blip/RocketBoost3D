using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private Transform spawnPoint; // Optional: where the player will reappear

    private bool activated;

    void OnTriggerEnter(Collider other)
    {
        if (activated) return;
        if (other.GetComponentInParent<PlayerHealth>() == null) return;

        activated = true;
        Transform point = spawnPoint != null ? spawnPoint : transform;
        CheckpointData.Set(point.position, point.rotation);
        Debug.Log("Checkpoint guardado");
    }
}
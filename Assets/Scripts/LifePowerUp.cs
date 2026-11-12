using DG.Tweening;
using UnityEngine;

public class LifePowerUp : MonoBehaviour
{
    [SerializeField] private int lifeAmount = 1;

    void OnTriggerEnter(Collider other)
    {
        PlayerHealth player = other.GetComponentInParent<PlayerHealth>();
        if (player == null) return;

        if (player.AddLife(lifeAmount))
        {
            transform.DOKill();
            Destroy(gameObject);
        }
    }
}
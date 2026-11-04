using DG.Tweening;
using UnityEngine;

public class AmmoPowerUp : MonoBehaviour
{
    [SerializeField] private int ammoAmount = 5;

    void OnTriggerEnter(Collider other)
    {
        PlayerShooting player = other.GetComponentInParent<PlayerShooting>();
        if (player == null) return;

        if (player.AddAmmo(ammoAmount))
        {
            transform.DOKill();
            Destroy(gameObject);
        }
    }
}
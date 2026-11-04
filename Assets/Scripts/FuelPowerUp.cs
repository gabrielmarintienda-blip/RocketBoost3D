using DG.Tweening;
using UnityEngine;

public class FuelPowerUp : MonoBehaviour
{
    [SerializeField] private float fuelAmount = 25f;

    void OnTriggerEnter(Collider other)
    {
        PlayerMovement player = other.GetComponentInParent<PlayerMovement>();
        if (player == null) return;

        if (player.AddFuel(fuelAmount))
        {
            // Kill any DOTween animation on this object before destroying it
            transform.DOKill();
            Destroy(gameObject);
        }
    }
}
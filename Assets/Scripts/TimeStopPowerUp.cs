using DG.Tweening;
using UnityEngine;

public class TimeStopPowerUp : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<PlayerMovement>() == null) return;
        if (TimeStopManager.Instance == null) return;

        TimeStopManager.Instance.Activate();

        transform.DOKill();
        Destroy(gameObject);
    }
}
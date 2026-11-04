using UnityEngine;
using DG.Tweening;

public class SizeObstacle : MonoBehaviour
{
    [SerializeField] private float duration = 2f;
    [SerializeField] private float xSize = 0f;
    [SerializeField] private float ySize = 0f;
    [SerializeField] private float zSize = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.DOScale(new Vector3(xSize, ySize, zSize), duration)
        .SetEase(Ease.Linear)
        .SetLoops(-1, LoopType.Restart);
        
    }
}

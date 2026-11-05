using UnityEngine;
using DG.Tweening;
using System;

public class FlyingEnemy : MonoBehaviour
{
    [Header("Detection Settings")]
    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private float loseRange = 7f;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float floatingHeight = 1f;
    [SerializeField] private float floatingDuration = 2f;

    private Transform playerTransform;
    private Vector3 initialPosition;
    private Tween floatingTween;

    private enum EnemyState
    {
        Idle,
        Chasing,
        Returning
    }

    private EnemyState currentState = EnemyState.Idle;

    private void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        initialPosition = transform.position;

        // Start the floating animation
    StartFloating();
    }

    void Update()
    {
        if(playerTransform == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer <= detectionRange)
        {
            StartChasing(distanceToPlayer);

        }
        else if (currentState == EnemyState.Chasing && distanceToPlayer > loseRange)
        {
            // Return to initial position
            ReturnToInitialPosition();
        }
    }

    private void StartChasing(float distanceToPlayer)
    {
        currentState = EnemyState.Chasing;   
        floatingTween.Kill();

        float duration = Mathf.Max(0.1f, distanceToPlayer / moveSpeed);

        floatingTween = transform.DOMove(playerTransform.position, duration)
            .SetEase(Ease.Linear);
    }

    private void OnDrawGizmosSelected()
    {
        // Draw detection range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // Draw lose range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, loseRange);
    }

    private void ReturnToInitialPosition()
    {
        currentState = EnemyState.Returning;
        floatingTween.Kill();
        // Move towards the initial position
        float distanceToInitial = Vector3.Distance(transform.position, initialPosition);

        float duration = Mathf.Max(0.1f, distanceToInitial / moveSpeed);

        floatingTween = transform.DOMove(initialPosition, duration)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                currentState = EnemyState.Idle;
                StartFloating();
            });
    }

    private void StartFloating()
    {
        // Create a floating animation using DOTween
        floatingTween = transform.DOMoveY(initialPosition.y +
        floatingHeight, floatingDuration)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }
}

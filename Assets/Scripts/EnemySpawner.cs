using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Three enemy groups spawn from their pools once the ship passes startX.
// The area is an XY ellipse: the same Z as the player, less height than width.
// Enemies never spawn under the ground, behind the player, too close to the player
// or on top of another enemy, and they grow in instead of popping in.
public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class SpawnGroup
    {
        public ObjectPool enemyPool;
        public Transform spawnCenter;
        public Color tint = Color.white;
        public float startX;
        public int enemyCount;
        public float spawnInterval = 3f;

        [HideInInspector] public bool hasStarted;
    }

    [Header("Spawn Area")]
    [SerializeField] private float radius = 9f;
    [SerializeField] private float verticalRadius = 3.5f;
    [SerializeField] private float minSpawnHeight = 2f;
    [Tooltip("Minimum distance between a new enemy and any enemy that is already alive")]
    [SerializeField] private float minSeparation = 3.5f;

    [Header("Player Safety")]
    [Tooltip("Keep this above the enemies' detection range so they don't spawn already chasing")]
    [SerializeField] private float minDistanceFromPlayer = 8f;
    [Tooltip("Enemies only spawn at least this far IN FRONT of the player (to the right). Set it a bit beyond the screen edge")]
    [SerializeField] private float minAheadOfPlayer = 10f;
    [SerializeField] private int maxAttempts = 40;
    [SerializeField] private float retryDelay = 0.25f;

    [SerializeField] private List<SpawnGroup> spawnGroups;

    private Transform playerTransform;
    private readonly List<Vector3> activeEnemyPositions = new List<Vector3>();
    private readonly WaitUntil waitForTimeToRun = new WaitUntil(() => !TimeStopManager.IsStopped);

    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (playerTransform == null)
        {
            return;
        }

        foreach (var group in spawnGroups)
        {
            if (group.spawnCenter == null || group.hasStarted)
            {
                continue;
            }

            if (playerTransform.position.x >= group.startX)
            {
                group.hasStarted = true;
                StartCoroutine(SpawnEnemies(group));
            }
        }
    }

    private IEnumerator SpawnEnemies(SpawnGroup group)
    {
        for (int i = 0; i < group.enemyCount; i++)
        {
            // Wait for a valid spot: not while time is stopped, and not near the player
            Vector3 spawnPosition;
            while (true)
            {
                yield return waitForTimeToRun;

                if (TryPickSpawnPosition(group, out spawnPosition)) break;

                yield return new WaitForSeconds(retryDelay);
            }

            GameObject enemy = group.enemyPool.GetObjectFromPool(spawnPosition);
            ApplyTint(enemy, group.tint);

            // Resets the pooled enemy and plays its grow-in animation
            if (enemy.TryGetComponent(out FlyingEnemy flyingEnemy))
            {
                flyingEnemy.Spawn(spawnPosition);
            }

            yield return new WaitForSeconds(group.spawnInterval);
        }
    }

    // Returns false when no valid position was found (the caller tries again shortly)
    private bool TryPickSpawnPosition(SpawnGroup group, out Vector3 position)
    {
        Vector3 center = group.spawnCenter.position;
        // 2.5D game: the ellipse is XY and the depth matches the ship.
        center.z = playerTransform.position.z;

        // Positions of the enemies alive right now (collected once, not per attempt)
        activeEnemyPositions.Clear();
        FlyingEnemy[] enemies = FindObjectsByType<FlyingEnemy>(FindObjectsSortMode.None);
        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i].gameObject.activeInHierarchy)
            {
                activeEnemyPositions.Add(enemies[i].transform.position);
            }
        }

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            Vector2 offset = Random.insideUnitCircle;
            Vector3 candidate = center + new Vector3(offset.x * radius, offset.y * verticalRadius, 0f);

            // Never under the ground (discard instead of clamping, so they don't line up)
            if (candidate.y < minSpawnHeight)
            {
                continue;
            }

            // Never behind the player: only spawn ahead of the ship
            if (candidate.x < playerTransform.position.x + minAheadOfPlayer)
            {
                continue;
            }

            // Never close to the player
            if (Vector2.Distance(candidate, playerTransform.position) < minDistanceFromPlayer)
            {
                continue;
            }

            // Never on top of another enemy: skip spots that are too close to one
            if (DistanceToNearestEnemy(candidate) < minSeparation)
            {
                continue;
            }

            position = candidate;
            return true;
        }

        // No valid spot this time
        position = center;
        return false;
    }

    private float DistanceToNearestEnemy(Vector3 candidate)
    {
        float nearest = float.MaxValue;

        for (int i = 0; i < activeEnemyPositions.Count; i++)
        {
            nearest = Mathf.Min(nearest, Vector3.Distance(candidate, activeEnemyPositions[i]));
        }

        return nearest;
    }

    static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
    static readonly int ColorId = Shader.PropertyToID("_Color");

    // One tint per group, without duplicating the model's shared material.
    private static void ApplyTint(GameObject enemy, Color tint)
    {
        Renderer[] renderers = enemy.GetComponentsInChildren<Renderer>(true);
        MaterialPropertyBlock block = new MaterialPropertyBlock();

        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].GetPropertyBlock(block);
            block.SetColor(BaseColorId, tint);
            block.SetColor(ColorId, tint);
            renderers[i].SetPropertyBlock(block);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (spawnGroups == null)
        {
            return;
        }

        // Spawn ellipses
        Gizmos.color = Color.cyan;
        for (int groupIndex = 0; groupIndex < spawnGroups.Count; groupIndex++)
        {
            Transform center = spawnGroups[groupIndex].spawnCenter;
            if (center != null)
            {
                DrawEllipse(center.position, radius, verticalRadius);
            }
        }

        if (playerTransform != null)
        {
            // Safe zone around the player (visible in Play mode)
            Gizmos.color = Color.red;
            DrawEllipse(playerTransform.position, minDistanceFromPlayer, minDistanceFromPlayer);

            // Enemies only spawn to the right of this yellow line
            Gizmos.color = Color.yellow;
            float limitX = playerTransform.position.x + minAheadOfPlayer;
            Vector3 bottom = new Vector3(limitX, playerTransform.position.y - 10f, playerTransform.position.z);
            Vector3 top = new Vector3(limitX, playerTransform.position.y + 10f, playerTransform.position.z);
            Gizmos.DrawLine(bottom, top);
        }
    }

    private static void DrawEllipse(Vector3 center, float horizontal, float vertical)
    {
        const int segments = 40;
        Vector3 previous = center + new Vector3(horizontal, 0f, 0f);

        for (int i = 1; i <= segments; i++)
        {
            float angle = i / (float)segments * Mathf.PI * 2f;
            Vector3 next = center + new Vector3(Mathf.Cos(angle) * horizontal, Mathf.Sin(angle) * vertical, 0f);
            Gizmos.DrawLine(previous, next);
            previous = next;
        }
    }
}
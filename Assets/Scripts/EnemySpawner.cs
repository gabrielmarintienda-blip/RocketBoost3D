using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class SpawnGroup
    {
        public ObjectPool enemyPool;
        public Transform spawnPoint;
        public float startX;
        public int enemyCount;
        public float spawnInterval = 2f;

        [HideInInspector] public bool hasStarted;
    }


    [SerializeField] private List<SpawnGroup> spawnGroups;

    private Transform playerTransform;

    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }
    
    void Update()
    {
        foreach (var group in spawnGroups)
        {
            if (!group.hasStarted && playerTransform.position.x >= group.startX)
            {
                StartCoroutine(SpawnEnemies(group));
                group.hasStarted = true;
            }
        }
    }

    private IEnumerator SpawnEnemies(SpawnGroup group)
    {
        for (int i = 0; i < group.enemyCount; i++)
        {
            var enemy = group.enemyPool.GetGameObjectFromPool(group.spawnPoint.position, group.spawnPoint.rotation);
            yield return new WaitForSeconds(group.spawnInterval);
        }
    }
}

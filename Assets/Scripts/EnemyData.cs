using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName = "Enemy/EnemyData")]
public class EnemyData : ScriptableObject
{
    public string enemyName;
    public GameObject enemyPrefab;

    [Header("Stats")]
    public float health;
    public int damage;

}

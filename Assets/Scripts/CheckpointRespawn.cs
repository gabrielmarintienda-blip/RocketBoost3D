using UnityEngine;

public class CheckpointRespawn : MonoBehaviour
{
    void Start()
    {
        if (!CheckpointData.HasCheckpoint) return;

        transform.position = CheckpointData.Position;
        transform.rotation = CheckpointData.Rotation;
    }
}
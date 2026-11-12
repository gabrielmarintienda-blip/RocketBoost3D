using UnityEngine;

public static class CheckpointData
{
    public static bool HasCheckpoint;
    public static Vector3 Position;
    public static Quaternion Rotation;

    public static void Set(Vector3 position, Quaternion rotation)
    {
        HasCheckpoint = true;
        Position = position;
        Rotation = rotation;
    }

    public static void Clear()
    {
        HasCheckpoint = false;
    }
}
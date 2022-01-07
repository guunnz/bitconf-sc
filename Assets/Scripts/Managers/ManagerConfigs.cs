using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SegmentTypeConfig
{
    public Config.Types.Segment.SegmentType type;
    public Config.Types.MeshType meshType;
    public int minSegmentsToSpawn;
    public int maxSegmentsToSpawn;
    public int segmentsBeforeRespawn;
    public int decorationsToSpawn = 5;
    //public GameObject[] sideDecorationMeshes;
    //public GameObject[] cornerDecorationMeshes;
}

[System.Serializable]
public class DecorationConfig
{
    public Config.Types.MeshType meshType;
    public GameObject[] sideDecorationMeshes;
    public GameObject[] cornerDecorationMeshes;
}

[System.Serializable]
public class ObstacleTypeConfig
{
    //public Config.Types.Obstacle.ObstacleType type;
    public int obstaclesToSpawn = 5;
    public int meshesToSpawn = 5;
    public Obstacle prefab;
    public GameObject[] meshes;
    public ObstacleMeshConfig[] specificMeshes;
    public GameObject[] replaceMeshIfDestroyed;
    public ObstacleMeshConfig[] replaceSpecificMeshIfDestroyed;
}


[System.Serializable]
public class ObstacleMeshConfig
{
    public Config.Types.MeshType meshType;
    public GameObject[] meshes;
}

[System.Serializable]
public class MovingObstacleConfig
{
    public Config.Types.Obstacle.MovingObstacleSpeed speedType;
    public float speed = 5;
}

[System.Serializable]
public class CollectibleConfig
{
    public int collectiblesToSpawn;
    public Collectible prefab;
}

public class ObstacleMeshPoolConfig
{
    public Config.Types.Obstacle.ObstacleType obstacleType;
    public Config.Types.MeshType? meshType;
    public List<GameObject> meshes;
    public bool isReplaceMeshWhenDestroy;
}
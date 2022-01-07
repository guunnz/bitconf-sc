using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ObstacleManager : MonoBehaviour
{
    public List<MovingObstacleConfig> movingObstacleConfig;
    public List<ObstacleTypeConfig> obstacles;

    public static ObstacleManager instance { get; private set; }
    private int nextDestroyCheck = 1;

    private List<Obstacle> obstaclesAvailable;
    private List<Obstacle> obstaclesUsed;

    private List<ObstacleMeshPoolConfig> obstacleMeshesAvailable;
    private List<ObstacleMeshPoolConfig> obstacleMeshesUsed;

    private Transform cameraContainer;

    public void Init()
    {
        if (instance != null)
        {
            Debug.LogError("More than one ObstacleManager in scene");
            return;
        }
        instance = this;
        cameraContainer = Camera.main.transform;
        PoolObstaclesAndObstacleMeshes();
    }

    void Update()
    {

        if (Time.time >= nextDestroyCheck)
        {
            nextDestroyCheck = Mathf.FloorToInt(Time.time) + 1;
            DespawnReplacedObstacleMeshes();
        }
    }


    private void PoolObstaclesAndObstacleMeshes()
    {
        obstaclesAvailable = new List<Obstacle>();
        obstaclesUsed = new List<Obstacle>();
        obstacleMeshesAvailable = new List<ObstacleMeshPoolConfig>();
        obstacleMeshesUsed = new List<ObstacleMeshPoolConfig>();
        for (int i = 0; i < obstacles.Count; i++)
        {
            ObstacleTypeConfig obstacleTypeConfig = obstacles[i];

            PoolObstacle(obstacleTypeConfig, obstacleTypeConfig.obstaclesToSpawn);
            //PoolObstacleMesh(obstacleTypeConfig, obstacleTypeConfig.meshesToSpawn);
            PoolObstacleMesh(obstacleTypeConfig, 1);
        }
    }

    private void PoolObstacle(ObstacleTypeConfig obstacleTypeConfig, int poolCount)
    {
        for (int i = 0; i < poolCount; i++)
        {
            Obstacle obstacle = Instantiate(obstacleTypeConfig.prefab, this.transform);
            obstacle.gameObject.SetActive(false);
            obstaclesAvailable.Add(obstacle);
        }
    }

    private void PoolObstacleMesh(ObstacleTypeConfig obstacleTypeConfig, int poolCount)
    {

        AddObstacleMesh(obstacleTypeConfig.prefab.obstacleType, null, false);
        AddObstacleMesh(obstacleTypeConfig.prefab.obstacleType, null, true);

        for (int i = 0; i < obstacleTypeConfig.meshes.Length; i++)
        {
            for (int j = 0; j < poolCount; j++)
            {
                GameObject obstacleMesh = Instantiate(obstacleTypeConfig.meshes[i], this.transform);
                obstacleMesh.gameObject.SetActive(false);
                obstacleMeshesAvailable.Single(x => x.obstacleType == obstacleTypeConfig.prefab.obstacleType && x.meshType == null && !x.isReplaceMeshWhenDestroy).meshes.Add(obstacleMesh);
            }
        }

        for (int i = 0; i < obstacleTypeConfig.specificMeshes.Length; i++)
        {
            ObstacleMeshConfig meshConfig = obstacleTypeConfig.specificMeshes[i];

            AddObstacleMesh(obstacleTypeConfig.prefab.obstacleType, meshConfig.meshType, false);

            for (int j = 0; j < meshConfig.meshes.Length; j++)
            {
                for (int k = 0; k < poolCount; k++)
                {
                    GameObject obstacleMesh = Instantiate(meshConfig.meshes[j], this.transform);
                    obstacleMesh.gameObject.SetActive(false);
                    obstacleMeshesAvailable.Single(x => x.obstacleType == obstacleTypeConfig.prefab.obstacleType && x.meshType == meshConfig.meshType && !x.isReplaceMeshWhenDestroy).meshes.Add(obstacleMesh);
                }
            }
        }


        for (int i = 0; i < obstacleTypeConfig.replaceMeshIfDestroyed.Length; i++)
        {
            for (int j = 0; j < poolCount; j++)
            {
                GameObject obstacleMesh = Instantiate(obstacleTypeConfig.replaceMeshIfDestroyed[i], this.transform);
                obstacleMesh.gameObject.SetActive(false);
                obstacleMeshesAvailable.Single(x => x.obstacleType == obstacleTypeConfig.prefab.obstacleType && x.meshType == null && x.isReplaceMeshWhenDestroy).meshes.Add(obstacleMesh);
            }
        }

        for (int i = 0; i < obstacleTypeConfig.replaceSpecificMeshIfDestroyed.Length; i++)
        {
            ObstacleMeshConfig meshConfig = obstacleTypeConfig.replaceSpecificMeshIfDestroyed[i];
            AddObstacleMesh(obstacleTypeConfig.prefab.obstacleType, meshConfig.meshType, true);
            for (int j = 0; j < meshConfig.meshes.Length; j++)
            {
                for (int k = 0; k < poolCount; k++)
                {
                    GameObject obstacleMesh = Instantiate(meshConfig.meshes[j], this.transform);
                    obstacleMesh.gameObject.SetActive(false);
                    obstacleMeshesAvailable.Single(x => x.obstacleType == obstacleTypeConfig.prefab.obstacleType && x.meshType == meshConfig.meshType && x.isReplaceMeshWhenDestroy).meshes.Add(obstacleMesh);
                }
            }
        }

    }

    private void AddObstacleMesh(Config.Types.Obstacle.ObstacleType obstacleType, Config.Types.MeshType? meshType, bool isReplaceMeshWhenDestroy)
    {
        if (!obstacleMeshesAvailable.Any(x => x.obstacleType == obstacleType && x.meshType == meshType && x.isReplaceMeshWhenDestroy == isReplaceMeshWhenDestroy))
        {
            obstacleMeshesAvailable.Add(new ObstacleMeshPoolConfig()
            {
                obstacleType = obstacleType,
                meshType = meshType,
                meshes = new List<GameObject>(),
                isReplaceMeshWhenDestroy = isReplaceMeshWhenDestroy
            });
        }

        if (!obstacleMeshesUsed.Any(x => x.obstacleType == obstacleType && x.meshType == meshType && x.isReplaceMeshWhenDestroy == isReplaceMeshWhenDestroy))
        {
            obstacleMeshesUsed.Add(new ObstacleMeshPoolConfig()
            {
                obstacleType = obstacleType,
                meshType = meshType,
                meshes = new List<GameObject>(),
                isReplaceMeshWhenDestroy = isReplaceMeshWhenDestroy
            });
        }
    }

    public void SpawnObstacleMesh(Config.Types.Obstacle.ObstacleType obstacleType, Vector3 positionToSpawn, out GameObject obstacleMesh, out Config.Types.MeshType meshType)
    {
        try
        {
            Config.Types.MeshType currentMeshType = SegmentManager.instance.GetMeshType();
            meshType = currentMeshType;
            ObstacleMeshPoolConfig obstacleMeshPoolConfigAvailable = obstacleMeshesAvailable.FirstOrDefault(x => x.obstacleType == obstacleType && x.meshType == currentMeshType && !x.isReplaceMeshWhenDestroy);
            ObstacleMeshPoolConfig obstacleMeshPoolConfigUsed = obstacleMeshesUsed.FirstOrDefault(x => x.obstacleType == obstacleType && x.meshType == currentMeshType && !x.isReplaceMeshWhenDestroy);
            if (obstacleMeshPoolConfigAvailable == null)
            {
                obstacleMeshPoolConfigAvailable = obstacleMeshesAvailable.Single(x => x.obstacleType == obstacleType && x.meshType == null && !x.isReplaceMeshWhenDestroy);
                obstacleMeshPoolConfigUsed = obstacleMeshesUsed.FirstOrDefault(x => x.obstacleType == obstacleType && x.meshType == null && !x.isReplaceMeshWhenDestroy);
            }

            if (obstacleMeshPoolConfigAvailable.meshes.Count == 0)
            {
                ObstacleTypeConfig obstacleTypeConfig = obstacles.Single(x => x.prefab.obstacleType == obstacleType);
                PoolObstacleMesh(obstacleTypeConfig, 1);
                Debug.Log("NEW OBSTACLE MESH INSTANTIATED");
            }

            obstacleMesh = Helper.GetRandom<GameObject>(obstacleMeshPoolConfigAvailable.meshes);
            while (obstacleMeshPoolConfigUsed.meshes.IndexOf(obstacleMesh) > -1)
            {
                obstacleMeshPoolConfigAvailable.meshes.Remove(obstacleMesh);

                if (obstacleMeshPoolConfigAvailable.meshes.Count == 0)
                {
                    ObstacleTypeConfig obstacleTypeConfig = obstacles.Single(x => x.prefab.obstacleType == obstacleType);
                    PoolObstacleMesh(obstacleTypeConfig, 1);
                    Debug.Log("NEW OBSTACLE MESH INSTANTIATED");
                }

                obstacleMesh = Helper.GetRandom<GameObject>(obstacleMeshPoolConfigAvailable.meshes);
            }


            obstacleMeshPoolConfigAvailable.meshes.Remove(obstacleMesh);
            obstacleMeshPoolConfigUsed.meshes.Add(obstacleMesh);

            obstacleMesh.transform.position = positionToSpawn;
            obstacleMesh.gameObject.SetActive(true);
        }
        catch (System.Exception ex)
        {
            Debug.Log(obstacleType);
            string a = ex.Message;
            throw ex;
        }
    }

    public void DespanwObstacleMesh(Config.Types.Obstacle.ObstacleType obstacleType, GameObject obstacleMesh, Config.Types.MeshType meshType)
    {
        if (obstacleMesh == null)
        {
            return;
        }
        ObstacleMeshPoolConfig obstacleMeshPoolConfigAvailable = obstacleMeshesAvailable.FirstOrDefault(x => x.obstacleType == obstacleType && x.meshType == meshType && !x.isReplaceMeshWhenDestroy);
        ObstacleMeshPoolConfig obstacleMeshPoolConfigUsed = obstacleMeshesUsed.FirstOrDefault(x => x.obstacleType == obstacleType && x.meshType == meshType && !x.isReplaceMeshWhenDestroy);
        if (obstacleMeshPoolConfigAvailable == null)
        {
            obstacleMeshPoolConfigAvailable = obstacleMeshesAvailable.Single(x => x.obstacleType == obstacleType && x.meshType == null && !x.isReplaceMeshWhenDestroy);
            obstacleMeshPoolConfigUsed = obstacleMeshesUsed.FirstOrDefault(x => x.obstacleType == obstacleType && x.meshType == null && !x.isReplaceMeshWhenDestroy);
        }

        obstacleMeshPoolConfigUsed.meshes.Remove(obstacleMesh);
        obstacleMeshPoolConfigAvailable.meshes.Add(obstacleMesh);

        obstacleMesh.SetActive(false);
    }

    public void SpawnReplaceObstacleMesh(Config.Types.Obstacle.ObstacleType obstacleType, Vector3 positionToSpawn, Config.Types.MeshType meshType)
    {
        ObstacleTypeConfig obstacleTypeConfig = obstacles.FirstOrDefault(x => x.prefab.obstacleType == obstacleType);
        if (obstacleTypeConfig.replaceMeshIfDestroyed.Length == 0 && !obstacleTypeConfig.replaceSpecificMeshIfDestroyed.Any(x => x.meshType == meshType))
        {
            return;
        }

        ObstacleMeshPoolConfig obstacleMeshPoolConfigAvailable = obstacleMeshesAvailable.FirstOrDefault(x => x.obstacleType == obstacleType && x.meshType == meshType && x.isReplaceMeshWhenDestroy);
        ObstacleMeshPoolConfig obstacleMeshPoolConfigUsed = obstacleMeshesUsed.FirstOrDefault(x => x.obstacleType == obstacleType && x.meshType == meshType && x.isReplaceMeshWhenDestroy);
        if (obstacleMeshPoolConfigAvailable == null)
        {
            obstacleMeshPoolConfigAvailable = obstacleMeshesAvailable.Single(x => x.obstacleType == obstacleType && x.meshType == null && x.isReplaceMeshWhenDestroy);
            obstacleMeshPoolConfigUsed = obstacleMeshesUsed.FirstOrDefault(x => x.obstacleType == obstacleType && x.meshType == null && x.isReplaceMeshWhenDestroy);
        }

        if (obstacleMeshPoolConfigAvailable.meshes.Count == 0)
        {
            PoolObstacleMesh(obstacleTypeConfig, 1);
            Debug.Log("NEW OBSTACLE MESH INSTANTIATED");
        }

        GameObject obstacleMesh = Helper.GetRandom<GameObject>(obstacleMeshPoolConfigAvailable.meshes);
        obstacleMeshPoolConfigAvailable.meshes.Remove(obstacleMesh);
        obstacleMeshPoolConfigUsed.meshes.Add(obstacleMesh);

        obstacleMesh.transform.position = positionToSpawn;
        obstacleMesh.gameObject.SetActive(true);

    }

    private void DespawnReplacedObstacleMeshes()
    {
        IEnumerable<ObstacleMeshPoolConfig> obstacleMeshesUsedToRemove = obstacleMeshesUsed.Where(x => x.isReplaceMeshWhenDestroy && x.meshes.Any());
        for (int i = 0; i < obstacleMeshesUsedToRemove.Count(); i++)
        {
            ObstacleMeshPoolConfig obstacleMeshPoolConfig = obstacleMeshesUsedToRemove.ElementAt(i);
            for (int j = 0; j < obstacleMeshPoolConfig.meshes.Count(); j++)
            {
                GameObject mesh = obstacleMeshPoolConfig.meshes[j];
                if (mesh.transform.position.z + mesh.transform.localScale.z < cameraContainer.position.z)
                {
                    mesh.SetActive(false);
                    obstacleMeshPoolConfig.meshes.Remove(mesh);
                }
            }
        }
    }


    public Obstacle SpawnObstacle(Config.Types.Obstacle.ObstacleType[] obstacleTypes, bool canBlock, Vector3 positionToSpawn)
    {
        IEnumerable<Obstacle> possibleObstacles = obstaclesAvailable.Where(x => obstacleTypes.Contains(x.obstacleType) && (canBlock || !x.completeBlock));
        if (possibleObstacles.Count() == 0)
        {
            for (int i = 0; i < obstacleTypes.Length; i++)
            {
                ObstacleTypeConfig obstacleTypeConfig = obstacles.Single(x => x.prefab.obstacleType == obstacleTypes[i]);
                PoolObstacle(obstacleTypeConfig, 1);
            }
            possibleObstacles = obstaclesAvailable.Where(x => obstacleTypes.Contains(x.obstacleType) && (canBlock || !x.completeBlock));
            Debug.Log("NEW OBSTACLE INSTANTIATED: " + string.Join(", ", obstacleTypes));
        }
        Obstacle obstacleToSpawn = Helper.GetRandom(possibleObstacles);
        obstaclesUsed.Add(obstacleToSpawn);
        obstaclesAvailable.Remove(obstacleToSpawn);
        obstacleToSpawn.transform.position = positionToSpawn;
        obstacleToSpawn.gameObject.SetActive(true);
        return obstacleToSpawn;
    }


    public void DespawnObstacles(List<Obstacle> obstacles)
    {
        if (obstacles == null)
        {
            return;
        }
        for (int i = 0; i < obstacles.Count; i++)
        {
            obstacles[i].gameObject.SetActive(false);
            obstaclesUsed.Remove(obstacles[i]);
            obstaclesAvailable.Add(obstacles[i]);
        }
    }

    public float GetObstacleSpeed(Config.Types.Obstacle.MovingObstacleSpeed speedType)
    {
        if (speedType == Config.Types.Obstacle.MovingObstacleSpeed.None)
        {
            return 0;
        }
        return movingObstacleConfig.Single(x => x.speedType == speedType).speed;
    }

}

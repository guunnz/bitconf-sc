using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RandowmObstacleSpawner : MonoBehaviour
{

    [Header("General")]
    public float spawnDistance = 60;
    public Collider triggerColliderFast;
    public Collider triggerColliderSlow;

    [Header("LanesConfig")]
    public RandomObstacleLaneConfig[] laneConfigs;

    private List<Obstacle> usedObstaclesFast;
    private List<Obstacle> usedObstaclesSlow;
    private List<Obstacle> usedObstacles;
    private int blockingPieces = 0;
    Config.Types.Lane nonBlockingLane;

    private List<Config.Types.Lane> spawnedLanes;

    private bool isStart = true;

    private void Awake()
    {
        usedObstacles = new List<Obstacle>();
        usedObstaclesFast = new List<Obstacle>();
        usedObstaclesSlow = new List<Obstacle>();
        spawnedLanes = new List<Config.Types.Lane>();
    }

    private void OnEnable()
    {
        if (isStart)
        {
            isStart = false;
            return;
        }
        SetCollidersPosition(triggerColliderFast, Config.Types.Obstacle.MovingObstacleSpeed.Fast);
        SetCollidersPosition(triggerColliderSlow, Config.Types.Obstacle.MovingObstacleSpeed.Slow);
        SpawnObstacles();
    }

    private void OnDisable()
    {
        DespawnObstacles();
    }

    public void DespawnObstacles()
    {
        if (usedObstaclesFast != null)
        {
            ObstacleManager.instance.DespawnObstacles(usedObstaclesFast);
            usedObstaclesFast.Clear();
        }
        if (usedObstaclesSlow != null)
        {
            ObstacleManager.instance.DespawnObstacles(usedObstaclesSlow);
            usedObstaclesSlow.Clear();
        }
        if (usedObstacles != null)
        {
            ObstacleManager.instance.DespawnObstacles(usedObstacles);
            usedObstacles.Clear();
        }
    }

    private void SetCollidersPosition(Collider collider, Config.Types.Obstacle.MovingObstacleSpeed speedType)
    {
        float timeForObstacleToReach = GetTimeForObstacleToReach(speedType);
        float playerPositionWhenStart = GetPlayerPositionWhenStart(timeForObstacleToReach);
        collider.transform.position = transform.position - new Vector3(0, 0, playerPositionWhenStart);
    }

    private float GetTimeForObstacleToReach(Config.Types.Obstacle.MovingObstacleSpeed speedType)
    {
        float speed = ObstacleManager.instance.GetObstacleSpeed(speedType);
        return spawnDistance / speed;
    }

    private float GetPlayerPositionWhenStart(float timeBeforeReach)
    {
        float playerSpeed = PlayerSpeed.instance.GetCurrentSpeed();
        return playerSpeed * timeBeforeReach;
    }

    public void GetLanes(out List<Config.Types.Lane> lanes, out Config.Types.Lane nonBlockingLane)
    {
        lanes = new List<Config.Types.Lane>() { Config.Types.Lane.Left, Config.Types.Lane.Center, Config.Types.Lane.Right };
        int rand = Random.Range(0, lanes.Count);
        nonBlockingLane = lanes[rand];
        lanes.RemoveAt(rand);
        lanes.Add(Config.Types.Lane.BetweenLeftCenter);
        lanes.Add(Config.Types.Lane.BetweenRightCenter);
        lanes = lanes.OrderBy(x => System.Guid.NewGuid()).ToList();
    }

    private void SpawnObstacles()
    {
        spawnedLanes.Clear();
        blockingPieces = 0;

        foreach (RandomObstacleLaneConfig laneConfig in laneConfigs.OrderBy(x => System.Guid.NewGuid()))
        {
            if (laneConfig.lanes != null && laneConfig.lanes.Length > 0)
            {
                IEnumerable<Config.Types.Lane> lanes = laneConfig.lanes.Where(x => !spawnedLanes.Contains(x));
                if (lanes.Count() == 0)
                {
                    continue;
                }
                Config.Types.Lane lane = Helper.GetRandom<Config.Types.Lane>(lanes);
                float positionX = Config.Lane.laneDistance * (int)lane / 100;
                SpawnObstacle(lane, laneConfig.obstacles, laneConfig.speeds, laneConfig.chancesToSpawn, positionX);
            }
        }
    }


    private void SpawnObstacle(Config.Types.Lane lane, Config.Types.Obstacle.ObstacleType[] obstaclesTypes, Config.Types.Obstacle.MovingObstacleSpeed[] speedTypes, int chancesToSpawn, float positionX)
    {
        if (obstaclesTypes == null || chancesToSpawn == 0 || obstaclesTypes.Length == 0)
        {
            return;
        }

        int rand = Random.Range(1, 100);
        if (rand > chancesToSpawn)
        {
            return;
        }

        Vector3 positionToSpawn = transform.position;
        positionToSpawn.x = positionX;

        Obstacle obstacleToSpawn = ObstacleManager.instance.SpawnObstacle(obstaclesTypes, blockingPieces <= 2, positionToSpawn);

        if (speedTypes != null && speedTypes.Length > 0 && obstacleToSpawn.canMove)
        {
            Config.Types.Obstacle.MovingObstacleSpeed speedType = Helper.GetRandom(speedTypes);
            obstacleToSpawn.SetSpeed(ObstacleManager.instance.GetObstacleSpeed(speedType));
            switch (speedType)
            {
                case Config.Types.Obstacle.MovingObstacleSpeed.Fast:
                    obstacleToSpawn.transform.position = positionToSpawn + new Vector3(0, 0, spawnDistance);
                    usedObstaclesFast.Add(obstacleToSpawn);
                    break;
                case Config.Types.Obstacle.MovingObstacleSpeed.Slow:
                    obstacleToSpawn.transform.position = positionToSpawn + new Vector3(0, 0, spawnDistance);
                    positionToSpawn.z += spawnDistance;
                    usedObstaclesSlow.Add(obstacleToSpawn);
                    break;
                case Config.Types.Obstacle.MovingObstacleSpeed.None:
                    usedObstacles.Add(obstacleToSpawn);
                    break;
            }
        }
        else
        {
            usedObstacles.Add(obstacleToSpawn);
        }
        blockingPieces += obstacleToSpawn.completeBlock ? 1 : 0;
        spawnedLanes.Add(lane);
    }

    public void TriggerEntered(Config.Types.Obstacle.MovingObstacleSpeed speedType)
    {
        switch (speedType)
        {
            case Config.Types.Obstacle.MovingObstacleSpeed.Fast:
                StartMoving(usedObstaclesFast);
                break;
            case Config.Types.Obstacle.MovingObstacleSpeed.Slow:
                StartMoving(usedObstaclesSlow);
                break;
        }
    }

    private void StartMoving(List<Obstacle> obstaclesToMove)
    {
        if (obstaclesToMove == null || obstaclesToMove.Count == 0)
        {
            return;
        }
        for (int i = 0; i < obstaclesToMove.Count; i++)
        {
            obstaclesToMove[i].StartMovement();
        }
    }
}

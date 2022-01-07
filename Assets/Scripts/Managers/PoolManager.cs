using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{

    public ObstacleManager obstacleManager;
    public FloorManager floorManager;
    public SegmentManager segmentManager;
    public CollectibleManager collectibleManager;
    public DecorationManager decorationManager;

    private void Awake()
    {
        decorationManager.Init();
        obstacleManager.Init();
        floorManager.Init();
        segmentManager.Init();
        collectibleManager.Init();
    }
}

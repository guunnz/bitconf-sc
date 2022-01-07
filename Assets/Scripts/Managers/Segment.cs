using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Segment : MonoBehaviour
{

    public Config.Types.Segment.SegmentLength length;
    public List<Config.Types.Segment.SegmentType> segmentStartType;
    public List<Config.Types.Segment.SegmentType> segmentEndType;
    public bool canStart;
    public bool spawnSpecialFloor;

    [HideInInspector]
    public Obstacle[] obstaclesInSegment;
    [HideInInspector]
    public Collectible[] collectiblesInSegment;

    [HideInInspector]
    private List<FloorMeshConfig> floorMeshes; //Guardo los posibles floors para no tener que calcularlos cada vez que spawnea. Publica para que copie el valor al generar las instancias


    [HideInInspector]
    public Config.Types.Segment.SegmentType startSpawnType;
    [HideInInspector]
    public Config.Types.Segment.SegmentType endSpawnType;

    private FloorMeshConfig currentFloorMesh;

    internal Config.Types.Segment.SegmentType lastTypeSpawned;

    private bool IsStart = true;

    private void Awake()
    {
        //TODO: ES VALIDO ESTO O CONSUME?
        obstaclesInSegment = GetComponentsInChildren<Obstacle>(true);
        collectiblesInSegment = GetComponentsInChildren<Collectible>(true);
        if (floorMeshes == null || floorMeshes.Count == 0)
        {
            PoolFloorMeshes();
        }
    }

    private void Start()
    {

    }

    private void OnEnable()
    {
        if (IsStart)
        {
            IsStart = false;
            return;
        }
        SpawnFloor();

        if (!PlayerBossBehaviour.instance.BossfightStarted)
        {
            for (int i = 0; i < obstaclesInSegment.Length; i++)
            {
                obstaclesInSegment[i].gameObject.SetActive(true);
            }



            for (int i = 0; i < collectiblesInSegment.Length; i++)
            {
                collectiblesInSegment[i].gameObject.SetActive(true);
            }
        }
    }

    private void OnDisable()
    {
        FloorManager.instance.DespawnFloor(currentFloorMesh);
    }

    private void PoolFloorMeshes()
    {
        floorMeshes = FloorManager.instance.GetPosibleFloors(this);
    }

    private void SpawnFloor()
    {
        if (floorMeshes.Count == 0)
        {
            return;
        }
        currentFloorMesh = FloorManager.instance.GetFloorMeshToSpawn(floorMeshes, startSpawnType, endSpawnType, this.transform.position);


        //currentFloorMesh.transform.eulerAngles = new Vector3(currentFloorMesh.transform.eulerAngles.x, PlayerMotor.instance.Reverse ? 180 : 0, currentFloorMesh.transform.eulerAngles.z);

        //if (PlayerMotor.instance.Reverse)
        //{

        //}
    }



}

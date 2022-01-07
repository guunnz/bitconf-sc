using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FloorManager : MonoBehaviour
{
    public int floorsMeshesToPool = 3;
    public FloorMeshConfig[] floors;
    public static FloorManager instance { get; private set; }

    private List<FloorMeshConfig> floorsAvailable;
    private List<FloorMeshConfig> floorsUsed;


    public void Init()
    {
        if (instance != null)
        {
            Debug.LogError("More than one SegmentSpawner in scene");
            return;
        }
        instance = this;
        PoolFloors();
    }

    private void PoolFloors()
    {
        floorsAvailable = new List<FloorMeshConfig>();
        floorsUsed = new List<FloorMeshConfig>();
        for (int i = 0; i < floors.Length; i++)
        {
            PoolFloors(floors[i], floorsMeshesToPool);
        }
    }

    private void PoolFloors(FloorMeshConfig floorMeshConfig, int countToSpawn)
    {
        for (int i = 0; i < countToSpawn; i++)
        {
            FloorMeshConfig floor = Instantiate(floorMeshConfig, this.transform);
            floor.gameObject.SetActive(false);
            floorsAvailable.Add(floor);
        }
    }

    public List<FloorMeshConfig> GetPosibleFloors(Segment segment)
    {
        List<FloorMeshConfig> values = floorsAvailable.ToList();

        values.AddRange(floorsUsed);
        List<FloorMeshConfig> possibleFloors = GetPosibleFloors(segment, floorsAvailable);
        possibleFloors.AddRange(GetPosibleFloors(segment, floorsUsed));

        if (possibleFloors.Count == 0)
        {
            Debug.LogError("FloorMeshConfig no disponible para segmento " + segment.name);
            return null;
        }
        return possibleFloors;
    }

    private List<FloorMeshConfig> GetPosibleFloors(Segment segment, List<FloorMeshConfig> floors)
    {
        List<FloorMeshConfig> possibleFloors = new List<FloorMeshConfig>();
        for (int i = 0; i < floors.Count(); i++)
        {
            FloorMeshConfig floorSpawner = floorsAvailable.ElementAt(i);
            if (floorSpawner.length != segment.length || floorSpawner.specialFloor != segment.spawnSpecialFloor)
            {
                continue;
            }

            Config.Types.Segment.SegmentType startType = SegmentManager.instance.segmentTypes.Single(x => x.meshType == floorSpawner.meshTypeStart).type;
            if (!segment.segmentStartType.Contains(startType))
            {
                continue;
            }

            Config.Types.Segment.SegmentType endType = SegmentManager.instance.segmentTypes.Single(x => x.meshType == floorSpawner.meshTypeEnd).type;

            if ((segment.segmentEndType.Count == 0 && startType == endType) || segment.segmentEndType.Contains(endType))
            {
                possibleFloors.Add(floorSpawner);
            }
        }
        return possibleFloors;
    }

    private FloorMeshConfig GetFloorToPool(List<FloorMeshConfig> floorSpawners, Config.Types.Segment.SegmentType segmentStartType, Config.Types.Segment.SegmentType segmentEndType)
    {
        IEnumerable<FloorMeshConfig> possibleFloors = floorsUsed;//.Where(x => floorSpawners.Contains(x));

        IEnumerable<FloorMeshConfig> possibleFloorsBad = floorsUsed.Where(x => floorSpawners.Contains(x));

        possibleFloors = possibleFloors.Where(x => segmentStartType == SegmentManager.instance.segmentTypes.Single(y => y.meshType == x.meshTypeStart).type);
        possibleFloors = possibleFloors.Where(x => segmentEndType == SegmentManager.instance.segmentTypes.Single(y => y.meshType == x.meshTypeEnd).type);


        //IEnumerable <FloorMeshConfig> possibleFloors = floorsUsed.Where(x => floorSpawners.Contains(x) &&
        //      segmentStartType == SegmentManager.instance.segmentTypes.Single(y => y.meshType == x.meshTypeStart).type &&
        //      segmentEndType == SegmentManager.instance.segmentTypes.Single(y => y.meshType == x.meshTypeEnd).type
        //   );
        FloorMeshConfig floorToClone = Helper.GetRandom(possibleFloors);
        FloorMeshConfig newFloor = Instantiate(floorToClone, this.transform);
        newFloor.gameObject.SetActive(false);
        floorsAvailable.Add(newFloor);
        return newFloor;
    }

    public FloorMeshConfig GetFloorMeshToSpawn(List<FloorMeshConfig> floorSpawners, Config.Types.Segment.SegmentType segmentStartType, Config.Types.Segment.SegmentType segmentEndType, Vector3 positionToSpawn)
    {
        IEnumerable<FloorMeshConfig> possibleFloors = floorsAvailable.Where(x => floorSpawners.Contains(x) &&
               segmentStartType == SegmentManager.instance.segmentTypes.Single(y => y.meshType == x.meshTypeStart).type &&
            segmentEndType == SegmentManager.instance.segmentTypes.Single(y => y.meshType == x.meshTypeEnd).type
        );
        FloorMeshConfig floorToSpawn;
        if (possibleFloors.Count() == 0)
        {
            floorToSpawn = GetFloorToPool(floorSpawners, segmentStartType, segmentEndType);
            //floorSpawners.Add(floorToSpawn);
            Debug.Log("NEW FLOOR INSTANTIATED");
        }
        else
        {
            floorToSpawn = Helper.GetRandom(possibleFloors);
        }

        floorsAvailable.Remove(floorToSpawn);
        floorsUsed.Add(floorToSpawn);
        floorToSpawn.transform.position = positionToSpawn;
        floorToSpawn.gameObject.SetActive(true);
        return floorToSpawn;
    }


    public void DespawnFloor(FloorMeshConfig floor)
    {
        if (floor == null)
        {
            return;
        }
        floor.gameObject.SetActive(false);
        floorsUsed.Remove(floor);
        floorsAvailable.Add(floor);
    }
}

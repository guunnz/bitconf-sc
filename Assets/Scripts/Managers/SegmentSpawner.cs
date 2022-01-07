using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class SegmentSpawner : MonoBehaviour
{

    [Header("GENERAL")]
    public int distanceBeforeSpawn = 30;
    public Transform startSpawnPoint;
    public Config.Types.Segment.SegmentType spawnType;
    public ObstacleRemover obstacleRemover;

    private Transform cameraContainer;
    private List<Segment> segmentsSpawned;

    private float nextSpawnPositionZ;
    private int countBeforeRepeat;
    private int nextDestroyCheck = 1;
    private int segmentSameTypeSpawned = 0;
    private int segmentsSameTypeToSpawn = 0;
    private int backSpawnPos;


    public static SegmentSpawner instance { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("More than one SegmentSpawner in scene");
            return;
        }
        instance = this;
        cameraContainer = Camera.main.transform;
        nextSpawnPositionZ = startSpawnPoint.position.z;
        obstacleRemover.gameObject.SetActive(false);
        segmentsSpawned = new List<Segment>();
    }

    private void Start()
    {
        SpawnSegment(true);
    }

    private void Update()
    {
        //if (nextSpawnPositionZ - cameraContainer.position.z < distanceBeforeSpawn && !PlayerMotor.instance.Reverse)
        //{
        //    SpawnSegment(false);
        //}
        if (Math.Abs(nextSpawnPositionZ - cameraContainer.position.z) < distanceBeforeSpawn/* && PlayerMotor.instance.Reverse*/)
        {
            SpawnSegment(false);
        }

        if (Time.time >= nextDestroyCheck)
        {
            nextDestroyCheck = Mathf.FloorToInt(Time.time) + 1;
            DespawnSegment();
        }
    }

    internal void SpawnSegment(bool isStart)
    {

        if (PlayerMotor.instance.Reverse)
        {
            if (segmentSameTypeSpawned == 0)
            {
                SegmentTypeConfig segmentType = SegmentManager.instance.segmentTypes.Single(x => x.type == spawnType);
                segmentsSameTypeToSpawn = UnityEngine.Random.Range(segmentType.minSegmentsToSpawn, segmentType.maxSegmentsToSpawn + 1);
                countBeforeRepeat = segmentType.segmentsBeforeRespawn;
            }

            Segment segment = SegmentManager.instance.GetSegmentToSpawn(spawnType, false, segmentSameTypeSpawned >= segmentsSameTypeToSpawn, countBeforeRepeat, 50, spawnBasedOnEndType: true);

            Config.Types.Segment.SegmentType nextSpawnType = spawnType;

            if (segmentSameTypeSpawned >= segmentsSameTypeToSpawn)
            {
                if (segment.segmentStartType.Any(x => SegmentManager.instance.GetReverseChangingSegment(spawnType).Contains(x)))
                {
                    List<Config.Types.Segment.SegmentType> startSegmentTypes;
                    startSegmentTypes = segment.segmentStartType.Where(x => x != spawnType).ToList();
                    if (startSegmentTypes.Count != 0)
                    {
                        nextSpawnType = Helper.GetRandom(startSegmentTypes);
                        segmentSameTypeSpawned = 0;
                    }
                    else
                    {
                        segmentSameTypeSpawned++;
                    }
                }
                else
                {
                    segmentSameTypeSpawned++;
                }
            }
            else
            {
                segmentSameTypeSpawned++;
            }


            segment.transform.position = new Vector3(0, 0, nextSpawnPositionZ);


            segment.startSpawnType = nextSpawnType;
            segment.endSpawnType = spawnType;
            segment.gameObject.SetActive(true);


            nextSpawnPositionZ += PlayerMotor.instance.Reverse ? -(int)segment.length : (int)segment.length;


            spawnType = nextSpawnType;
            segmentsSpawned.Add(segment);
        }
        else
        {
            if (segmentSameTypeSpawned == 0)
            {
                SegmentTypeConfig segmentType = SegmentManager.instance.segmentTypes.Single(x => x.type == spawnType);
                segmentsSameTypeToSpawn = UnityEngine.Random.Range(segmentType.minSegmentsToSpawn, segmentType.maxSegmentsToSpawn + 1);
                countBeforeRepeat = segmentType.segmentsBeforeRespawn;
            }

            Segment segment = SegmentManager.instance.GetSegmentToSpawn(spawnType, isStart, segmentSameTypeSpawned >= segmentsSameTypeToSpawn, countBeforeRepeat, PlayerMotor.instance.Reverse ? 50 : 0);

            Config.Types.Segment.SegmentType nextSpawnType = spawnType;

            if (segmentSameTypeSpawned >= segmentsSameTypeToSpawn)
            {
                segmentSameTypeSpawned = 0;
                List<Config.Types.Segment.SegmentType> endSegmentTypes;
                endSegmentTypes = segment.segmentEndType.Where(x => x != spawnType).ToList();
                nextSpawnType = Helper.GetRandom(endSegmentTypes);
            }
            else
            {
                segmentSameTypeSpawned++;
            }


            segment.transform.position = new Vector3(0, 0, nextSpawnPositionZ);


            segment.startSpawnType = spawnType;
            segment.endSpawnType = nextSpawnType;
            segment.gameObject.SetActive(true);


            nextSpawnPositionZ += PlayerMotor.instance.Reverse ? -(int)segment.length : (int)segment.length;


            spawnType = nextSpawnType;
            segmentsSpawned.Add(segment);
        }



    }

    internal void SpawnSegmentBack()
    {
        segmentSameTypeSpawned = 0;
        List<Segment> SegmentsToDespawn = new List<Segment>();
        for (int i = 1; i < segmentsSpawned.Count; i++)
        {
            Segment seg = segmentsSpawned[i];
            if (Vector3.Distance(seg.transform.position, cameraContainer.position) > (int)seg.length * 1.5f)
            {
                SegmentsToDespawn.Add(seg);
            }
        }
        SegmentsToDespawn.ForEach(x => { SegmentManager.instance.DespawnSegment(x); segmentsSpawned.Remove(x); });


        spawnType = segmentsSpawned[0].startSpawnType;
        //if (segmentSameTypeSpawned == 0)
        //{
        //    SegmentTypeConfig segmentType = SegmentManager.instance.segmentTypes.Single(x => x.type == spawnType);
        //    segmentsSameTypeToSpawn = UnityEngine.Random.Range(segmentType.minSegmentsToSpawn, segmentType.maxSegmentsToSpawn + 1);
        //    countBeforeRepeat = segmentType.segmentsBeforeRespawn;
        //}
        Segment segment = SegmentManager.instance.GetSegmentToSpawn(spawnType, false, segmentSameTypeSpawned >= segmentsSameTypeToSpawn, countBeforeRepeat, 50, spawnBasedOnEndType: true);

        Config.Types.Segment.SegmentType nextSpawnType = spawnType;

        //if (segmentSameTypeSpawned >= segmentsSameTypeToSpawn)
        //{
        //    segmentSameTypeSpawned = 0;
        //    List<Config.Types.Segment.SegmentType> endSegmentTypes;
        //    endSegmentTypes = segment.segmentEndType.Where(x => x != spawnType).ToList();
        //    nextSpawnType = Helper.GetRandom(endSegmentTypes);
        //}
        //else
        //{
        //    segmentSameTypeSpawned++;
        //}

        segment.transform.position = new Vector3(0, 0, backSpawnPos + 50);
        segment.startSpawnType = spawnType;
        segment.endSpawnType = nextSpawnType;
        segment.gameObject.SetActive(true);

        nextSpawnPositionZ = (backSpawnPos);

        List<Segment> newSegmentList = new List<Segment>();
        for (int i = segmentsSpawned.Count - 1; i >= 0; i--)
        {
            newSegmentList.Add(segmentsSpawned[i]);
        }
        segmentsSpawned = newSegmentList;

        spawnType = nextSpawnType;
        segmentsSpawned.Add(segment);
    }


    internal void SpawnSegmentFront()
    {
        segmentSameTypeSpawned = 0;
        List<Segment> SegmentsToDespawn = new List<Segment>();
        for (int i = 1; i < segmentsSpawned.Count; i++)
        {
            Segment seg = segmentsSpawned[i];
            if (Vector3.Distance(seg.transform.position, cameraContainer.position) > (int)seg.length * 1.5f)
            {
                SegmentsToDespawn.Add(seg);
            }
        }

        spawnType = segmentsSpawned[0].startSpawnType;

        SegmentsToDespawn.ForEach(x => { SegmentManager.instance.DespawnSegment(x); segmentsSpawned.Remove(x); });

        Segment segment = SegmentManager.instance.GetSegmentToSpawn(spawnType, false, segmentSameTypeSpawned >= segmentsSameTypeToSpawn, countBeforeRepeat, 50);
       
        Config.Types.Segment.SegmentType nextSpawnType = spawnType;

        segment.transform.position = new Vector3(0, 0, backSpawnPos - 50);
        segment.startSpawnType = spawnType;
        segment.endSpawnType = nextSpawnType;
        segment.gameObject.SetActive(true);

        nextSpawnPositionZ = backSpawnPos;

        List<Segment> newSegmentList = new List<Segment>();
        for (int i = segmentsSpawned.Count - 1; i >= 0; i--)
        {
            newSegmentList.Add(segmentsSpawned[i]);
        }
        segmentsSpawned = newSegmentList;
        spawnType = nextSpawnType;
        segmentsSpawned.Add(segment);
    }

    private void DespawnSegment()
    {
        if (segmentsSpawned.Count == 0)
        {
            return;
        }
        Segment segment = segmentsSpawned[0];
        if (Vector3.Distance(segment.transform.position, cameraContainer.position) > (int)segment.length * 1.5f)
        {
            backSpawnPos = (int)segment.transform.position.z;
            SegmentManager.instance.DespawnSegment(segmentsSpawned[0]);
            segmentsSpawned.RemoveAt(0);
        }
    }


    public void EnableObstacleRemoval(float length)
    {
        Vector3 scale = obstacleRemover.gameObject.transform.localScale;
        Transform ObstacleRemover = obstacleRemover.gameObject.transform;
        ObstacleRemover.localScale = new Vector3(scale.x, scale.y, length);
        ObstacleRemover.position = new Vector3(0, 0, PlayerMotor.instance.GetPosition().z);
        obstacleRemover.gameObject.SetActive(true);
    }
}

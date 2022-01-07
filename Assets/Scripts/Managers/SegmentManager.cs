using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SegmentManager : MonoBehaviour
{
    public Segment[] segments;
    public List<SegmentTypeConfig> segmentTypes;

    private List<Segment> segmentsAvailable;
    private Dictionary<Config.Types.Segment.SegmentType, List<Segment>> segmentsUsed;

    public static SegmentManager instance { get; private set; }

    public void Init()
    {
        if (instance != null)
        {
            Debug.LogError("More than one SegmentManager in scene");
            return;
        }
        instance = this;
        PoolSegments();
    }


    private void PoolSegments()
    {
        segmentsUsed = new Dictionary<Config.Types.Segment.SegmentType, List<Segment>>();
        segmentsAvailable = new List<Segment>();

        for (int i = 0; i < segments.Length; i++)
        {
            Segment segment = Instantiate(segments[i], this.transform);
            segment.gameObject.SetActive(false);
            segmentsAvailable.Add(segment);
        }
    }


    public List<Config.Types.Segment.SegmentType> GetReverseChangingSegment(Config.Types.Segment.SegmentType spawnType)
    {
        IEnumerable<Segment> availableSegmentesFiltered = segmentsAvailable.Where(x => x.segmentEndType.Any(y => y == spawnType) && !x.segmentStartType.Any(y => y == spawnType));

        Segment segmentToSpawn = Helper.GetRandom(availableSegmentesFiltered);

        List<Config.Types.Segment.SegmentType> TypesList = new List<Config.Types.Segment.SegmentType>();

        availableSegmentesFiltered.ToList().ForEach(x => x.segmentStartType.ForEach(y => TypesList.Add(y)));

        return TypesList;
    }


    public Segment GetSegmentToSpawn(Config.Types.Segment.SegmentType spawnType, bool isStart, bool changeSpawnType, int countBeforeRepeat, int forcedLength = 0, bool spawnBasedOnEndType = false)
    {
        if (spawnBasedOnEndType)
        {
            if (!segmentsUsed.ContainsKey(spawnType))
            {
                segmentsUsed.Add(spawnType, new List<Segment>());
            }
            IEnumerable<Segment> availableSegmentesFiltered = segmentsAvailable.Where(x => (x.segmentStartType.Any(y => y == spawnType) && x.segmentEndType.Count == 0 || x.segmentEndType.Any(y => y == spawnType)) && !segmentsUsed[spawnType].Contains(x));

            if (changeSpawnType)
            {
                availableSegmentesFiltered = availableSegmentesFiltered.Where(x => x.segmentStartType.Any(y => y != spawnType));
            }
            else
            {
                availableSegmentesFiltered = availableSegmentesFiltered.Where(x => x.segmentStartType.Any(y => y == spawnType));
            }

            if (forcedLength != 0)
            {
                availableSegmentesFiltered = availableSegmentesFiltered.Where(x => (int)x.length == forcedLength);

            }

            Segment segmentToSpawn = Helper.GetRandom(availableSegmentesFiltered);
            segmentsAvailable.Remove(segmentToSpawn);
            segmentToSpawn.lastTypeSpawned = spawnType;

            segmentsUsed[spawnType].Add(segmentToSpawn);

            if (segmentsUsed[spawnType].Count > countBeforeRepeat)
            {
                segmentsUsed[spawnType].RemoveAt(0);
            }
            return segmentToSpawn;
        }
        else
        {
            if (!segmentsUsed.ContainsKey(spawnType))
            {
                segmentsUsed.Add(spawnType, new List<Segment>());
            }
            IEnumerable<Segment> availableSegmentesFiltered = segmentsAvailable.Where(x => x.segmentStartType.Any(y => y == spawnType) && !segmentsUsed[spawnType].Contains(x));


            if (isStart)
            {
                availableSegmentesFiltered = availableSegmentesFiltered.Where(x => x.canStart);
            }
            else if (changeSpawnType)
            {
                availableSegmentesFiltered = availableSegmentesFiltered.Where(x => x.segmentEndType.Any(y => y != spawnType));
            }
            else
            {
                availableSegmentesFiltered = availableSegmentesFiltered.Where(x => !x.segmentEndType.Any() || x.segmentEndType.Any(y => y == spawnType));
            }

            if (forcedLength != 0)
            {
                availableSegmentesFiltered = availableSegmentesFiltered.Where(x => (int)x.length == forcedLength);

            }
            Segment segmentToSpawn = Helper.GetRandom(availableSegmentesFiltered);
            segmentsAvailable.Remove(segmentToSpawn);
            segmentToSpawn.lastTypeSpawned = spawnType;

            segmentsUsed[spawnType].Add(segmentToSpawn);

            if (segmentsUsed[spawnType].Count > countBeforeRepeat)
            {
                segmentsUsed[spawnType].RemoveAt(0);
            }
            return segmentToSpawn;
        }




    }


    public void DespawnSegment(Segment segment)
    {
        segmentsAvailable.Add(segment);
        segment.gameObject.SetActive(false);
    }

    public Config.Types.MeshType GetMeshType()
    {
        return segmentTypes.Single(x => x.type == SegmentSpawner.instance.spawnType).meshType;
    }

}

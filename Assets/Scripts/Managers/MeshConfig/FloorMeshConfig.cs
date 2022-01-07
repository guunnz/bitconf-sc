using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorMeshConfig : MonoBehaviour
{
    public Config.Types.Segment.SegmentLength length;
    public Config.Types.MeshType meshTypeStart;
    public Config.Types.MeshType meshTypeEnd;
    public bool canStart;
    public bool specialFloor;
    public bool decorationEnd;

    public Transform[] decorationSideSpawnPoints;
    public Transform[] decorationCornerSpawnPoints;

    [HideInInspector]
    private List<GameObject> decorationsSideUsed;
    [HideInInspector]
    private List<GameObject> decorationsCornerUsed;
    //public List<Transform> spawnedDecorations;

    private bool isStart = true;


    private void OnEnable()
    {
        if (isStart)
        {
            isStart = false;
            return;
        }
        SpawnDecoration();
    }

    private void OnDisable()
    {
        DespawnDecoration();
    }

    internal void SpawnDecoration()
    {
        if(decorationsSideUsed== null)
        {
            decorationsSideUsed = new List<GameObject>();
        }
        if(decorationsCornerUsed == null)
        {
            decorationsCornerUsed = new List<GameObject>();
        }

        Config.Types.MeshType decoratinType = decorationEnd ? meshTypeEnd : meshTypeStart;
        for (int i = 0; i < decorationSideSpawnPoints.Length; i++)
        {
            GameObject decoration = DecorationManager.instance.SpawnSideDecoration(decoratinType, decorationSideSpawnPoints[i]);
            decoration.SetActive(true);
            //spawnedDecorations.Add(decoration.transform);
            decorationsSideUsed.Add(decoration);
        }

        for (int i = 0; i < decorationCornerSpawnPoints.Length; i++)
        {
            GameObject decoration = DecorationManager.instance.SpawnCornerDecoration(decoratinType, decorationCornerSpawnPoints[i]);
            decorationsCornerUsed.Add(decoration);
        }
    }

    internal void DespawnDecoration()
    {
        if (decorationsSideUsed == null && decorationsCornerUsed == null)
        {
            return;
        }
        Config.Types.MeshType decoratinType = decorationEnd ? meshTypeEnd : meshTypeStart;
        DecorationManager.instance.DespawnSideDecoration(decoratinType, decorationsSideUsed);
        DecorationManager.instance.DespawnCornerDecoration(decoratinType, decorationsCornerUsed);
        decorationsSideUsed.Clear();
        decorationsCornerUsed.Clear();
    }
}

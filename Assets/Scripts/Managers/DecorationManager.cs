using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DecorationManager : MonoBehaviour
{
    public int decorationsToSpawn = 5;
    public DecorationConfig[] decorationConfigs;
    public static DecorationManager instance { get; private set; }

    private Dictionary<Config.Types.MeshType, List<GameObject>> decorationsSideAvailable;
    private Dictionary<Config.Types.MeshType, List<GameObject>> decorationsCornerAvailable;
    private Dictionary<Config.Types.MeshType, List<GameObject>> decorationsUsed;



    public void Init()
    {
        if (instance != null)
        {
            Debug.LogError("More than one DecorationManager in scene");
            return;
        }
        instance = this;
        PoolDecorations();
    }

    private void PoolDecorations()
    {
        decorationsSideAvailable = new Dictionary<Config.Types.MeshType, List<GameObject>>();
        decorationsCornerAvailable = new Dictionary<Config.Types.MeshType, List<GameObject>>();
        decorationsUsed = new Dictionary<Config.Types.MeshType, List<GameObject>>();

        for (int i = 0; i < decorationConfigs.Length; i++)
        {
            DecorationConfig decorationConfig = decorationConfigs[i];
            decorationsSideAvailable.Add(decorationConfig.meshType, new List<GameObject>());
            decorationsCornerAvailable.Add(decorationConfig.meshType, new List<GameObject>());
            decorationsUsed.Add(decorationConfig.meshType, new List<GameObject>());

            PoolDecorations(decorationConfig.meshType, decorationsToSpawn, decorationConfig.sideDecorationMeshes, decorationsSideAvailable);
            PoolDecorations(decorationConfig.meshType, decorationsToSpawn, decorationConfig.cornerDecorationMeshes, decorationsCornerAvailable);
        }
    }

    private void PoolDecorations(Config.Types.MeshType meshType, int decorationsToSpawn, GameObject[] decorations, Dictionary<Config.Types.MeshType, List<GameObject>> availableDictionary)
    {
        for (int j = 0; j < decorations.Length; j++)
        {
            for (int k = 0; k < decorationsToSpawn; k++)
            {
                GameObject decoration = Instantiate(decorations[j], this.transform);
                decoration.SetActive(false);
                availableDictionary[meshType].Add(decoration);
            }
        }
    }

    public GameObject SpawnSideDecoration(Config.Types.MeshType meshType, Transform transformToSpawn)
    {
        if (decorationsSideAvailable[meshType].Where(x => !x.activeSelf).Count() == 0)
        {
            DecorationConfig decorationConfig = decorationConfigs.Single(x => x.meshType == meshType);
            PoolDecorations(meshType, 1, decorationConfig.sideDecorationMeshes, decorationsSideAvailable);
            Debug.Log("NEW SIDE DECORATION INSTANTIATED");
        }
        return SpawnDecoration(meshType, decorationsSideAvailable, transformToSpawn);
    }

    public GameObject SpawnCornerDecoration(Config.Types.MeshType meshType, Transform transformToSpawn)
    {
        if (decorationsCornerAvailable[meshType].Where(x => !x.activeSelf).Count() == 0)
        {
            DecorationConfig decorationConfig = decorationConfigs.Single(x => x.meshType == meshType);
            PoolDecorations(meshType, 1, decorationConfig.cornerDecorationMeshes, decorationsCornerAvailable);
            Debug.Log("NEW CORNER DECORATION INSTANTIATED");
        }
        return SpawnDecoration(meshType, decorationsCornerAvailable, transformToSpawn);
    }


    private GameObject SpawnDecoration(Config.Types.MeshType meshType, Dictionary<Config.Types.MeshType, List<GameObject>> availableDecorations, Transform transformToSpawn)
    {
        GameObject decoration = Helper.GetRandom<GameObject>(availableDecorations[meshType]);
        if (decorationsUsed[meshType].IndexOf(decoration) > -1)
        {
            Debug.LogError("ESTO ESTA MAL");
        }
        availableDecorations[meshType].Remove(decoration);
        decorationsUsed[meshType].Add(decoration);
        decoration.transform.position = transformToSpawn.position;
        decoration.transform.rotation = transformToSpawn.rotation;
        decoration.gameObject.SetActive(true);
        return decoration;
    }

    public void DespawnSideDecoration(Config.Types.MeshType meshType, List<GameObject> decorationsToRemove)
    {
        DespawnDecoration(meshType, decorationsToRemove, decorationsSideAvailable);
    }

    public void DespawnCornerDecoration(Config.Types.MeshType meshType, List<GameObject> decorationsToRemove)
    {
        DespawnDecoration(meshType, decorationsToRemove, decorationsCornerAvailable);
    }

    private void DespawnDecoration(Config.Types.MeshType meshType, List<GameObject> decorationsToRemove, Dictionary<Config.Types.MeshType, List<GameObject>> decorations)
    {
        if (decorationsToRemove == null)
        {
            return;
        }
        foreach (GameObject decoration in decorationsToRemove)
        {
            decorationsUsed[meshType].Remove(decoration);
            decorations[meshType].Add(decoration);
            decoration.gameObject.SetActive(false);
        }
    }
}
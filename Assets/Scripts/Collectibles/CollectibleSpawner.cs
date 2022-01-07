using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CollectibleSpawner : MonoBehaviour
{
    public int chancesToSpawn;
    public List<CollectibleSpawnerConfig> collectibles;
    private Collectible collectibleSpawned;
    private bool isStart = true;

    private void OnEnable()
    {
        if (isStart)
        {
            isStart = false;
            return;
        }
        SpawnCollectible();
    }

    private void OnDisable()
    {
        if(collectibleSpawned == null)
        {
            return;
        }
        CollectibleManager.instance.DespawnCollectible(collectibleSpawned);
        collectibleSpawned = null;
    }

    private void SpawnCollectible()
    {
        int rand = Random.Range(1, 100);
        if (rand > chancesToSpawn)
        {
            return;
        }

        int totalChances = collectibles.Sum(x => x.chancesToSpawn);
        int collectibleRand = Random.Range(1, totalChances);
        int chance = 0;
        Config.Types.Collectible.CollectibleType? collectibleTypeToSpawn = null;
        for (int i = 0; i < collectibles.Count(); i++)
        {
            CollectibleSpawnerConfig collectible = collectibles.ElementAt(i);
            chance += collectible.chancesToSpawn;
            if(!collectibleTypeToSpawn.HasValue && collectibleRand <= chance )
            {
                collectibleTypeToSpawn = collectible.collectibleType;
            }
        }

        if (!collectibleTypeToSpawn.HasValue)
        {
            return;
        }

        collectibleSpawned = CollectibleManager.instance.SpawnCollectible(collectibleTypeToSpawn.Value, this.transform.position);
        
    }
}

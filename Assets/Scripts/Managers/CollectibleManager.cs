using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public class CollectibleManager : MonoBehaviour
{

    public CollectibleConfig[] collectibleConfigs;

    public int minCoinsInRow = 2;
    public int maxCoinsInRow = 12;

    private Dictionary<Config.Types.Collectible.CollectibleType, List<Collectible>> availableCollectibles;
    private Dictionary<Config.Types.Collectible.CollectibleType, List<Collectible>> usedCollectibles;

    public static CollectibleManager instance { get; private set; }

    public void Init()
    {
        if (instance != null)
        {
            Debug.LogError("More than one CollectibleManager in scene");
            return;
        }
        instance = this;
        PoolCollectibles();
    }

    private void PoolCollectibles()
    {
        availableCollectibles = new Dictionary<Config.Types.Collectible.CollectibleType, List<Collectible>>();
        usedCollectibles = new Dictionary<Config.Types.Collectible.CollectibleType, List<Collectible>>();
        for (int i = 0; i < collectibleConfigs.Length; i++)
        {
            CollectibleConfig collectibleConfig = collectibleConfigs[i];
            availableCollectibles.Add(collectibleConfig.prefab.collectibleType, new List<Collectible>());
            usedCollectibles.Add(collectibleConfig.prefab.collectibleType, new List<Collectible>());
            PoolCollectible(collectibleConfig.prefab, collectibleConfig.collectiblesToSpawn);
        }
    }

    private void PoolCollectible(Collectible prefab, int collectiblesToSpawn)
    {
        for (int i = 0; i < collectiblesToSpawn; i++)
        {
            Collectible collectible = Instantiate(prefab, this.transform);
            collectible.gameObject.SetActive(false);
            availableCollectibles[prefab.collectibleType].Add(collectible);
        }
    }

    public Collectible SpawnCollectible(Config.Types.Collectible.CollectibleType collectibleType, Vector3 positionToSpawn)
    {
        if (availableCollectibles[collectibleType].Count == 0)
        {
            PoolCollectible(Array.Find(collectibleConfigs, x => x.prefab.collectibleType == collectibleType).prefab, 1);
        }
        Collectible collectible = availableCollectibles[collectibleType].First();
        availableCollectibles[collectibleType].Remove(collectible);
        usedCollectibles[collectibleType].Add(collectible);
        collectible.transform.position = positionToSpawn;
        collectible.gameObject.SetActive(true);
        return collectible;
    }

    public void DespawnCollectible(Collectible collectible)
    {
        availableCollectibles[collectible.collectibleType].Add(collectible);
        usedCollectibles[collectible.collectibleType].Remove(collectible);
        collectible.gameObject.SetActive(false);
    }

    public static float GetValue(Config.Types.Collectible.CollectibleType collectibleType)
    {
        Config.Types.Collectible.CollectibleLevel collectibleLevel = PlayerPrefValues.GetCollectiblLevel(collectibleType);
        switch (collectibleType)
        {
            case Config.Types.Collectible.CollectibleType.BigCoin:
                return GetBigCoinValue(collectibleLevel);
            case Config.Types.Collectible.CollectibleType.BonusCoin:
                return 1;
            case Config.Types.Collectible.CollectibleType.Coin:
                return 1;
            case Config.Types.Collectible.CollectibleType.CoinMagnet:
                return CoinMagnetValue(collectibleLevel);
            case Config.Types.Collectible.CollectibleType.CoinMultiplier:
                return CoinMultiplierValue(collectibleLevel);
            case Config.Types.Collectible.CollectibleType.Gold:
                return 1;
            case Config.Types.Collectible.CollectibleType.ScoreMultiplier:
                return ScoreMultiplierValue(collectibleLevel);
            case Config.Types.Collectible.CollectibleType.Shield:
                return ShieldValue(collectibleLevel);
            case Config.Types.Collectible.CollectibleType.Turbo:
                return TurboValue(collectibleLevel);
            default:
                return 0;
        }
    }

    public static int GetValueInt(Config.Types.Collectible.CollectibleType collectibleType)
    {
        return (int)GetValue(collectibleType);
    }

    private static float CoinMultiplierValue(Config.Types.Collectible.CollectibleLevel collectibleLevel)
    {
        switch (collectibleLevel)
        {
            case Config.Types.Collectible.CollectibleLevel.Level1:
                return 10;
            case Config.Types.Collectible.CollectibleLevel.Level2:
                return 13;
            case Config.Types.Collectible.CollectibleLevel.Level3:
                return 16;
            case Config.Types.Collectible.CollectibleLevel.Level4:
                return 19;
            case Config.Types.Collectible.CollectibleLevel.Level5:
                return 22;
            default:
                return 0;
        }
    }

    private static float CoinMagnetValue(Config.Types.Collectible.CollectibleLevel collectibleLevel)
    {
        switch (collectibleLevel)
        {
            case Config.Types.Collectible.CollectibleLevel.Level1:
                return 10;
            case Config.Types.Collectible.CollectibleLevel.Level2:
                return 13;
            case Config.Types.Collectible.CollectibleLevel.Level3:
                return 16;
            case Config.Types.Collectible.CollectibleLevel.Level4:
                return 19;
            case Config.Types.Collectible.CollectibleLevel.Level5:
                return 22;
            default:
                return 0;
        }
    }

    private static float GetBigCoinValue(Config.Types.Collectible.CollectibleLevel collectibleLevel)
    {
        switch (collectibleLevel)
        {
            case Config.Types.Collectible.CollectibleLevel.Level1:
                return 10;
            case Config.Types.Collectible.CollectibleLevel.Level2:
                return 13;
            case Config.Types.Collectible.CollectibleLevel.Level3:
                return 16;
            case Config.Types.Collectible.CollectibleLevel.Level4:
                return 19;
            case Config.Types.Collectible.CollectibleLevel.Level5:
                return 22;
            default:
                return 0;
        }
    }

    private static float ScoreMultiplierValue(Config.Types.Collectible.CollectibleLevel collectibleLevel)
    {
        switch (collectibleLevel)
        {
            case Config.Types.Collectible.CollectibleLevel.Level1:
                return 10;
            case Config.Types.Collectible.CollectibleLevel.Level2:
                return 13;
            case Config.Types.Collectible.CollectibleLevel.Level3:
                return 16;
            case Config.Types.Collectible.CollectibleLevel.Level4:
                return 19;
            case Config.Types.Collectible.CollectibleLevel.Level5:
                return 22;
            default:
                return 0;
        }
    }
    private static float ShieldValue(Config.Types.Collectible.CollectibleLevel collectibleLevel)
    {
        switch (collectibleLevel)
        {
            case Config.Types.Collectible.CollectibleLevel.Level1:
                return 5;
            case Config.Types.Collectible.CollectibleLevel.Level2:
                return 13;
            case Config.Types.Collectible.CollectibleLevel.Level3:
                return 16;
            case Config.Types.Collectible.CollectibleLevel.Level4:
                return 19;
            case Config.Types.Collectible.CollectibleLevel.Level5:
                return 22;
            default:
                return 0;
        }
    }
    private static float TurboValue(Config.Types.Collectible.CollectibleLevel collectibleLevel)
    {
        switch (collectibleLevel)
        {
            case Config.Types.Collectible.CollectibleLevel.Level1:
                return 5;
            case Config.Types.Collectible.CollectibleLevel.Level2:
                return 13;
            case Config.Types.Collectible.CollectibleLevel.Level3:
                return 16;
            case Config.Types.Collectible.CollectibleLevel.Level4:
                return 19;
            case Config.Types.Collectible.CollectibleLevel.Level5:
                return 22;
            default:
                return 0;
        }
    }

    public void SpawnCoins(float startZ, float distance)
    {
        float coinDistance = PlayerSpeed.instance.changeLaneDistance;
        int quantity = Convert.ToInt32(distance / coinDistance);
        int coinsSpawned = 0;

        List<Config.Types.Lane> lanes = new List<Config.Types.Lane>() { Config.Types.Lane.Center, Config.Types.Lane.Left, Config.Types.Lane.Right };
        Config.Types.Lane currentLane = Helper.GetRandom(lanes);
        Dictionary<Config.Types.Lane, float> lanePosition = new Dictionary<Config.Types.Lane, float>();
        foreach (Config.Types.Lane lane in lanes)
        {
            lanePosition.Add(lane, Config.Lane.GetLanePosition(lane));
        }

        Vector3 coinPosition = new Vector3(lanePosition[currentLane], 0, startZ);
        lanes.RemoveAll(x => x == Config.Types.Lane.Center);

        while (coinsSpawned < quantity)
        {
            int coinsQty = UnityEngine.Random.Range(minCoinsInRow, maxCoinsInRow + 1);
            int toSpawn = Math.Min(coinsQty, quantity - coinsSpawned);            
            for (int i = 0; i < toSpawn; i++)
            {
                SpawnCollectible(Config.Types.Collectible.CollectibleType.Coin, coinPosition);
                coinPosition += new Vector3(0, 0, coinDistance);
            }
            coinsSpawned += toSpawn;

            switch (currentLane)
            {
                case Config.Types.Lane.Right:
                case Config.Types.Lane.Left:
                    currentLane = Config.Types.Lane.Center;
                    break;
                default:
                    currentLane = Helper.GetRandom(lanes);
                    break;
            }
            coinPosition = new Vector3(lanePosition[currentLane], 0, coinPosition.z);
        }
    }
}
using Config;


public class Turbo : Collectible
{

    public int speedBoostPercentage = 100;
    public int coinSpawnMargin = 15;
    public float secondsToReachMaxSpeed = 0.5f;

    public override Types.SFX? collectedSFX => Types.SFX.AgarrarPowerUp;
    public override Types.SFX? collectibleBackgroundSFX => null;
    public override Types.SFX? collectibleEndSFX => null;
    public override Types.Collectible.CollectibleType collectibleType => Types.Collectible.CollectibleType.Turbo;
    public override void PickedUp()
    {
        base.PickedUp();
        float duration = CollectibleManager.GetValue(this.collectibleType);
        UIManager.instance.ShowCollectible(collectibleType, duration);
        PlayerCollectible.instance.TurboStart(this, duration);
        PlayerSpeed.instance.TurboStart(speedBoostPercentage, secondsToReachMaxSpeed);

        float turboDistance = PlayerSpeed.instance.GetDistanceToTravelInSeconds(duration, speedBoostPercentage);

        SegmentSpawner.instance.EnableObstacleRemoval(turboDistance + (coinSpawnMargin * 2));

        CollectibleManager.instance.SpawnCoins(this.transform.position.z + coinSpawnMargin, turboDistance);


    }

    public override void Ended()
    {
        base.Ended();
        PlayerCollectible.instance.TurboEnd();
        PlayerSpeed.instance.TurboEnd();
    }
}

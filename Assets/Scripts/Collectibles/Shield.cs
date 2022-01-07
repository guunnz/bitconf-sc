using Config;


public class Shield : Collectible
{
    public override Types.SFX? collectedSFX => Types.SFX.AgarrarPowerUp;
    public override Types.SFX? collectibleBackgroundSFX => null;
    public override Types.SFX? collectibleEndSFX => null;
    public override Types.Collectible.CollectibleType collectibleType => Types.Collectible.CollectibleType.Shield;

    public int obstacleRemoverLength = 20;

    public override void PickedUp()
    {
        base.PickedUp();
        float duration = CollectibleManager.GetValue(this.collectibleType);
        UIManager.instance.ShowCollectible(collectibleType, duration);
        PlayerCollectible.instance.ShieldStart(this, duration);
    }

    public override void Ended()
    {
        base.Ended();
        PlayerCollectible.instance.ShieldEnd(false);
    }

    public void Used()
    {
        //AudioManager.DO.Stop(this.collectibleBackgroundSFX.Value);
        //AudioManager.DO.Play(Config.Types.SFX.ShieldUsed);
        UIManager.instance.HideCollectible(Config.Types.Collectible.CollectibleType.Shield);
        SegmentSpawner.instance.EnableObstacleRemoval(obstacleRemoverLength);
        PlayerCollectible.instance.ShieldEnd(true);
    }
}

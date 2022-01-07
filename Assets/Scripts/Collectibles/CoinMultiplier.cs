using Config;


public class CoinMultiplier : Collectible
{
    public override Types.SFX? collectedSFX => Types.SFX.AgarrarPowerUp;
    public override Types.SFX? collectibleBackgroundSFX => null;
    public override Types.SFX? collectibleEndSFX => null;
    public override Types.Collectible.CollectibleType collectibleType => Types.Collectible.CollectibleType.CoinMultiplier;

    public override void PickedUp()
    {
        base.PickedUp();
        float duration = CollectibleManager.GetValue(this.collectibleType);
        UIManager.instance.ShowCollectible(collectibleType, duration);
        PlayerCollectible.instance.CoinMultiplierStart(this, duration, 2);
    }

    public override void Ended()
    {
        base.Ended();
        PlayerCollectible.instance.CoinMultiplierEnd();
    }
}
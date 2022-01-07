using Config;

public class CoinMagnet : Collectible
{
    public override Types.SFX? collectedSFX => Types.SFX.AgarrarPowerUp;
    public override Types.SFX? collectibleBackgroundSFX => Types.SFX.LoopPowerUpIman;
    public override Types.SFX? collectibleEndSFX => null;
    public override Types.Collectible.CollectibleType collectibleType => Types.Collectible.CollectibleType.CoinMagnet;

    public override void PickedUp()
    {
        base.PickedUp();
        float duration = CollectibleManager.GetValue(this.collectibleType);
        UIManager.instance.ShowCollectible(collectibleType, duration);
        PlayerCollectible.instance.CoinMagnetStart(this, duration);
    }

    public override void Ended()
    {
        base.Ended();
        PlayerCollectible.instance.CoinMagnetEnd();
    }

}

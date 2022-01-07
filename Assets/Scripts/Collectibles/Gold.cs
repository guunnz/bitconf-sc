using Config;

public class Gold : Collectible
{
    public override Types.SFX? collectedSFX => Types.SFX.AgarrarMonedaEspecial;
    public override Types.SFX? collectibleBackgroundSFX => null;
    public override Types.SFX? collectibleEndSFX => null;
    public override Types.Collectible.CollectibleType collectibleType => Types.Collectible.CollectibleType.Gold;

    public override void PickedUp()
    {
        PlayerMotor.instance.animator.SetTrigger(Config.AnimationTriggers.Player.PowerUpPickup);

        base.PickedUp();
        int value = CollectibleManager.GetValueInt(this.collectibleType);
        GameManager.instance.GoldCollected(value);
        PlayerCollectible.instance.ShowCoinCollectAnimator();
    }
}
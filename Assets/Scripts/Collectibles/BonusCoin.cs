using Config;

public class BonusCoin : Collectible
{

    public override Types.SFX? collectedSFX => Types.SFX.AgarrarItemEspecial;

    public override Types.SFX? collectibleBackgroundSFX => null;

    public override Types.SFX? collectibleEndSFX => null;

    public override Types.Collectible.CollectibleType collectibleType => Types.Collectible.CollectibleType.BonusCoin;

    public override void PickedUp()
    {
        PlayerMotor.instance.animator.SetTrigger(Config.AnimationTriggers.Player.PowerUpPickup);
        base.PickedUp();
        PlayerCollectible.instance.ShowCollectedParticles();
        int value = CollectibleManager.GetValueInt(this.collectibleType);
        GameManager.instance.BonusCoinCollected(value);
        PlayerCollectible.instance.ShowCoinCollectAnimator();
    }
}
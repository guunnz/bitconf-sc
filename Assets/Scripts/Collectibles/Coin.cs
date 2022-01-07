using UnityEngine;

public class Coin : Collectible
{
    public override Config.Types.SFX? collectedSFX => Config.Types.SFX.AgarrarMonedaNormal;
    public override Config.Types.SFX? collectibleBackgroundSFX => null;
    public override Config.Types.SFX? collectibleEndSFX => null;
    public override Config.Types.Collectible.CollectibleType collectibleType => Config.Types.Collectible.CollectibleType.Coin;


    private bool inCoinMagnet = false;

    private void OnEnable()
    {
        StartRotation();
        inCoinMagnet = false;
        ImmuneToDestroyer = true;
        destroyInBoss = true;
    }


    private void Update()
    {
        if (!inCoinMagnet)
        {
            return;
        }

        float distance = (PlayerSpeed.instance.GetCurrentSpeed() * Time.deltaTime);
        if (PlayerCollectible.instance.coinMagnetTarget.position.z > this.transform.position.z)
        {
            distance *= 2;
        }
        transform.position = Vector3.MoveTowards(transform.position, PlayerCollectible.instance.coinMagnetTarget.position, distance);
    }

    public void InCoinMagnet()
    {
        inCoinMagnet = true;
    }

    public override void PickedUp()
    {
        base.PickedUp();
        int value = CollectibleManager.GetValueInt(this.collectibleType);
        GameManager.instance.CoinCollected(value * PlayerCollectible.instance.coinMultiplierValue);
        PlayerCollectible.instance.ShowCoinCollectAnimator();
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class Collectible : MonoBehaviour
{
    public Animator collectibleAnimator;
    public bool ImmuneToDestroyer;

    public bool destroyInBoss;

    [HideInInspector]
    public abstract Config.Types.SFX? collectedSFX { get; }
    [HideInInspector]
    public abstract Config.Types.SFX? collectibleBackgroundSFX { get; }
    [HideInInspector]
    public abstract Config.Types.SFX? collectibleEndSFX { get; }

    [HideInInspector]
    public abstract Config.Types.Collectible.CollectibleType collectibleType { get; }

    private void OnEnable()
    {
        StartRotation();
    }

    public void StartRotation()
    {
        float distanceBetweenCollectibles = 2;
        int rotationCounts = 7;
        int steps = Mathf.RoundToInt(distanceBetweenCollectibles * rotationCounts);
        int rotationValue = Mathf.RoundToInt(transform.position.z) % steps;
        float startPosition = (1f / steps) * rotationValue * 0.5f;
        collectibleAnimator.Play(Config.AnimationState.Collectible.CollectibleRotate, -1, startPosition);
    }


    public virtual void PickedUp()
    {
        if (this.collectedSFX.HasValue)
        {
            AudioManager.DO.Play(this.collectedSFX.Value);
        }
        if (this.collectibleBackgroundSFX.HasValue)
        {
            AudioManager.DO.Play(this.collectibleBackgroundSFX.Value);
        }
        this.gameObject.SetActive(false);
    }

    public virtual void Ended()
    {
        if (this.collectibleBackgroundSFX.HasValue)
        {
            AudioManager.DO.Stop(this.collectibleBackgroundSFX.Value);
        }
        if (this.collectibleEndSFX.HasValue)
        {
            AudioManager.DO.Play(collectibleEndSFX.Value);
        }
    }

}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerCollectible : MonoBehaviour
{
    [Header("Config on collect")]
    [SerializeField]
    private Animator collectCoinAnimator;
    [SerializeField]
    private GameObject collectCoinGO;
    [SerializeField]
    private ParticleSystem collectPowerUpParticleSystem;

    public GameObject TurboMesh;

    [Header("Coin Magnet Config")]
    [SerializeField]
    private GameObject coinMagnetCollider;
    public Transform coinMagnetTarget;
    [SerializeField]
    private GameObject coinMagnectEffect;

    [Header("Shield Config")]
    [SerializeField]
    private ParticleSystem shieldCrashParticleSystem;
    [SerializeField]
    private GameObject shieldEffect;

    [Header("Turbo Config")]
    [SerializeField]
    private ParticleSystem collectTurboParticleSystem;
    [SerializeField]
    private ParticleSystem turboUsingParticleSystem;

    [Header("Coin Multiplier Config")]
    [SerializeField]
    private GameObject coinMultiplierEffect;

    [Header("Score Multiplier Config")]
    [SerializeField]
    private GameObject scoreMultiplierEffect;


    [HideInInspector]
    public int coinMultiplierValue = 1;
    [HideInInspector]
    public int scoreMultiplierValue = 1;

    private List<Tuple<Config.Types.Collectible.CollectibleType, Collectible, Coroutine>> powerUpCoroutines;

    public static PlayerCollectible instance { get; private set; }

    #region Events

    void Start()
    {
        if (instance != null)
        {
            Debug.LogError("More than one PlayerPowerup in scene");
            return;
        }
        instance = this;
        ResetVariables();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag != Config.Tags.Collectible)
        {
            return;
        }
        Collectible collectible = other.gameObject.GetComponent<Collectible>();
        collectible.PickedUp();
    }

    #endregion

    private void ResetVariables()
    {
        powerUpCoroutines = new List<Tuple<Config.Types.Collectible.CollectibleType, Collectible, Coroutine>>();
        coinMagnetCollider.SetActive(false);
        //shieldObstacleRemoveCollider.SetActive(false);
        coinMultiplierValue = 1;
        scoreMultiplierValue = 1;
    }


    #region VFX

    public void ShowCoinCollectAnimator()
    {
        List<int> rotationAngles = new List<int>() { 0, 45, 90, 135, 180, 225, 270, 315 };
        int rotationAngle = Helper.GetRandom<int>(rotationAngles.Where(x => x != (int)collectCoinGO.transform.rotation.eulerAngles.z));
        collectCoinGO.transform.Rotate(new Vector3(0, 0, rotationAngle - collectCoinGO.transform.rotation.eulerAngles.z));
        collectCoinAnimator.SetTrigger(Config.AnimationTriggers.Collectible.Collected);
    }

    internal void ShowCollectedParticles()
    {
        collectPowerUpParticleSystem.gameObject.SetActive(true);
        Invoke("HideCollectParticles", 1);
    }

    private void HideCollectParticles()
    {
        collectPowerUpParticleSystem.gameObject.SetActive(false);
    }

    #endregion

    private Coroutine StartCoroutine(IEnumerator routine, Collectible collectible)
    {
        Tuple<Config.Types.Collectible.CollectibleType, Collectible, Coroutine> tuple = powerUpCoroutines.FirstOrDefault(x => x.Item1 == collectible.collectibleType);
        if (tuple != null)
        {
            StopCoroutine(tuple.Item3);
            powerUpCoroutines.Remove(tuple);
        }
        Coroutine coroutine = StartCoroutine(routine);
        powerUpCoroutines.Add(new Tuple<Config.Types.Collectible.CollectibleType, Collectible, Coroutine>(collectible.collectibleType, collectible, coroutine));

        if (collectible.collectibleBackgroundSFX.HasValue)
        {
            AudioManager.DO.Play(collectible.collectibleBackgroundSFX.Value);
        }

        return coroutine;
    }

    private void RemoveCoroutine(Config.Types.Collectible.CollectibleType collectibleType, Config.Types.SFX? overrideEndSFX = null)
    {
        Tuple<Config.Types.Collectible.CollectibleType, Collectible, Coroutine> tuple = powerUpCoroutines.FirstOrDefault(x => x.Item1 == collectibleType);
        if (tuple == null)
        {
            return;
        }
        powerUpCoroutines.Remove(tuple);
    }

    public void StartCollectibleCoroutine(Collectible collectible, float duration)
    {
        StartCoroutine(CollectibleCoroutine(collectible, duration), collectible);
    }

    private IEnumerator CollectibleCoroutine(Collectible collectible, float duration)
    {
        if (collectible.collectibleType == Config.Types.Collectible.CollectibleType.Gold)
        {
            PlayerMotor.instance.animator.SetTrigger(Config.AnimationTriggers.Player.PowerUpPickup);
        }

        float currentDuration = 0;
        while (currentDuration < duration)
        {
            currentDuration += Time.deltaTime;
            yield return null;
        }
        RemoveCoroutine(collectible.collectibleType);
        collectible.Ended();
    }


    #region Coin Magnet

    public void CoinMagnetStart(CoinMagnet coinMagnet, float duration)
    {
        PlayerMotor.instance.animator.SetBool(Config.AnimationTriggers.Player.Iman, true);
        StartCollectibleCoroutine(coinMagnet, duration * PlayerPrefs.GetInt(ePowerUp.Iman.ToString()));
        coinMagnetCollider.SetActive(true);
        ShowCollectedParticles();
        coinMagnectEffect.SetActive(true);
    }

    public void CoinMagnetEnd()
    {
        PlayerMotor.instance.animator.SetBool(Config.AnimationTriggers.Player.Iman, false);
        coinMagnetCollider.SetActive(false);
        coinMagnectEffect.SetActive(false);
    }

    #endregion

    #region Coin Multiplier

    public void CoinMultiplierStart(CoinMultiplier coinMultiplier, float duration, int multiplierValue)
    {
        PlayerMotor.instance.animator.SetTrigger(Config.AnimationTriggers.Player.PowerUpPickup);

        StartCollectibleCoroutine(coinMultiplier, duration * PlayerPrefs.GetInt(ePowerUp.CoinMultiplier.ToString()));
        coinMultiplierValue = multiplierValue;
        ShowCollectedParticles();
        coinMultiplierEffect.SetActive(true);
    }

    public void CoinMultiplierEnd()
    {
        coinMultiplierValue = 1;
        coinMultiplierEffect.SetActive(false);
    }

    #endregion

    #region Score Multiplier
    public void ScoreMultiplierStart(ScoreMultiplier scoreMultiplier, float duration, int multiplierValue)
    {
        PlayerMotor.instance.animator.SetTrigger(Config.AnimationTriggers.Player.PowerUpPickup);

        StartCollectibleCoroutine(scoreMultiplier, duration * PlayerPrefs.GetInt(ePowerUp.ScoreMultiplier.ToString()));
        scoreMultiplierValue = multiplierValue;
        ShowCollectedParticles();
        scoreMultiplierEffect.SetActive(true);
    }

    public void ScoreMultiplierEnd()
    {
        scoreMultiplierValue = 1;
        scoreMultiplierEffect.SetActive(false);
    }

    #endregion

    #region Shield

    public void ShieldStart(Shield shield, float duration)
    {
        PlayerMotor.instance.animator.SetTrigger(Config.AnimationTriggers.Player.PowerUpPickup);

        StartCollectibleCoroutine(shield, duration * PlayerPrefs.GetInt(ePowerUp.Escudo.ToString()));
        PlayerCollision.instance.hasShield = true;
        ShowCollectedParticles();
        shieldEffect.SetActive(true);
    }

    public void ShieldEnd(bool delayedDisablePlayerShield)
    {
        RemoveCoroutine(Config.Types.Collectible.CollectibleType.Shield);
        if (delayedDisablePlayerShield)
        {
            Invoke("DisablePlayerShield", 0.1f);
        }
        else
        {
            DisablePlayerShield();
        }
        shieldEffect.SetActive(false);
    }

    private void DisablePlayerShield()
    {
        PlayerCollision.instance.hasShield = false;
    }

    public void ShieldUsed()
    {
        //TODO: A VECES ENTRA 2 VECES SEGUIDAS ACA (DETECTA 2 COLICIONES CON EL MISMO ESCUDO). COMO SE PUEDE SOLUCIONAR?)
        Shield shield = (Shield)powerUpCoroutines.Single(x => x.Item1 == Config.Types.Collectible.CollectibleType.Shield).Item2;
        shield.Used();
        shieldEffect.SetActive(false);
        ShowCollectedUsed();
    }

    public void ShowCollectedUsed()
    {
        shieldCrashParticleSystem.gameObject.SetActive(true);
        Invoke("HideCollectedUsed", 1);
    }

    private void HideCollectedUsed()
    {
        shieldCrashParticleSystem.gameObject.SetActive(false);
    }

    #endregion

    #region Turbo

    public void TurboStart(Turbo turbo, float duration)
    {
        ParticlesManager.DO.Play(ParticleType.CameraTurbo);
        TurboMesh.SetActive(true);
        PlayerMotor.instance.animator.SetBool(Config.AnimationTriggers.Player.Turbo, true);
        StartCollectibleCoroutine(turbo, duration * PlayerPrefs.GetInt(ePowerUp.Turbo.ToString()));
        ShowCollectedTurbo();
        ShowTurboUsing();
    }

    public void TurboEnd()
    {
        ParticlesManager.DO.Stop(ParticleType.CameraTurbo);
        TurboMesh.SetActive(false);
        PlayerMotor.instance.animator.SetBool(Config.AnimationTriggers.Player.Turbo, false);
        HideTurboUsing();
    }

    public void ShowCollectedTurbo()
    {
        collectTurboParticleSystem.gameObject.SetActive(true);
        Invoke("HideCollectTurbo", 1);
    }

    private void HideCollectTurbo()
    {
        collectTurboParticleSystem.gameObject.SetActive(false);
    }

    private void ShowTurboUsing()
    {
        turboUsingParticleSystem.gameObject.SetActive(true);
    }

    private void HideTurboUsing()
    {
        turboUsingParticleSystem.gameObject.SetActive(false);
    }

    #endregion

}
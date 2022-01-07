using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    [SerializeField]
    private PlayerSpeed playerSpeed;
    [SerializeField]
    private PlayerMotor playerMotor;
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private CharacterController controller;
    [SerializeField]
    private int injureDuration = 4;
    public ParticleSystem ParticlesOnDeath;
    [HideInInspector]
    public bool hasShield = false;
    internal bool isInjured = false;

    public static PlayerCollision instance { get; private set; }

    public float OffsetX;
    public float OffsetYFeet;
    public float OffsetYHead;
    private bool canBeInjuredAgain = true;

    public GameObject BrokenBike;

    public int GetInjureDuration()
    {
        return injureDuration;
    }

    void Start()
    {
        if (instance != null)
        {
            Debug.LogError("More than one PlayerMotor in scene");
            return;
        }
        instance = this;
        //Invoke("InjureStart", 0.01f);
    }

    public void InjureStart()
    {
        Injure();
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.tag == Config.Tags.Floor)
        {
            return;
        }
        Vector2 crash = new Vector2(hit.point.x - transform.position.x, hit.point.y - transform.position.y - hit.controller.center.y);
        float x = (float)Math.Round(crash.x, 2);
        float y = (float)Math.Round(crash.y, 2);
        if (Math.Abs(x) < OffsetX && (y < 0 ? playerMotor.Sliding ? false : Math.Abs(y) < OffsetYFeet : playerMotor.Sliding ? Math.Abs(y) >= 0.13f/*OffsetYHead /2*/ : Math.Abs(y) < OffsetYHead))
        {

            Death();
        }
        else if (playerMotor.Sliding ? y > 0.05f : y > -0.3f && Math.Abs(x) > OffsetX)
        {
            //canBeInjuredAgain = false;
            Injure(true);
            //PlayerMotor.instance.StartPlayerMovement(true);
        }
    }


    private void OnTriggerEnter(Collider hit)
    {
        if (hit.CompareTag(Config.Tags.AgentBullet))
        {
            StartCoroutine(PlayerWeapon.instance.GotHurtBoss());
            CameraManager.instance.DoNoise(1f, 1f, 0.1f);
            PlayerMotor.instance.animator.SetTrigger("GetsShot");
            AudioManager.DO.Play(Config.Types.SFX.Explosion);
            GameManager.instance.BonusCoinCollected(-1);

            Bullet bul = hit.GetComponent<Bullet>();
            bul.Explode();
        }
        if (hit.gameObject.tag == Config.Tags.Floor)
        {
            return;
        }

        if (hit.gameObject.tag == Config.Tags.Death)
        {
            Death();
        }
    }

    private void Death()
    {

        if (hasShield)
        {
            PlayerCollectible.instance.ShieldUsed();
            ParticlesOnDeath.Play();
            return;
        }

        if (PlayerBossBehaviour.instance.BossfightStarted)
        {
            PlayerBossBehaviour.instance.Boss.BossAnim.SetTrigger(Config.AnimationTriggers.BossStop);
        }
        ParticlesOnDeath.Play();
        BrokenBike.SetActive(true);
        PlayerMotor.instance.Humo.ForEach(x => x.gameObject.SetActive(false));
        animator.SetTrigger(Config.AnimationTriggers.Player.Dead);


        playerSpeed.isRunning = false;
        //isRunning = false;
        GameManager.instance.OnDeath();
        AudioManager.DO.Play(Config.Types.SFX.ChoqueMuerte);

        Invoke("SetDeathPos", 0.05f);
    }

    public void SetDeathPos()
    {
        EnemyChasing.instance.Anim.SetTrigger(Config.AnimationTriggers.BossChasingLose);
        transform.position = new Vector3(transform.position.x, 0f, transform.position.z);
    }

    private void Injure(bool movePlayer = false)
    {
        if (hasShield)
        {
            PlayerCollectible.instance.ShieldUsed();
            return;
        }
        if (isInjured)
        {
            Death();
            return;
        }
        isInjured = true;

        //animator.SetBool(Config.AnimationTriggers.Player.Injured, true);
        if (movePlayer)
        {
            playerMotor.StartPlayerMovement(true);
        }

        EnemyChasing.instance.StartChasing();
        AudioManager.DO.Play(Config.Types.SFX.ChoqueContraObstaculo);
        AudioManager.DO.Play(Config.Types.SFX.PisadaTrex);
        CameraManager.instance.ChangeCamera(CameraType.CamaraChase);
        CameraManager.instance.DoNoise(2, 2, 0.01f);
        Invoke("StopInjure", injureDuration);
        //Invoke("CanGetInjuredAgain", 1f);
    }

    private void CanGetInjuredAgain()
    {
        canBeInjuredAgain = true;
    }

    private void StopInjure()
    {
        isInjured = false;
        AudioManager.DO.Stop(Config.Types.SFX.PisadaTrex);
        CameraManager.instance.ChangeCamera(CameraType.Normal);
        EnemyChasing.instance.StopChasing();
        //AudioManager.DO.Play(Config.Types.SFX.ChoqueContraObstaculo);
        //animator.SetBool(Config.AnimationTriggers.Player.Injured, false);
    }

}

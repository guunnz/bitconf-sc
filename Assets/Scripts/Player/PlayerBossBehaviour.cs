using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;



public class PlayerBossBehaviour : MonoBehaviour
{
    public EnemyBossAI Boss;

    static public PlayerBossBehaviour instance;
    public float BossDuration;
    private float BossDurationAux;
    internal bool BossfightStarted;
    public int PizzasRequiredForBossfight = 1;
    private GameManager GM;
    private PlayerMotor PM;
    public GameObject EnemyChaser;


    private float pizzaPerTime;
    private float pizzaPerTimeAux;

    public GameObject weapon;

    private void Awake()
    {
        instance = this;
        BossDurationAux = BossDuration;
        //CurrentWeapon = PlayerPrefs.GetInt("WeaponSelected");

    }

    private void Start()
    {
        GM = GameManager.instance;
        pizzaPerTime = BossDuration / 8;
        pizzaPerTimeAux = pizzaPerTime - 0.1f;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Config.Tags.Bullet))
        {
            StartCoroutine(PlayerWeapon.instance.GotHurtBoss());
            CameraManager.instance.DoNoise(1f, 1f, 0.1f);
            PlayerMotor.instance.animator.SetTrigger("GetsShot");
            AudioManager.DO.Play(Config.Types.SFX.Explosion);
            GM.GoldCollected(-1);
            GM.BonusCoinCollected(-1);

            Bullet bul = other.GetComponent<Bullet>();
            bul.Explode();
        }
        else if (other.CompareTag(Config.Tags.Clock))
        {
            BossDurationAux += Boss.TimeAddedPerClock;
        }
    }

    //public void BossStomp()
    //{
    //    CameraManager.instance.DoNoise(0.1f, 0.1f, 0.03f);
    //}

    public void StartBossfight()
    {
        //PlayerPrefs.SetInt(PlayerPrefValues.BonusCoin, 7);
        weapon.SetActive(true);
        //InvokeRepeating("BossStomp", 0f, 0.2f);
        AudioManager.DO.PlayBackgroundMusic(Config.Types.SFX.BossMusic);
        PlayerMotor.instance.animator.SetBool("OnBoss", true);
        //PlayerWeapon.instance.WeaponButton.SetActive(true);
        EnemyChaser.SetActive(false);
        BossDurationAux = BossDuration;
        BossfightStarted = true;
        Boss.gameObject.SetActive(true);
        Boss.BossCanvas.SetActive(true);
    }

    public void EndBossfight()
    {
        CancelInvoke("BossStomp");
        Boss.BossCanvas.SetActive(false);
        PlayerPrefs.SetInt(PlayerPrefValues.BonusCoin, 0);
        AudioManager.DO.PlayBackgroundMusic(Config.Types.SFX.GameplayTierra);

        Boss.BossAnim.ResetTrigger("Stop");
        PlayerMotor.instance.animator.SetBool("OnBoss", false);
        PlayerWeapon.instance.StopWeapon();
        //PlayerWeapon.instance.WeaponButton.SetActive(false);
        StopCoroutine(PlayerWeapon.instance.IUseWeapon());

        PlayerMotor.instance.animator.SetTrigger("Stop");
        Invoke("RemoveWeapon", 1.1f);
    }

    public void RemoveWeapon()
    {
        weapon.SetActive(false);

        Boss.gameObject.SetActive(false);
    }


    private void Update()
    {
        if (BossfightStarted)
        {
            BossDurationAux -= Time.deltaTime;
            pizzaPerTimeAux -= Time.deltaTime;
            if (pizzaPerTimeAux <= 0)
            {
                GameManager.instance.BonusCoinCollected(-1);
                pizzaPerTimeAux = pizzaPerTime;
            }

            if (BossDurationAux <= 0)
            {
                BossfightStarted = false;

                if (!Boss.ChargingAttack)
                    StartCoroutine(PlayerMotor.instance.IToggleBossfight());
            }


        }
        if (!BossfightStarted && GM.GetBonusCoinCollected() >= PizzasRequiredForBossfight)
        {
            BossfightStarted = true;
            StartCoroutine(PlayerMotor.instance.IToggleBossfight());
        }
    }
}

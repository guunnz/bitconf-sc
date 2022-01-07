using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using DG.Tweening;

[System.Serializable]
public class BossPos
{
    public EnemyBossAI.Position Pos;
    public float PosX;
    public bool Ground;
}

public class EnemyBossAI : MonoBehaviour
{
    public GameObject GoldPrefab;
    public GameObject EggPrefab;
    public GameObject BossMissilePrefab;
    public GameObject ClockPrefab;
    public Animator BossAnim;
    public Image RewardBar;

    public GameObject BossCanvas;

    public ParticleSystem ExplosionOnCoin;
    public ParticleSystem ExplosionOnHit;
    public enum Move
    {
        MoveVertical = 0,
        MoveHorizontal = 1,
    }

    public enum Position
    {
        Left = 0,
        Mid = 1,
        Right = 2,
    }

    public float DelayToAction;

    private float DelayToActionAux;

    public float DistanceFromPlayer = 5;

    private bool IsGrounded = true;

    public List<BossPos> BossPositions;

    public float YPositionGround;

    public float YPositionUp;

    public float MoveAmount = 0.3f;

    public float MissileFireRate;

    public int DamagePerCoin;
    private int DamageReceived;

    private Position currentPosition;

    private GameObject LastCoinSpawned;

    public float TimeForFirstShot = 1;

    public bool CanOnlyMoveAdjacent;

    private int MoveCount;

    public float BossChargedAttackTimer = 5;

    public int DamageRequiredToStopSpecialAttack = 10;

    public int BossBulletsAmount;

    public float BossSpecialMissilesRate = 0.5f;

    internal bool ChargingAttack;
    public float TimeAddedPerClock = 15;
    public float ClockSpecialChance = 50;

    public bool CanGoUp = false;

    public bool AttackOnAnyLane = false;

    private List<Bullet> BossSpecialSpawnedBullets = new List<Bullet>();
    public bool OnAction;
    public bool Shooting;

    public bool DroppingCoin;

    public bool CanMove = true;

    private void Start()
    {
        currentPosition = Position.Mid;
    }
    private void OnEnable()
    {
        DelayToActionAux = DelayToAction;
        StartCoroutine(ICheckShoot());
        AudioManager.DO.Play(Config.Types.SFX.PisadaTrex);
    }

    private void OnDisable()
    {
        AudioManager.DO.Stop(Config.Types.SFX.PisadaTrex);
    }

    private void Update()
    {
        if (CanMove)
            this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, PlayerMotor.instance.transform.position.z - DistanceFromPlayer);

        DelayToActionAux -= Time.deltaTime;

        float dmg = (DamageReceived / (float)DamagePerCoin);
        RewardBar.transform.localScale = new Vector3(1 - dmg, 1, 1);



        if (DelayToActionAux < 0 && !OnAction)
        {
            //if (MoveCount < 2 /*&& !ChargingAttack*/)
            //{
            DelayToActionAux = DelayToAction;
            StartCoroutine(IMoveHorizontal());
            //}
            //else if (!ChargingAttack && CanGoUp)
            //{
            //    if (Random.Range(0, 2) == 0)
            //    {
            //        StartCoroutine(IMoveHorizontal());
            //    }
            //    else
            //    {
            //        StartCoroutine(SpecialAttack());
            //    }

            //}
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Config.Tags.Bullet))
        {
            Bullet bul = other.GetComponent<Bullet>();

            if (bul.PositiveZTrajectory)
            {
                return;
            }
            ReceiveDamage(PlayerWeapon.instance.WeaponSelected.Damage);

            if (ChargingAttack)
            {
                DamageRequiredToStopSpecialAttack--;
                if (DamageRequiredToStopSpecialAttack <= 0)
                {
                    CancelSpecialAttack();
                }
            }
            Destroy(bul.gameObject);
        }
    }

    public IEnumerator ICheckShoot()
    {
        yield return new WaitForSeconds(TimeForFirstShot);
        while (true)
        {
            if (!OnAction && !ChargingAttack && !DroppingCoin && !Shooting)
            {
                Shooting = true;
                Shoot();
            }
            else if (!ChargingAttack)
            {
                while (OnAction)
                {
                    yield return null;
                }
            }
            yield return new WaitForSeconds(MissileFireRate);
            Shooting = false;
        }
    }

    public void Shoot()
    {

        if (AttackOnAnyLane)
        {
            BossPos posSelected = BossPositions.Where(x => x.Ground == IsGrounded).ToList()[Random.Range(0, 3)];
            Instantiate(EggPrefab, new Vector3(posSelected.PosX, this.transform.position.y + 2, this.transform.position.z - 8), Quaternion.identity, null);
        }
        else
        {
            Instantiate(EggPrefab, new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z - 8), Quaternion.identity, null);
        }
        this.BossAnim.SetTrigger("BossAttack");
    }

    public void ReceiveDamage(int damageReceived)
    {
        ExplosionOnHit.Play();
        AudioManager.DO.Play(Config.Types.SFX.TrexVoice);
        DamageReceived += damageReceived;
        if (DamageReceived == DamagePerCoin)
        {

            DamageReceived = 0;
            DroppingCoin = true;
            StartCoroutine(DropCoin());
        }
    }

    public IEnumerator DropCoin()
    {
        while (Shooting)
        {
            yield return null;
        }
        ExplosionOnCoin.Play();
        BossAnim.SetTrigger("ReceiveHit");
        //float PositionDrop = BossPositions[Random.Range(0, BossPositions.Count)].PosX;
        LastCoinSpawned = Instantiate(GoldPrefab, new Vector3(this.transform.position.x, 0, this.transform.position.z - 10), Quaternion.Euler(0,180,0), null);
        Invoke("DroppingCoinFalse", DelayToActionAux);
        Invoke("DestroyCoin", 1f);
    }
    public void DroppingCoinFalse()
    {
        DroppingCoin = false;
    }

    public void DestroyCoin()
    {
        Destroy(LastCoinSpawned);
        LastCoinSpawned = null;
    }

    public IEnumerator IMoveVertical()
    {
        //BossPos pos = BossPositions.Single(x => x.Ground == !IsGrounded);
        Vector3 moveTo = new Vector3();
        if (IsGrounded)
        {
            moveTo = new Vector3(this.transform.position.x, YPositionUp, this.transform.position.z);
            while (Vector3.Distance(transform.position, moveTo) > 0.05f)
            {

                transform.position = Vector3.Lerp(transform.position, moveTo, MoveAmount);
                yield return null;
                moveTo = new Vector3(this.transform.position.x, YPositionUp, this.transform.position.z);
            }
        }
        else
        {
            moveTo = new Vector3(this.transform.position.x, YPositionGround, this.transform.position.z);
            while (Vector3.Distance(transform.position, moveTo) > 0.05f)
            {

                transform.position = Vector3.Lerp(transform.position, moveTo, MoveAmount);
                yield return null;
                moveTo = new Vector3(this.transform.position.x, YPositionGround, this.transform.position.z);
            }
        }

        this.transform.position = moveTo;
        IsGrounded = !IsGrounded;
        OnAction = false;
        DelayToActionAux = DelayToAction;
    }


    public void ShootBossMissile()
    {
        BossPos posSelected = BossPositions.Where(x => x.Ground == IsGrounded).ToList()[Random.Range(0, 3)];
        Instantiate(EggPrefab, new Vector3(posSelected.PosX, this.transform.position.y, this.transform.position.z - 6), Quaternion.identity, null);
    }

    public IEnumerator SpecialAttack()
    {
        StartCoroutine(IMoveVertical());
        ChargingAttack = true;

        float timeForSpawn = BossChargedAttackTimer / BossBulletsAmount;
        yield return new WaitForSeconds(timeForSpawn);
        while (BossSpecialSpawnedBullets.Count < BossBulletsAmount)
        {
            BossPos posSelected = BossPositions.Where(x => x.Ground == IsGrounded).ToList()[Random.Range(0, 3)];

            Bullet bul = Instantiate(BossMissilePrefab, new Vector3(posSelected.PosX, this.transform.position.y, this.transform.position.z), Quaternion.identity, this.transform).GetComponent<Bullet>();
            BossSpecialSpawnedBullets.Add(bul);

            yield return new WaitForSeconds(timeForSpawn);
        }

        float timePerShot = BossChargedAttackTimer / BossBulletsAmount;

        foreach (Bullet bul in BossSpecialSpawnedBullets)
        {
            bul.transform.parent = null;
            bul.StartShot = true;
            yield return new WaitForSeconds(BossSpecialMissilesRate);
        }

        if (Random.Range(0, 101) <= ClockSpecialChance)
        {
            BossPos posSelected = BossPositions.Where(x => x.Ground == IsGrounded).ToList()[Random.Range(0, 3)];
            Instantiate(ClockPrefab, new Vector3(posSelected.PosX, this.transform.position.y, this.transform.position.z), Quaternion.identity, this.transform);
        }

        BossSpecialSpawnedBullets.Clear();
        StartCoroutine(IMoveVertical());
        CancelSpecialAttack();
        yield return null;
    }

    public void CancelSpecialAttack()
    {
        StopCoroutine(SpecialAttack());
        BossSpecialSpawnedBullets.ForEach(x => x.Destroy());
        BossSpecialSpawnedBullets.Clear();
        ChargingAttack = false;
        StartCoroutine(IMoveVertical());
    }

    public IEnumerator IMoveHorizontal()
    {
        OnAction = true;
        BossPos posSelected = new BossPos();

        if (CanOnlyMoveAdjacent)
        {
            switch (currentPosition)
            {
                case Position.Left:
                    BossAnim.SetTrigger(Config.AnimationTriggers.BossJumpRight);
                    posSelected = BossPositions.Single(x => x.Ground == IsGrounded && x.Pos == Position.Mid);
                    break;
                case Position.Mid:
                    posSelected = BossPositions.Where(x => x.Ground == IsGrounded && x.Pos != Position.Mid).ToList()[Random.Range(0, 2)];
                    if (posSelected.Pos == Position.Left)
                    {
                        BossAnim.SetTrigger(Config.AnimationTriggers.BossJumpLeft);
                    }
                    else
                    {
                        BossAnim.SetTrigger(Config.AnimationTriggers.BossJumpRight);
                    }
                    break;
                case Position.Right:
                    BossAnim.SetTrigger(Config.AnimationTriggers.BossJumpLeft);
                    posSelected = BossPositions.Single(x => x.Ground == IsGrounded && x.Pos == Position.Mid);
                    break;
            }
        }
        else
        {
            posSelected = BossPositions.Where(x => x.Ground == IsGrounded && x.Pos != currentPosition).ToList()[Random.Range(0, 2)];

            switch (currentPosition)
            {
                case Position.Left:
                    BossAnim.SetTrigger(Config.AnimationTriggers.BossJumpRight);
                    break;
                case Position.Mid:
                    if (posSelected.Pos == Position.Left)
                    {
                        BossAnim.SetTrigger(Config.AnimationTriggers.BossJumpLeft);
                    }
                    else
                    {
                        BossAnim.SetTrigger(Config.AnimationTriggers.BossJumpRight);
                    }
                    break;
                case Position.Right:
                    BossAnim.SetTrigger(Config.AnimationTriggers.BossJumpLeft);
                    break;
            }
        }
        yield return new WaitForSeconds(0.2f);//wait for animation startup

        Vector3 moveTo = new Vector3(posSelected.PosX, this.transform.position.y, this.transform.position.z);
        while (Vector3.Distance(transform.position, moveTo) > 0.05f)
        {
            transform.position = Vector3.Lerp(transform.position, moveTo, MoveAmount);
            yield return null;
            moveTo = new Vector3(posSelected.PosX, this.transform.position.y, this.transform.position.z);
        }
        currentPosition = posSelected.Pos;
        yield return new WaitForSeconds(MoveAmount);
        OnAction = false;
        MoveCount++;
    }

}

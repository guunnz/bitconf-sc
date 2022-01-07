using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[System.Serializable]
public class Weapon
{
    public enum eType
    {
        Rifle = 0,
        Escopeta = 1,
        Rastreadora = 2,
        Cañon = 3,
    }
    public eType Type;
    public float FireRate;
    public int Damage;
    public float WeaponCooldown;
    public float WeaponDuration;
    public GameObject BulletPrefab;
}

public class PlayerWeapon : MonoBehaviour
{
    public List<Weapon> WeaponList;
    internal Weapon WeaponSelected;
    static public PlayerWeapon instance;
    public Weapon.eType CurrentWeapon;
    public Weapon.eType DefaultWeapon;
    public Transform ShotParent;
    private bool WeaponOnCooldown;
    //public GameObject WeaponButton;
    private bool canShoot = true;
    private bool ShootingBasic;
    public float delayOnHit;

    public bool dontShoot = false;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        WeaponSelected = WeaponList.Single(x => x.Type == CurrentWeapon);
    }

    public void Update()
    {
#if UNITY_EDITOR
        if (CurrentWeapon != WeaponSelected.Type)
        {
            WeaponSelected = WeaponList.Single(x => x.Type == CurrentWeapon);
        }
#endif

        if (!ShootingBasic && PlayerBossBehaviour.instance.BossfightStarted)
        {

            ShootingBasic = true;
            StartCoroutine(IUseBasicWeapon());
        }
        //#if UNITY_ANDROID
        //        if (Input.GetTouch(0).tapCount == 2)
        //        {
        //            if (!WeaponOnCooldown)
        //                UseWeapon();
        //        }
        //#endif
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (!WeaponOnCooldown)
                UseWeapon();
        }
    }

    public IEnumerator GotHurtBoss()
    {
        dontShoot = true;
        yield return new WaitForSeconds(delayOnHit);
        dontShoot = false;
        yield return null;
    }

    public IEnumerator IUseBasicWeapon()
    {
        //yield return new WaitForSeconds(2f);

        //WeaponOnCooldown = true;
        //float timer = WeaponSelected.WeaponDuration;
        StartCoroutine(IShootBasic());
        //while (PlayerBossBehaviour.instance.BossfightStarted)
        //{
        //    yield return null;
        //}
        //PlayerMotor.instance.animator.ResetTrigger("Shoot");
        //ShootingBasic = false;
        //StopCoroutine(IShootBasic());
        yield return null;
    }

    public IEnumerator IShootBasic()
    {
        //while (ShootingBasic)
        //{
        //    if (!dontShoot)
        //    {
                PlayerMotor.instance.animator.SetTrigger("Shoot");
                AudioManager.DO.Play(Config.Types.SFX.DisparoPizzaNormal);
                //yield return new WaitForSeconds(WeaponList.Single(x => x.Type == DefaultWeapon).FireRate);

                Shoot();
        //    }
        //    yield return null;
        //}
        yield return null;
    }


    public void UseWeapon()
    {
        StartCoroutine(IUseWeapon());
    }

    public IEnumerator IUseWeapon()
    {
        WeaponOnCooldown = true;
        float timer = WeaponSelected.WeaponDuration;
        StartCoroutine(IShoot());
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            yield return null;
        }
        StopWeapon();
    }

    public IEnumerator IShoot()
    {
        while (canShoot)
        {
            AudioManager.DO.Play(Config.Types.SFX.DisparoPizzaEspecial);
            PlayerMotor.instance.animator.SetTrigger("Shoot");
            yield return new WaitForSeconds(WeaponSelected.FireRate);

            Shoot();
        }
    }

    public void Shoot()
    {
        switch (WeaponSelected.Type)
        {
            case Weapon.eType.Escopeta:
                Instantiate(WeaponSelected.BulletPrefab, new Vector3(0, ShotParent.transform.position.y, ShotParent.transform.position.z), Quaternion.identity, null);
                Instantiate(WeaponSelected.BulletPrefab, new Vector3(3, ShotParent.transform.position.y, ShotParent.transform.position.z), Quaternion.identity, null);
                Instantiate(WeaponSelected.BulletPrefab, new Vector3(-3, ShotParent.transform.position.y, ShotParent.transform.position.z), Quaternion.identity, null);
                break;
            case Weapon.eType.Rastreadora:
            case Weapon.eType.Rifle:
                Instantiate(WeaponSelected.BulletPrefab, ShotParent.transform.position, Quaternion.identity, null);
                break;
            case Weapon.eType.Cañon:
                Instantiate(WeaponSelected.BulletPrefab, ShotParent.transform.position, Quaternion.identity, null);
                StopWeapon();
                break;
        }




    }

    public void StopWeapon()
    {
        canShoot = false;
        StopCoroutine(IShoot());
        StopCoroutine(IUseWeapon());
        StartCoroutine(SetWeaponOnCooldown(false, WeaponSelected.WeaponCooldown));
    }

    public IEnumerator SetWeaponOnCooldown(bool status, float delay)
    {
        yield return new WaitForSeconds(delay);
        WeaponOnCooldown = status;
        canShoot = true;
    }
}

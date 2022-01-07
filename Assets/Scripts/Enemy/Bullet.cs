using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Bullet : MonoBehaviour
{
    public bool PositiveZTrajectory;

    public float Speed = 5;

    public int TimeToDestroy = 2;

    public bool StartShot = true;

    private bool StartDestroy;

    public ParticleSystem particles;

    public float particlesEnd = 0.3f;

    public MeshRenderer MeshToDisableOnExplode;

    //public bool RotateItself;

    public bool playerBullet = false;


    private void Start()
    {
        PositiveZTrajectory = !PlayerMotor.instance.animator.GetBool("OnBoss");
    }

    void FixedUpdate()
    {
        if (StartShot)
        {
            this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z + (Time.deltaTime * (PositiveZTrajectory ? Speed : -Speed)));
            if (!StartDestroy)
            {
                StartDestroy = true;
                Invoke("Destroy", TimeToDestroy);
            }
            //if (RotateItself)
            //{
            //    transform.DOLocalRotate(new Vector3(0,0,90), 2, RotateMode.FastBeyond360);
            //}
        }
    }

    public void Explode()
    {
        if (particles == null)
            return;
        MeshToDisableOnExplode.enabled = false;
        particles.Play();
    }



    public void Destroy()
    {
        Destroy(this.gameObject);
    }
}
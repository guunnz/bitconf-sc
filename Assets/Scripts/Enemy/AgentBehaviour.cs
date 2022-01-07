using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentBehaviour : MonoBehaviour
{
    public GameObject Pizza;

    public GameObject Shot;

    public float DelayToShoot = 0.5f;



    private void Start()
    {
        StartCoroutine(Shoot());

    }

    public IEnumerator Shoot()
    {
        yield return new WaitForSecondsRealtime(DelayToShoot);
        Instantiate(Shot, this.transform.position, Quaternion.identity, null);
        yield return null;
    }

    public void Die()
    {
        Instantiate(Pizza, new Vector3(this.transform.position.x, 0, this.transform.position.z), Quaternion.identity, null);
        Destroy(gameObject);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Config.Tags.Player) || other.CompareTag(Config.Tags.Bullet))
        {
            Die();
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCoinMagnet : MonoBehaviour
{
    private void OnEnable()
    {
        this.transform.eulerAngles = new Vector3(0, 0, 0);
        this.transform.position = new Vector3(0, this.transform.position.y, this.transform.position.z);
    }

    private void Update()
    {
        this.transform.eulerAngles = new Vector3(0, 0, 0);
        this.transform.position = new Vector3(0, this.transform.position.y, this.transform.position.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag != Config.Tags.Collectible)
        {
            return;
        }
        Coin coin = other.gameObject.GetComponent<Coin>();
        if(coin != null)
        {
            coin.InCoinMagnet();
        }
    }
}

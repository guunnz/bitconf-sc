using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetDistanceFromPlayer : MonoBehaviour
{
    public bool UseX;
    public bool UseY;
    public bool UseZ;
    public float Distance;

    private GameObject player;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        if (player == null)
        {
            player = PlayerCollision.instance.gameObject;
            return;
        }
            
        this.transform.position = new Vector3(UseX ? player.transform.position.x + Distance : this.transform.position.x , UseY ? player.transform.position.y + Distance : this.transform.position.y, UseZ ? player.transform.position.z + Distance : this.transform.position.z);
    }
}
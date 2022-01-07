using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleRemover : MonoBehaviour
{

    private int nextDisableCheck = 3;

    public bool DontDisable = false;

    private void Update()
    {
        if (DontDisable)
            return;
        if (Time.time < nextDisableCheck)
        {
            return;
        }
        nextDisableCheck = Mathf.FloorToInt(Time.time) + 3;
        if ((this.transform.position.z + this.transform.localScale.z) < PlayerMotor.instance.GetPosition().z)
        {
            this.gameObject.SetActive(false);
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        Obstacle obstacle = other.gameObject.GetComponentInParent<Obstacle>(true); //TODO: SE PUEDE CAMBIAR POR UN TAG
        if (obstacle != null)
        {
            obstacle.DisableObstacle();
            return;
        }

        RandowmObstacleSpawner randowmObstacleSpawner = other.gameObject.GetComponent<RandowmObstacleSpawner>();
        if (randowmObstacleSpawner != null)
        {
            randowmObstacleSpawner.DespawnObstacles();
            return;
        }

        Collectible collectible = other.gameObject.GetComponent<Collectible>();
        if (collectible != null && (!collectible.ImmuneToDestroyer || (PlayerMotor.instance.Reverse && collectible.destroyInBoss)))
        {
            collectible.gameObject.SetActive(false);
            return;
        }
    }
}

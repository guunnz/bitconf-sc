using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObstacleCollider : MonoBehaviour
{

    public RandowmObstacleSpawner randowmObstacleSpawner;
    public Config.Types.Obstacle.MovingObstacleSpeed speedType;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != Config.Tags.Player)
        {
            return;
        }
        randowmObstacleSpawner.TriggerEntered(speedType);
    }
}

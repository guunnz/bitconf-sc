using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{

    public Config.Types.Obstacle.ObstacleType obstacleType;
    public bool completeBlock;
    public bool betweenLanes;
    public bool canMove;

    private float? speedMovement = null;
    private bool shouldMove = false;

    private GameObject currentObstacleMesh;
    private Config.Types.MeshType currentMeshType;

    private bool isStart = true;

    private void Awake()
    {
        //MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();
        //for (int i = 0; i < meshRenderers.Length; i++)
        //{
        //    meshRenderers[i].enabled = SegmentSpawner.instance.showObstacleColliders;
        //}
    }

    private void OnEnable()
    {
        if (isStart)
        {
            isStart = false;
            return;
        }
        speedMovement = null;
        shouldMove = false;
        if (PlayerBossBehaviour.instance.BossfightStarted)
            return;
        ObstacleManager.instance.SpawnObstacleMesh(obstacleType, this.transform.position, out currentObstacleMesh, out currentMeshType);
        currentObstacleMesh.transform.SetParent(transform);
    }

    private void OnDisable()
    {
        speedMovement = null;
        shouldMove = false;
        ObstacleManager.instance.DespanwObstacleMesh(obstacleType, currentObstacleMesh, currentMeshType);
    }

    private void Update()
    {
        if (!shouldMove || !speedMovement.HasValue)
        {
            return;
        }
        gameObject.transform.position -= new Vector3(0, 0, speedMovement.Value * Time.deltaTime);
    }

    public void SetSpeed(float speed)
    {
        speedMovement = speed;
    }

    public void StartMovement()
    {
        shouldMove = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != Config.Tags.DisableIfHitObstacle)
        {
            return;
        }
        other.gameObject.SetActive(false);
    }

    public void DisableObstacle()
    {
        ObstacleManager.instance.SpawnReplaceObstacleMesh(obstacleType, this.transform.position, currentMeshType);
        this.gameObject.SetActive(false);
    }
}

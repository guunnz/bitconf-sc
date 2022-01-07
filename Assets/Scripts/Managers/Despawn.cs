using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Despawn : MonoBehaviour
{
    public Transform endPosition;
    private int nextDestroyCheck = 1;
    private Transform cameraContainer;

    public void Awake()
    {
        cameraContainer = Camera.main.transform;
    }

    void Update()
    {
        if (Time.time >= nextDestroyCheck)
        {
            nextDestroyCheck = Mathf.FloorToInt(Time.time) + 1;
            if (endPosition.position.z < cameraContainer.position.z)
            {
                this.gameObject.SetActive(false);
            }
        }
    }

}

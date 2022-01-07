using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSelectorOnTrigger : MonoBehaviour
{
    public CameraType CamTypeOnEnter;
    public CameraType CamTypeOnExit;
    private BoxCollider Col;
    private void Start()
    {
        Col = GetComponent<BoxCollider>();
        if (Col == null)
            return;
        Col.size = new Vector3(Col.size.x, Col.size.y, Col.size.z + (PlayerSpeed.instance.GetCurrentSpeed()));
    }


    private void OnTriggerEnter(Collider other)
    {
        if (CamTypeOnEnter == CameraType.None)
        {
            return;
        }
        if (other.CompareTag(Config.Tags.Player))
        {
            CameraManager.instance.ChangeCamera(CamTypeOnEnter);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (CamTypeOnExit == CameraType.None)
        {
            return;
        }
        if (other.CompareTag(Config.Tags.Player))
        {
            CameraManager.instance.ChangeCamera(CamTypeOnExit);
        }
    }
}

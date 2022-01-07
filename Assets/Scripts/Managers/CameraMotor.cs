using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMotor : MonoBehaviour
{
    public Vector3 offset = new Vector3(0, 5.0f, -5.0f);
    public Vector3 rotation = new Vector3(35, 0, 0);

    [Header("Axis X")]
    public float clampX = 2.0f;
    //public float lerpTimeX = 1.5f;

    public bool IsMoving { get; set; }

    private float lastY = 0;

    private void Start()
    {
        lastY = PlayerMotor.instance.GetPosition().y;
    }

    private void Update()
    {
        if (!IsMoving)
        {
            return;
        }
        Vector3 playerPosition = PlayerMotor.instance.GetPosition();
        Vector3 desiredPosition = playerPosition + offset;
        //if (!PlayerMotor.instance.isGrounded)
        //{
        //    desiredPosition.y = lastY + offset.y;
        //}
        //else
        //{
        //    desiredPosition.y = Mathf.Lerp(lastY + offset.y, playerPosition.y + offset.y, 1);
        //    lastY = playerPosition.y;
        //    if (lastY < 0.4)
        //    {
        //        lastY = 0;
        //    }
        //}
        //desiredPosition.x = 0;
        //transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime);

        float desiredX = Mathf.Clamp(playerPosition.x, -clampX, clampX);

        //desiredPosition.x = Mathf.Lerp(PlayerMotor.instance.GetDesiredPositionX(), transform.position.x, lerpTimeX * Time.deltaTime);
        desiredPosition.x = desiredX;
        transform.position = desiredPosition;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(rotation), 1.0f);
    }
}
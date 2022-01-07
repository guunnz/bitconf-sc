using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpeed : MonoBehaviour
{
    public bool stopPlayer;

    public float changeLaneDistance = 2.0f;

    [SerializeField]
    private PlayerSpeedConfig[] speeds;

    private float currentSpeed = 0f;
    private int speedIndex;
    private int? speedNextUpdateDistance;
    public bool isRunning = false;
    private float acceleration;
    private bool isAccelerating = false;
    private int nextSpeedChangeCheck = 1;

    //TURBO
    private int turboBoostPercentage = 0;
    private float turboSpeedToAdd = 0f;
    private float turboAcceleration = 0;
    private float turboSpeedToReach = 0;
    private float distanceReached = 0;
    public float BossfightSpeed = 50;


    public static PlayerSpeed instance { get; private set; }

    void Start()
    {
        if (instance != null)
        {
            Debug.LogError("More than one PlayerSpeed in scene");
            return;
        }
        instance = this;
        ReserVariables();
        GetNextSpeedVariablesUpdate();
    }

    void Update()
    {
        if (!isRunning || stopPlayer)
        {
            return;
        }
        UpdateSpeed();
        Accelerate();

        if (!PlayerBossBehaviour.instance.BossfightStarted)
        distanceReached += currentSpeed * Time.deltaTime;
    }

    private void ReserVariables()
    {
        speedIndex = -1;
        currentSpeed = 0;
        isAccelerating = false;
    }

    private void GetNextSpeedVariablesUpdate()
    {
        speedIndex++;
        if (speeds.Length <= speedIndex || speeds.Length <= speedIndex + 1)
        {
            speedNextUpdateDistance = null;
            return;
        }
        acceleration = (speeds[speedIndex].speed - currentSpeed) / (speeds[speedIndex].secondsToReachSpeed == 0 ? 1 : speeds[speedIndex].secondsToReachSpeed);
        speedNextUpdateDistance = Random.Range(speeds[speedIndex + 1].distanceMin, speeds[speedIndex + 1].distanceMax + 1);
    }


    private void UpdateSpeed()
    {
        if (Time.time < nextSpeedChangeCheck || isAccelerating)
        {
            return;
        }
        nextSpeedChangeCheck = Mathf.FloorToInt(Time.time) + 1;
        if (!speedNextUpdateDistance.HasValue || distanceReached < speedNextUpdateDistance.Value)
        {
            return;
        }
        GetNextSpeedVariablesUpdate();
    }

    private void Accelerate()
    {
        if (turboBoostPercentage > 0)
        {
            if (turboSpeedToReach > 0)
            {
                AccelerateTurbo();
                return;
            }
            DeaccelerateTurbo();
            return;
        }
        float speed = PlayerBossBehaviour.instance.BossfightStarted ? BossfightSpeed : speeds[speedIndex].speed;
        if (currentSpeed == speed)
        {
            isAccelerating = false;
            return;
        }
        isAccelerating = true;
        currentSpeed += acceleration * Time.deltaTime;

        if (currentSpeed > speed)
        {
            currentSpeed = speed;
        }
    }

    private void AccelerateTurbo()
    {
        if (turboSpeedToReach == currentSpeed + turboSpeedToAdd)
        {
            return;
        }
        if (turboSpeedToReach < currentSpeed + turboSpeedToAdd)
        {
            turboSpeedToReach = currentSpeed + turboSpeedToAdd;
            return;
        }
        turboSpeedToAdd += turboAcceleration * Time.deltaTime;
    }

    private void DeaccelerateTurbo()
    {
        if (turboSpeedToAdd <= 0)
        {
            turboBoostPercentage = 0;
            turboAcceleration = 0;
            turboSpeedToAdd = 0f;
            return;
        }
        turboSpeedToAdd -= turboAcceleration * Time.deltaTime;
    }

    public float GetCurrentSpeed(bool firstSpeedIfZero = true)
    {
        if(currentSpeed == 0)
        {
            return speeds[0].speed;
        }
        return currentSpeed + turboSpeedToAdd;
    }

    public float GetChangeLaneSpeed()
    {
        float timeToChange = changeLaneDistance / GetCurrentSpeed();
        return Config.Lane.laneDistance / timeToChange;
    }

    public void TurboStart(int boostPercentage, float secondsToReachMaxSpeed)
    {
        float playerSpeed = speeds[speedIndex].speed;
        turboSpeedToReach = playerSpeed + (playerSpeed * boostPercentage / 100);
        turboBoostPercentage = boostPercentage;
        turboAcceleration = (turboSpeedToReach - playerSpeed) / secondsToReachMaxSpeed;
        turboSpeedToAdd = 0;
    }

    public void TurboEnd()
    {
        //TURBO IS GRADUALY DEACCELERATED. SEE DEACCELERATE METHOD.
        turboSpeedToReach = 0;
    }

    public float GetDistanceToTravelInSeconds(float seconds, int speedBoostPercentage)
    {
        float speed = speeds[speedIndex].speed + (speedBoostPercentage * speeds[speedIndex].speed / 100);
        return speed * seconds;
    }

}

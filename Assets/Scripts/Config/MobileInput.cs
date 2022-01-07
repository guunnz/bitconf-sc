using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobileInput : MonoBehaviour
{
    public static MobileInput instance { get; private set; }

    public int deadZone = 100;

    public bool Tap { get; private set; }
    public bool SwipeLeft { get; private set; }
    public bool SwipeRight { get; private set; }
    public bool SwipeUp { get; private set; }
    public bool SwipeDown { get; private set; }

    public bool DoubleTap { get; private set; }
    private Vector2 SwipeDelta { get; set; }
    private Vector2 StartTouch { get; set; }
    private bool HasPendingInput { get; set; }


    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("More than one MobileInput in scene");
            return;
        }
        instance = this;
    }

    void Update()
    {
        if (HasPendingInput)
        {
            return;
        }
        //
        ManageInputs();
        CalculateSwipeDelta();
        CalculateSwipeDirection();
        HasPendingInput = Tap || SwipeLeft || SwipeRight || SwipeDown || SwipeUp || DoubleTap;
    }

    //private void LateUpdate()
    //{
    //    ResetVariables();
    //}

    private void ManageInputs()
    {
        //KEYBOARD INPUT
        if (Input.GetKeyDown(KeyCode.A))
        {
            SwipeLeft = true;
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            SwipeDown = true;
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            SwipeRight = true;
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            SwipeUp = true;
        }

        if (Input.touchCount != 0)
        {
            if (Input.GetTouch(0).tapCount == 2)
            {
                DoubleTap = true;
            }
        }

        //MOUSE INPUT
        if (Input.GetMouseButtonDown(0))
        {
            Tap = true;
            StartTouch = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            StartTouch = Vector2.zero;
            SwipeDelta = Vector2.zero;
        }

        //MOBILE INPUT
        if (Input.touches.Length == 0)
        {
            return;
        }

        switch (Input.touches[0].phase)
        {
            case TouchPhase.Began:
                Tap = true;
                StartTouch = Input.mousePosition;
                break;
            case TouchPhase.Ended:
            case TouchPhase.Canceled:
                StartTouch = Vector2.zero;
                SwipeDelta = Vector2.zero;
                break;
        }
    }

    private void CalculateSwipeDelta()
    {
        SwipeDelta = Vector2.zero;
        if (StartTouch == Vector2.zero)
        {
            return;
        }

        if (Input.touches.Length > 0)
        {
            SwipeDelta = Input.touches[0].position - StartTouch;
        }
        else if (Input.GetMouseButton(0))
        {
            SwipeDelta = (Vector2)Input.mousePosition - StartTouch;
        }
    }

    private void CalculateSwipeDirection()
    {
        if (SwipeDelta.magnitude <= deadZone)
        {
            return;
        }

        float x = SwipeDelta.x;
        float y = SwipeDelta.y;



        if (Mathf.Abs(x) > Mathf.Abs(y))
        {
            if (x < 0)
            {
                SwipeLeft = true;
            }
            else
            {
                SwipeRight = true;
            }
        }
        else
        {
            if (y < 0)
            {
                SwipeDown = true;
            }
            else
            {
                SwipeUp = true;
            }
        }
        StartTouch = Vector2.zero;
        SwipeDelta = Vector2.zero;
    }


    public void ResetVariables()
    {
        Tap = false;
        SwipeLeft = false;
        SwipeRight = false;
        SwipeDown = false;
        SwipeUp = false;
        HasPendingInput = false;
        DoubleTap = false;
    }
}

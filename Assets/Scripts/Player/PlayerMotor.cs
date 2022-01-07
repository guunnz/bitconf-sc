using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class PlayerMotor : MonoBehaviour
{
    [SerializeField]
    private PlayerSpeed playerSpeed;
    [SerializeField]
    private CharacterController controller;
    [SerializeField]
    internal Animator animator;
    [Header("Player variables")]
    [SerializeField]
    private float jumpForce = 4.0f;
    [SerializeField]
    private float gravity = 12.0f;

    private float verticalPosition;
    private float prevPositionY;

    private Config.Types.Lane desiredLane = Config.Types.Lane.Center;

    private Config.Types.Lane currentLane = Config.Types.Lane.Center;


    private bool jumpWhenGrounded;
    private bool lastMoveRight = false;
    private Vector3 moveVector;

    internal bool Reverse;

    public static PlayerMotor instance { get; private set; }

    public float RotationTurnAroundSpeed = 100;

    public BoxCollider BossObstacleRemoverFrontCollider;
    public BoxCollider BossObstacleRemoverBackCollider;
    internal bool Sliding;

    public float RaycastDistance;
    public Transform EmptyToMoveWithRaycast;
    public LayerMask LayerForRaycast;
    public float CameraLerpT;

    public List<ParticleSystem> Humo;



    private bool lastFrameGrounded = true;
    void Start()
    {
        if (instance != null)
        {
            Debug.LogError("More than one PlayerMotor in scene");
            return;
        }
        instance = this;
        ResetVariables();
   
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            StartCoroutine(IToggleBossfight());
        }

        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.up * -1, out hit, RaycastDistance, LayerForRaycast))
        {

            EmptyToMoveWithRaycast.transform.position = new Vector3(EmptyToMoveWithRaycast.transform.position.x, EmptyToMoveWithRaycast.transform.position.y, transform.position.z);
            EmptyToMoveWithRaycast.transform.position = Vector3.Lerp(EmptyToMoveWithRaycast.transform.position, new Vector3(transform.position.x <= -2f ? -2f : transform.position.x > 2f ? 2f : transform.position.x, System.Math.Round(hit.point.y, 2) < 0 ? 0 : (float)System.Math.Round(hit.point.y, 2), EmptyToMoveWithRaycast.transform.position.z), 0.2f);
        }
        else
        {
            EmptyToMoveWithRaycast.transform.position = Vector3.Lerp(EmptyToMoveWithRaycast.transform.position, new Vector3(transform.position.x <= -2f ? -2f : transform.position.x > 2f ? 2f : transform.position.x, transform.position.y, transform.position.z), 0.2f);
        }
    }

    private void ResetVariables()
    {
        jumpWhenGrounded = false;
    }

    void FixedUpdate()
    {
        if (!playerSpeed.isRunning || playerSpeed.stopPlayer)
        {
            return;
        }
        if (MobileInput.instance.DoubleTap || Input.GetMouseButtonDown(1))
        {
            //CameraManager.instance.CameraUIHelper.SetActive(!CameraManager.instance.CameraUIHelper.activeSelf);

            //shoot;
            //Debug.Log("Shot");
            StartCoroutine(PlayerWeapon.instance.IUseBasicWeapon());
        }
        SetShouldJumpWhenGrounded();
        StartPlayerMovement(false);
        MobileInput.instance.ResetVariables();
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.up * -1, out hit, RaycastDistance, LayerForRaycast))
        {

            EmptyToMoveWithRaycast.transform.position = new Vector3(EmptyToMoveWithRaycast.transform.position.x, EmptyToMoveWithRaycast.transform.position.y, transform.position.z);
            EmptyToMoveWithRaycast.transform.position = Vector3.Lerp(EmptyToMoveWithRaycast.transform.position, new Vector3(transform.position.x <= -2f ? -2f : transform.position.x > 2f ? 2f : transform.position.x, System.Math.Round(hit.point.y, 2) < 0 ? 0 : (float)System.Math.Round(hit.point.y, 2), EmptyToMoveWithRaycast.transform.position.z), 0.2f);
        }
        else
        {
            EmptyToMoveWithRaycast.transform.position = Vector3.Lerp(EmptyToMoveWithRaycast.transform.position, new Vector3(transform.position.x <= -2f ? -2f : transform.position.x > 2f ? 2f : transform.position.x, transform.position.y, transform.position.z), 0.2f);
        }
    }

    private void SetShouldJumpWhenGrounded()
    {
        if (controller.isGrounded)
        {
            return;
        }
        if (MobileInput.instance.SwipeLeft || MobileInput.instance.SwipeRight || MobileInput.instance.SwipeDown)
        {
            jumpWhenGrounded = false;
            return;
        }
        if (MobileInput.instance.SwipeUp && prevPositionY > controller.gameObject.transform.position.y)
        {
            jumpWhenGrounded = true;
        }
        prevPositionY = controller.gameObject.transform.position.y;
    }

    public void StartPlayerMovement(bool forceMove)
    {
        float horizontalPosition = GetLanePosition(forceMove);
        verticalPosition = GetVerticalPosition(verticalPosition);

        float speed = playerSpeed.GetCurrentSpeed();
        MovePlayer(horizontalPosition, verticalPosition, speed);

        switch (Mathf.Round(transform.position.x * 10) / 10)
        {
            case 0:
                currentLane = Config.Types.Lane.Center;
                break;
            case Config.Lane.laneDistance:
                currentLane = Config.Types.Lane.Right;
                break;
            case -Config.Lane.laneDistance:
                currentLane = Config.Types.Lane.Left;
                break;
        }

        if (controller.isGrounded && !lastFrameGrounded)
        {
            animator.SetBool(Config.AnimationTriggers.Player.OnJump, false);
            animator.SetTrigger(Config.AnimationTriggers.Player.Landed);
        }
        lastFrameGrounded = controller.isGrounded;
        MobileInput.instance.ResetVariables();
    }

    private float GetLanePosition(bool forceMove)
    {
        if (forceMove)
        {
            MoveLane(!lastMoveRight);
        }
        else
        {
            if (MobileInput.instance.SwipeLeft)
            {
                MoveLane(false);
            }
            if (MobileInput.instance.SwipeRight)
            {
                MoveLane(true);
            }
        }

        return Config.Lane.GetLanePosition(desiredLane);
    }

    private void MoveLane(bool goingRight)
    {
        lastMoveRight = goingRight;

        if (Reverse)
        {
            goingRight = !goingRight;
        }

        if (goingRight)
        {
            if (!Sliding && desiredLane != Config.Types.Lane.Right && animator.GetCurrentAnimatorClipInfo(0)[0].clip.name != Config.AnimationTriggers.PowerupPickup)
                animator.SetTrigger(Reverse ? Config.AnimationTriggers.Player.GirarIzquierda : Config.AnimationTriggers.Player.GirarDerecha);

            switch (desiredLane)
            {
                case Config.Types.Lane.Center:
                    desiredLane = Config.Types.Lane.Right;
                    AudioManager.DO.Play(Config.Types.SFX.CambioDeCarril);
                    break;
                case Config.Types.Lane.Left:
                    AudioManager.DO.Play(Config.Types.SFX.CambioDeCarril);
                    desiredLane = Config.Types.Lane.Center;
                    break;
            }
        }
        else
        {
            if (!Sliding && desiredLane != Config.Types.Lane.Left && animator.GetCurrentAnimatorClipInfo(0)[0].clip.name != Config.AnimationTriggers.PowerupPickup)
                animator.SetTrigger(!Reverse ? Config.AnimationTriggers.Player.GirarIzquierda : Config.AnimationTriggers.Player.GirarDerecha);
            switch (desiredLane)
            {
                case Config.Types.Lane.Center:
                    AudioManager.DO.Play(Config.Types.SFX.CambioDeCarril);
                    desiredLane = Config.Types.Lane.Left;
                    break;
                case Config.Types.Lane.Right:
                    AudioManager.DO.Play(Config.Types.SFX.CambioDeCarril);
                    desiredLane = Config.Types.Lane.Center;
                    break;
            }
        }
        //desiredLane += goingRight ? Config.Lane.laneDistance : -Config.Lane.laneDistance;
        //desiredLane = Mathf.Clamp(desiredLane, 0, 2);

    }

    private float GetVerticalPosition(float currentPosition)
    {
        float verticalTargetPosition;

        animator.SetBool(Config.AnimationTriggers.Player.Grounded, controller.isGrounded);

        if (controller.isGrounded)
        {
            //ON GROUND
            verticalTargetPosition = -0.1f;
            if (MobileInput.instance.SwipeUp || jumpWhenGrounded)
            {
                animator.SetBool(Config.AnimationTriggers.Player.OnJump, true);
                animator.SetBool(Config.AnimationTriggers.Player.Sliding, false);
                animator.SetTrigger(Config.AnimationTriggers.Player.Jump);
                verticalTargetPosition = jumpForce;
                jumpWhenGrounded = false;
            }
            else if (MobileInput.instance.SwipeDown)
            {
                if (!Sliding)
                    StartCoroutine(StartSliding());
                //Invoke("StopSliding", 1.0f);
            }
        }
        else
        {
            verticalTargetPosition = currentPosition - gravity * Time.deltaTime;
            if (MobileInput.instance.SwipeDown)
            {
                //FAST FALLING MECHANIC
                if (MobileInput.instance.SwipeDown)
                {
                    if (!Sliding)
                        StartCoroutine(StartSliding());
                    //Invoke("StopSliding", 1.0f);
                }
                verticalTargetPosition = -jumpForce;
            }
        }
        return verticalTargetPosition;
    }

    private void MovePlayer(float lanePosition, float verticalPosition, float _speed)
    {
        moveVector = Vector3.zero;
        //moveVector.x = lanePosition;
        moveVector.y = verticalPosition;
        moveVector.z = _speed;
        moveVector *= Time.deltaTime;

        float dirX = Mathf.Sign(lanePosition - transform.position.x);
        float deltaX = playerSpeed.GetChangeLaneSpeed() * dirX * Time.deltaTime;

        // Correct for overshoot
        if (Mathf.Sign(lanePosition - (transform.position.x + deltaX)) != dirX)
        {
            float overshoot = lanePosition - (transform.position.x + deltaX);
            deltaX += overshoot;
        }

        moveVector.x = deltaX;

        if (Reverse)
        {
            moveVector = new Vector3(moveVector.x, moveVector.y, moveVector.z * -1);
        }

        controller.Move(moveVector);

        //Vector3 direction = controller.velocity;
        //if (direction != Vector3.zero)
        //{
        //    direction.y = 0;
        //    transform.forward = Vector3.Lerp(transform.forward, direction, changeLaneSpeed);
        //    //TODO: MoveTowards o Lerp? Antes estaba con Lerp, pero creo que debería estar mal
        //}

    }

    public Vector3 GetMoveVector()
    {
        return moveVector;
    }

    public IEnumerator StartSliding()
    {
        AudioManager.DO.Play(Config.Types.SFX.ColeadaSlideMoto);
        Sliding = true;
        animator.SetBool(Config.AnimationTriggers.Player.Sliding, true);
        if (controller.height > 1)
        {
            controller.height /= 2;
            controller.center = new Vector3(controller.center.x, controller.center.y / 2, controller.center.z);
        }
        yield return new WaitForSeconds(1);
        Sliding = false;
        animator.SetBool(Config.AnimationTriggers.Player.Sliding, false);
        if (controller.height < 1)
        {
            controller.height *= 2;
            controller.center = new Vector3(controller.center.x, controller.center.y * 2, controller.center.z);
        }
    }

    public void StartRunning()
    {
        playerSpeed.isRunning = true;
        animator.SetTrigger(Config.AnimationTriggers.Player.StartRunning);
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public float GetcurrentPositionX()
    {
        return Config.Lane.GetLanePosition(currentLane);
    }

    public float GetDesiredPositionX()
    {
        return Config.Lane.GetLanePosition(desiredLane);
    }

    public float GetJumpHeight()
    {
        float v0 = jumpForce;
        float maxJump_y = (v0 * v0) / (2 * gravity);
        return maxJump_y;
    }

    public float GetJumpTime()
    {
        return ((jumpForce) / (gravity)) * 2;
    }

    public float GetJumpLength()
    {
        return playerSpeed.GetCurrentSpeed() * GetJumpTime();
    }

    public IEnumerator IToggleBossfight()
    {
        while (!controller.isGrounded)
        {
            yield return null;
        }
        if (!Reverse)
        {
            playerSpeed.stopPlayer = true;
            CameraManager.instance.ChangeCamera(CameraType.BossfightFrenada);
            animator.SetTrigger("BossSlide");
            animator.SetBool("StartRunning", false);
            PlayerBossBehaviour.instance.StartBossfight();
            PlayerBossBehaviour.instance.Boss.CanMove = true;
            BossObstacleRemoverBackCollider.enabled = true;
            Reverse = !Reverse;
            CameraManager.instance.ChangeCamera(CameraType.CameraPostFrenada);
            //StartCoroutine(FloorManager.instance.RotateFloors());

            for (int i = 0; i < 60; i++)//30 es la duracion de la animacion de slide boss
            {
                //EmptyToMoveWithRaycast.transform.rotation = transform.rotation;
                //transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y + (Time.deltaTime * RotationTurnAroundSpeed), transform.eulerAngles.z);
                yield return new WaitForFixedUpdate();
            }
            animator.SetTrigger("StopBossSlide");
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, 180, transform.eulerAngles.z);
            SegmentSpawner.instance.SpawnSegmentBack();
            EmptyToMoveWithRaycast.transform.rotation = transform.rotation;
            BossObstacleRemoverFrontCollider.enabled = true;
            animator.SetBool("StartRunning", true);
            ParticlesManager.DO.Play(ParticleType.CameraBoss);
            CameraManager.instance.ChangeCamera(CameraType.Bossfight);
        }
        else
        {
            PlayerBossBehaviour.instance.Boss.BossAnim.SetTrigger(Config.AnimationTriggers.BossStop);
            PlayerBossBehaviour.instance.Boss.CanMove = false;
            yield return new WaitForSeconds(0.5f);
            playerSpeed.stopPlayer = true;
            CameraManager.instance.ChangeCamera(CameraType.PostBossfightFrenada);
            animator.SetBool("StartRunning", false);
            animator.SetTrigger("BossSlide");
            Reverse = !Reverse;
            //StartCoroutine(FloorManager.instance.RotateFloors());

            for (int i = 0; i < 60; i++)//30 es la duracion de la animacion de slide boss
            {
                //EmptyToMoveWithRaycast.transform.rotation = transform.rotation;
                //transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y - (Time.deltaTime * RotationTurnAroundSpeed), transform.eulerAngles.z);
                yield return new WaitForFixedUpdate();
            }
            animator.ResetTrigger("GirarIzq");
            animator.ResetTrigger("GirarDer");
            animator.ResetTrigger("Jump");
            animator.ResetTrigger("Landed");

            PlayerBossBehaviour.instance.EndBossfight();
            ParticlesManager.DO.Stop(ParticleType.CameraBoss);


            transform.eulerAngles = new Vector3(transform.eulerAngles.x, 0, transform.eulerAngles.z);
            EmptyToMoveWithRaycast.transform.rotation = transform.rotation;

            yield return new WaitForSeconds(0.05f);
            SegmentSpawner.instance.SpawnSegmentFront();
            yield return new WaitForSeconds(0.05f);
            animator.SetTrigger("StopBossSlide");
            animator.SetBool("StartRunning", true);
            BossObstacleRemoverFrontCollider.enabled = false;
            BossObstacleRemoverBackCollider.enabled = false;
            CameraManager.instance.ChangeCamera(CameraType.Normal);
        }
        playerSpeed.stopPlayer = false;
    }
}
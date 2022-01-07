using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyChasing : MonoBehaviour
{

    [SerializeField]
    private float distanceToPlayer;

    [SerializeField]
    private GameObject camera;


    public GameObject AttackPrefab;

    [SerializeField]
    private float appearingDuration = 0.5f;


    [SerializeField]
    private float introDuration = 2f;

    private bool isAppearing = false;
    private bool isChasing = true;

    private int nextDestroyCheck = 1;

    private List<Vector3> followPositions;

    public static EnemyChasing instance { get; private set; }

    public Animator Anim;

    private GameObject SpawnedAttack;

    void Start()
    {
        if (instance != null)
        {
            Debug.LogError("More than one PlayerMotor in scene");
            return;
        }
        instance = this;

        followPositions = new List<Vector3>();
        StartCoroutine(Show(false));
    }

    public void OnEnable()
    {
        if (PlayerMotor.instance == null)
        {
            return;
        }
        isChasing = true;
        StartCoroutine(Show());
    }

    public void Update()
    {
        if (isChasing)
        {
            return;
        }
        if (Time.time >= nextDestroyCheck)
        {
            nextDestroyCheck = Mathf.FloorToInt(Time.time) + 1;
            if (this.transform.position.z < camera.gameObject.transform.position.z)
            {
                this.gameObject.SetActive(false);
            }
        }
    }

    public void FixedUpdate()
    {
        followPositions.Add(PlayerMotor.instance.GetPosition());

        if (isAppearing || !isChasing)
        {
            return;
        }
        transform.position = followPositions[0];
        followPositions.RemoveAt(0);
        RemoveOldPositions();
    }

    private IEnumerator Shoot()
    {
        Debug.Log("Charging ATTACK");
        yield return new WaitForSeconds(PlayerCollision.instance.GetInjureDuration());

        SpawnedAttack = Instantiate(AttackPrefab, this.transform.position, Quaternion.identity, null);

        yield return new WaitForSeconds(0.1f);
        Destroy(SpawnedAttack);
    }

    private IEnumerator Show(bool shoot = true)
    {
        yield return new WaitForFixedUpdate();
        AudioManager.DO.Play(Config.Types.SFX.TrexVoice);
        if (shoot)
            StartCoroutine(Shoot());
        isAppearing = true;
        Vector3 startPosition = new Vector3(PlayerMotor.instance.GetDesiredPositionX(), 0, PlayerMotor.instance.GetPosition().z - distanceToPlayer);
        this.gameObject.transform.position = startPosition;
        float duration = 0;
        while (duration < appearingDuration)
        {
            duration += Time.deltaTime;
            Vector3 playerPosition = PlayerMotor.instance.GetPosition();
            Vector3 destination = new Vector3(PlayerMotor.instance.GetDesiredPositionX(), 0, playerPosition.z - distanceToPlayer);
            transform.position = Vector3.Lerp(transform.position, destination, duration);
            yield return null;
        }
        isAppearing = false;

        RemoveOldPositions();
        if (!shoot)
        {
            yield return new WaitForSeconds(introDuration);
            EnemyChasing.instance.StopChasing();
        }
    }

    private IEnumerator SetFollowPositions(float wait)
    {
        yield return new WaitForSecondsRealtime(wait);
    }

    private void RemoveOldPositions()
    {
        followPositions.RemoveAll(x => x.z < PlayerMotor.instance.GetPosition().z - distanceToPlayer);
    }

    public void StartChasing()
    {
        this.gameObject.SetActive(true);
    }

    public void StopChasing()
    {
        isChasing = false;
        //this.gameObject.SetActive(false);
    }
}
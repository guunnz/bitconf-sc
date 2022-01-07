using UnityEngine;

public class CoinSpawner : MonoBehaviour
{

    public GameObject maxJump;
    public GameObject startJump;
    public GameObject endJump;



    private void OnEnable()
    {
        float y = PlayerMotor.instance.GetJumpHeight();
        float lenght = PlayerMotor.instance.GetJumpLength();
        maxJump.transform.position = new Vector3(this.transform.position.x, this.transform.position.y + y, this.transform.position.z + (lenght / 2));
        startJump.transform.position = this.transform.position;
        endJump.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z + lenght);
    }
    //public int maxCoin = 5;

    //public float changeToSpawn = 0.5f;
    //public bool forceSpawnAll = false;

    //private GameObject[] coins;


    //private void Awake()
    //{
    //    coins = new GameObject[transform.childCount];
    //    for (int i = 0; i < transform.childCount; i++)
    //    {
    //        coins[i] = transform.GetChild(i).gameObject;
    //    }
    //    DisableCoins();
    //}

    //private void OnEnable()
    //{
    //    if (Random.Range(0f, 1f) > changeToSpawn)
    //    {
    //        return;
    //    }

    //    int coinsToSpawn = maxCoin;
    //    if (!forceSpawnAll)
    //    {
    //        coinsToSpawn = Random.Range(0, maxCoin + 1);
    //    }

    //    for (int i = 0; i < coinsToSpawn; i++)
    //    {
    //        coins[i].SetActive(true);
    //    }

    //}

    //private void DisableCoins()
    //{
    //    foreach (GameObject goCoin in coins)
    //    {
    //        goCoin.SetActive(false);
    //    }
    //}

    //private void OnDisable()
    //{
    //    DisableCoins();
    //}
}


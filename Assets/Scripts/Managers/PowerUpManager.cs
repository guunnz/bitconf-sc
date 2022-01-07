using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum ePowerUp
{
    None = 0,
    Turbo = 1,
    Iman = 2,
    Escudo = 3,
    BigCoin = 4,
    CoinMultiplier = 5,
    ScoreMultiplier = 6,
}

[System.Serializable]
public class PowerUp
{
    public int Cost;
    public ePowerUp _PowerUp;
}

public class PowerUpManager : MonoBehaviour
{
    List<PowerUp> PowerUpList = new List<PowerUp>();

    public void Purchase(int powerUp)
    {
        PowerUp power = PowerUpList.Single(x => x._PowerUp == (ePowerUp)powerUp);

        PlayerPrefs.SetInt(((ePowerUp)powerUp).ToString(), PlayerPrefs.GetInt(((ePowerUp)powerUp).ToString()) + 1);
    }
}
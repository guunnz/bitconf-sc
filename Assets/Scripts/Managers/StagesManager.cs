using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StagesManager : MonoBehaviour
{
    public int MoonUnlockPrice = 250;

    public int RocketUnlockPrice = 250;

    public GameObject UnlockMoonConfirm;
    public GameObject UnlockRocketConfirm;
    public GameObject Congratz;

    public void UnlockMoon()
    {
        if (PlayerPrefs.GetInt(PlayerPrefValues.Coin) >= MoonUnlockPrice)
        {
            PlayerPrefs.SetInt(PlayerPrefValues.Moon, 1);
            Congratz.SetActive(true);
            Invoke("DisableCongratz", 0.8f);
        }
    }

    public void RocketUnlock()
    {
        if (PlayerPrefs.GetInt(PlayerPrefValues.Coin) >= RocketUnlockPrice)
        {
            PlayerPrefs.SetInt(PlayerPrefValues.Rocket, 1);
            Congratz.SetActive(true);
            Invoke("DisableCongratz", 0.8f);
        }
    }

    public void DisableCongratz()
    {
        Congratz.SetActive(false);
    }

    public void ConfirmRocketUnlock()
    {
        UnlockRocketConfirm.SetActive(true);
    }

    public void ConfirmMoonUnlock()
    {
        UnlockMoonConfirm.SetActive(true);
    }

    public void SelectRocket()
    {
        if (PlayerPrefs.GetInt(PlayerPrefValues.Rocket) != 0)
        {
            SceneManager.LoadScene(3);
        }
        else
        {
            ConfirmRocketUnlock();
        }
    }

    public void SelectMoon()
    {
        if (PlayerPrefs.GetInt(PlayerPrefValues.Moon) != 0)
        {
            SceneManager.LoadScene(2);
        }
        else
        {
            ConfirmMoonUnlock();
        }
    }

    public void SelectEarth()
    {
        SceneManager.LoadScene(1);
    }
}
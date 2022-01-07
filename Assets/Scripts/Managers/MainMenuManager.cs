using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System;

public class MainMenuManager : MonoBehaviour
{
    public TextMeshProUGUI DiamondsText;
    public TextMeshProUGUI CoinsText;
    public TextMeshProUGUI PizzasText;
    public GameObject ClosegamePopup;
    public GameObject MainMenuCanvas;
    public GameObject AvatarCanvas;
    public GameObject VehiclesCanvas;
    public GameObject ShopCanvas;
    public GameObject PowerupCanvas;
    public GameObject StagesCanvas;
    public GameObject LeaderboardCanvas;
    public GameObject TipsCanvas;
    static public MainMenuManager instance;
    private bool CanLoad = false;

    private bool OnMain = true;
    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        StartCoroutine(LoadGameAsync());
        UpdateScores();
    }
    public void UpdateScores()
    {
        int score = 0;
        int lastScore = 0;
        int modifierScore = 1;
        int bonusCoinScore = PlayerPrefs.GetInt(PlayerPrefValues.BonusCoin);
        int coinScore = 0;
        int goldScore = 0;
        coinScore = PlayerPrefs.GetInt(PlayerPrefValues.Coin);
        if (bonusCoinScore < 0)
        {
            bonusCoinScore = 0;
        }
        UIManager.instance.UpdateGameScore(Convert.ToInt32(score));
        UIManager.instance.UpdateCoinScore(coinScore);
        UIManager.instance.UpdateBonusCoinScore(bonusCoinScore);
        UIManager.instance.UpdateGoldScore(goldScore);
        UIManager.instance.UpdateHighScoreMainMenu(PlayerPrefs.GetInt("Highscore"));
    }
    IEnumerator LoadGameAsync()
    {
        // The Application loads the Scene in the background as the current Scene runs.
        // This is particularly good for creating loading screens.
        // You could also load the Scene by using sceneBuildIndex. In this case Scene2 has
        // a sceneBuildIndex of 1 as shown in Build Settings.

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(1);
        asyncLoad.allowSceneActivation = false;
        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone && !CanLoad)
        {
            yield return null;
        }
        asyncLoad.allowSceneActivation = true;
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            BackButton();
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            PlayerPrefs.DeleteAll();
        }
    }

    public void BackButton()
    {
        if (OnMain)
        {
            CloseGamePopup();
            return;
        }
        else
        {
            ResetAllCanvas();
            MainMenu();
        }
    }

    public void ResetAllCanvas()
    {
        ClosegamePopup.SetActive(false);
        MainMenuCanvas.SetActive(false);
        AvatarCanvas.SetActive(false);
        VehiclesCanvas.SetActive(false);
        ShopCanvas.SetActive(false);
        PowerupCanvas.SetActive(false);
        StagesCanvas.SetActive(false);
        LeaderboardCanvas.SetActive(false);
        TipsCanvas.SetActive(false);
    }


    public void RunAd()
    {

    }

    public void CloseGamePopup()
    {
        ClosegamePopup.SetActive(true);
    }

    public void Tips()
    {
        TipsCanvas.SetActive(true);
    }


    public void MainMenu()
    {
        MainMenuCanvas.SetActive(true);
    }

    public void Leaderboard()
    {
        ResetAllCanvas();
        LeaderboardCanvas.SetActive(true);
    }

    public void Shop()
    {
        OnMain = false;
        ResetAllCanvas();
        ShopCanvas.SetActive(true);
    }

    public void Stages()
    {
        OnMain = false;
        ResetAllCanvas();
        StagesCanvas.SetActive(true);
    }

    public void Avatar()
    {
        ResetAllCanvas();
        OnMain = false;
        AvatarCanvas.SetActive(true);
    }

    public void Cerrar()
    {
        Application.Quit();
    }

    public void StartGame()
    {
        CanLoad = true;
    }
}

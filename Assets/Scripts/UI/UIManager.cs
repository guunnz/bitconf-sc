using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{

    public Text FPSText;

    [Header("MainMenu")]
    public GameObject mainMenuUI;
    public Text highscoreMainMenuText;

    [Header("GameUI")]
    public GameObject gameUI;
    public GameObject postGamePanel;
    public Animator diamondAnimator;
    public Text scoreText;
    public Text coinText;
    public Text bonusText;
    public Text goldText;

    [Header("Collectibles")]
    public GameObject collectibleContainer;
    public CollectibleUI coinMagnetProgressBar;
    public CollectibleUI coinMultiplierProgressBar;
    public CollectibleUI scoreMultiplierProgressBar;
    public CollectibleUI turboProgressBar;
    public CollectibleUI shieldProgressBar;


    [Header("PauseMenu")]
    public GameObject pauseMenuUI;
    public GameObject pausePanel;
    public GameObject resumingPanel;
    public GameObject warningQuitGamePanel;
    public Animator resumeSecondsAnimator;
    public Text resumingSecondsText;
    public int resumeSeconds = 3;

    [Header("DeathMenu")]
    public GameObject deathMenuUI;
    public Animator deathMenuAnimator;
    //public TextMeshProUGUI deadScoreText;
    public Text deadCoinText;
    //public TextMeshProUGUI newHighscore;
    public TextMeshProUGUI deathMenuScore;
    public GameObject NotHighscoreObj;
    public GameObject NewRecordObj;

    public List<Sprite> PizzaImages;
    public Image PizzaIMG;

    public static UIManager instance;

    internal bool Paused;

    private int TimesPizza;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("More than one UIManager in scene");
            return;
        }
        instance = this;

    }

    private void Start()
    {
        //GameManager.instance.BonusCoinCollected(7);
    }

    public void Update()
    {
        float current = (int)(1f / Time.unscaledDeltaTime);
        //current = Time.frameCount / Time.time;
        if (FPSText == null)
            return;
        FPSText.text = "FPS " + ((int)current).ToString();
    }

    public void GameStarted()
    {
        gameUI.SetActive(true);
        mainMenuUI.SetActive(false);
    }

    public void UpdateGameScore(int score)
    {
        scoreText.text = score.ToString();
    }

    public void UpdateCoinScore(int coins)
    {
        coinText.text = coins.ToString();
        //diamondAnimator.SetTrigger(Config.AnimationTriggers.GameMenu.DiamondCollect);
    }

    public void UpdateGoldScore(int gold)
    {
        goldText.text = gold.ToString();
    }

    public void UpdateBonusCoinScore(int bonus)
    {
        if (bonus < 0)
        {
            return;
        }
        bonusText.text = bonus.ToString();
        if (PizzaImages.Count == 0)
        {
            return;
        }
        if (PizzaImages.Count >= bonus && bonus < 9)
            PizzaIMG.sprite = PizzaImages[bonus];


    }


    public void UpdateHighScoreMainMenu(int highScore)
    {
        if (highscoreMainMenuText == null)
            return;
        highscoreMainMenuText.text = "Highscore " + highScore;
    }

    public void PlayerIsDeath(int score, int coins)
    {
        deadCoinText.text = coins.ToString();
        deathMenuScore.text = score.ToString();
        deathMenuUI.SetActive(true);
        gameUI.SetActive(false);
        CheckHighscore(score);
    }

    private void CheckHighscore(int score)
    {
        //newHighscore.enabled = false;
        if (score > PlayerPrefs.GetInt("Highscore"))
        {
            NewRecordObj.SetActive(true);
            PlayerPrefs.SetInt("Highscore", score);
            //newHighscore.enabled = true;
            deathMenuScore.text = PlayerPrefs.GetInt("Highscore").ToString("0");
        }
        else
        {
            NotHighscoreObj.SetActive(true);
            deathMenuScore.text = score.ToString("0");
        }
    }

    public void OnPlay_Click()
    {
        Time.timeScale = 1f;
        GameManager.instance.Restart();
    }

    public void OnPauseToggle_Click()
    {

        if (Time.timeScale == 1 && !Paused)
        {
            Paused = true;
            pauseMenuUI.SetActive(!pauseMenuUI.activeSelf);
            resumingPanel.SetActive(false);
            pausePanel.SetActive(!pausePanel.activeSelf);
            Time.timeScale = 0.0000000000000001f;
        }
        else if (Paused)
        {
            Paused = false;
            pausePanel.SetActive(false);
            resumingPanel.SetActive(true);
            ResumeCountdown(resumeSeconds);
        }
    }

    public void OnPause_Click()
    {
        if (mainMenuUI.activeSelf || deathMenuUI.activeSelf || pauseMenuUI.activeSelf)
        {
            return;
        }
        Paused = true;
        pauseMenuUI.SetActive(true);
        resumingPanel.SetActive(false);
        pausePanel.SetActive(true);
        Time.timeScale = 0f;
    }


    public void OnQuit_Click()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
        //pausePanel.SetActive(false);
        //warningQuitGamePanel.SetActive(true);
    }

    public void OnQuitWarningBack_Click()
    {
        pausePanel.SetActive(true);
        warningQuitGamePanel.SetActive(false);
    }

    public void OnQuitWarning_Click()
    {
        GameManager.instance.GoToMenu();
    }

    public void Continue_Click()
    {
        pausePanel.SetActive(false);
        resumingPanel.SetActive(true);
        ResumeCountdown(resumeSeconds);
    }

    public async void ResumeCountdown(int resumeSeconds)
    {
        while (resumeSeconds > 0)
        {
            resumingSecondsText.text = resumeSeconds.ToString();
            resumeSecondsAnimator.SetTrigger(Config.AnimationTriggers.PauseMenu.Show);
            await Task.Delay(TimeSpan.FromSeconds(1));
            resumeSeconds--;
        }

        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        Paused = false;
    }

    public void ShowCollectible(Config.Types.Collectible.CollectibleType collectibleType, float duration)
    {
        switch (collectibleType)
        {
            case Config.Types.Collectible.CollectibleType.CoinMagnet:
                coinMagnetProgressBar.StartSlider(duration);
                break;
            case Config.Types.Collectible.CollectibleType.CoinMultiplier:
                coinMultiplierProgressBar.StartSlider(duration);
                break;
            case Config.Types.Collectible.CollectibleType.ScoreMultiplier:
                scoreMultiplierProgressBar.StartSlider(duration);
                break;
            case Config.Types.Collectible.CollectibleType.Shield:
                shieldProgressBar.StartSlider(duration);
                break;
            case Config.Types.Collectible.CollectibleType.Turbo:
                turboProgressBar.StartSlider(duration);
                break;
        }
    }


    public void HideCollectible(Config.Types.Collectible.CollectibleType collectibleType)
    {
        switch (collectibleType)
        {
            case Config.Types.Collectible.CollectibleType.CoinMagnet:
                coinMagnetProgressBar.gameObject.SetActive(false);
                break;
            case Config.Types.Collectible.CollectibleType.CoinMultiplier:
                coinMultiplierProgressBar.gameObject.SetActive(false);
                break;
            case Config.Types.Collectible.CollectibleType.ScoreMultiplier:
                scoreMultiplierProgressBar.gameObject.SetActive(false);
                break;
            case Config.Types.Collectible.CollectibleType.Shield:
                shieldProgressBar.gameObject.SetActive(false);
                break;
            case Config.Types.Collectible.CollectibleType.Turbo:
                turboProgressBar.gameObject.SetActive(false);
                break;
        }
    }

}

using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

public enum Characters
{
    None = 0,
    Girl = 1,
    Boy = 2,
    Robot = 3
}

[System.Serializable]
public class Character
{
    public List<GameObject> ObjectsToTurnOn;
    public Characters character;
}

public class GameManager : MonoBehaviour
{

    public static GameManager instance { get; private set; }


    [HideInInspector]
    public bool IsGameStarted = false;

    private PlayerMotor playerMotor;
    private float lastScore;
    private float score;
    private int coinScore;
    private float modifierScore;
    private int goldScore;
    private int bonusCoinScore;

    public List<Character> CharacterList;

    public Characters CharacterSelected;

    private bool IsDead { get; set; }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("More than one GameManager in scene");
            return;
        }
        instance = this;
        playerMotor = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMotor>();
        UpdateScores();
        Application.targetFrameRate = 300;
    }

    public void Start()
    {
        Characters _characterSelected = (Characters)PlayerPrefs.GetInt(PlayerPrefValues.Character);

        if (_characterSelected != Characters.None)
        {
            CharacterSelected = _characterSelected;
        }

        Character character = CharacterList.FirstOrDefault(x => x.character == CharacterSelected);

        if (character == null)
            Debug.LogError("Character Not Selected");

        character.ObjectsToTurnOn.ForEach(x => x.SetActive(true));
    }

    private void Update()
    {
        if (IsDead)
        {
            return;
        }
        if (!IsGameStarted)
        {
            //if (MobileInput.instance.Tap)
            //{
            IsGameStarted = true;
            playerMotor.StartRunning();
            UIManager.instance.GameStarted();
            FindObjectOfType<CameraMotor>().IsMoving = true;
            //}
        }
        else if (!PlayerBossBehaviour.instance.BossfightStarted)
        {
            score += (Time.deltaTime * modifierScore) * PlayerCollectible.instance.scoreMultiplierValue;
            if (score != lastScore)
            {
                UIManager.instance.UpdateGameScore(Convert.ToInt32(score));
                lastScore = score;
            }
        }
    }

    public void UpdateScores()
    {
        score = 0;
        lastScore = 0;
        modifierScore = 200;
        bonusCoinScore = PlayerPrefs.GetInt(PlayerPrefValues.BonusCoin);
        coinScore = PlayerPrefs.GetInt(PlayerPrefValues.Coin);

        if (bonusCoinScore < 0)
        {
            bonusCoinScore = 0;
        }
        else if (bonusCoinScore >= 8)
        {
            bonusCoinScore = 0;
        }

        goldScore = PlayerPrefs.GetInt("Gold");

        UIManager.instance.UpdateGameScore(Convert.ToInt32(score));
        UIManager.instance.UpdateCoinScore(coinScore);
        UIManager.instance.UpdateBonusCoinScore(bonusCoinScore);
        UIManager.instance.UpdateGoldScore(goldScore);
        UIManager.instance.UpdateHighScoreMainMenu(PlayerPrefs.GetInt("Highscore"));
    }

    public void UpdateModifier(float modifierAmount)
    {
        modifierScore = 1 + modifierAmount;
    }

    public void CoinCollected(int coinAmount)
    {
        coinScore += coinAmount;
        PlayerPrefs.SetInt(PlayerPrefValues.Coin, coinScore);
        UIManager.instance.UpdateCoinScore(coinScore);
    }

    public void GoldCollected(int goldAmount)
    {
        goldScore += goldAmount;
        if (goldScore < 0)
        {
            goldScore = 0;
        }

        PlayerPrefs.SetInt("Gold", goldScore);

        UIManager.instance.UpdateGoldScore(goldScore);
    }

    public void BonusCoinCollected(int bonusCoinAmount)
    {
        bonusCoinScore += bonusCoinAmount;
        PlayerPrefs.SetInt(PlayerPrefValues.BonusCoin, bonusCoinScore);
        UIManager.instance.UpdateBonusCoinScore(bonusCoinScore);
    }


    public int GetBonusCoinCollected()
    {
        return bonusCoinScore;
    }

    public void EnableBossfight()
    {

    }

    public void Restart()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Earth");
    }

    public void OnDeath()
    {
        CameraManager.instance.ChangeCamera(CameraType.Death);
        CameraManager.instance.DoNoise(2, 2, 0.04f);
        GameManager.instance.IsDead = true;

        Invoke("OpenPostGameUI", 1.1f);
    }

    public void OpenPostGameUI()
    {
        UIManager.instance.PlayerIsDeath(Convert.ToInt32(score), coinScore);
    }

    internal void GoToMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}

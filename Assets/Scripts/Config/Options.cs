using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Options : MonoBehaviour
{
    public AudioManager AM;

    public GameObject OptionsObj;

    public UnityEngine.UI.Toggle sfxToggle;
    public UnityEngine.UI.Toggle musicToggle;


    public void ToggleSFX()
    {
        //int sfx = PlayerPrefs.GetInt(PlayerPrefValues.SFX);

        PlayerPrefs.SetInt(PlayerPrefValues.SFX, sfxToggle.isOn ? 1 : 0);

        if (AM != null)
        {
            AudioManager.DO.SetVolumes();
        }

    }

    public void Toggle()
    {
        if (OptionsObj.activeSelf && UIManager.instance.Paused)
        {
            OptionsObj.SetActive(false);
        }

        else if (!OptionsObj.activeSelf && !UIManager.instance.Paused && Time.timeScale == 1)
        {
            OptionsObj.SetActive(true);
        }
    }

    public void ToggleMainMenu()
    {
        OptionsObj.SetActive(!OptionsObj.activeSelf);
    }

    public void ToggleMusic()
    {
        //int music = PlayerPrefs.GetInt(PlayerPrefValues.Music);

        PlayerPrefs.SetInt(PlayerPrefValues.Music, musicToggle.isOn ? 1 : 0);
        if (AM != null)
        {
            AudioManager.DO.SetVolumes();
        }
    }
}
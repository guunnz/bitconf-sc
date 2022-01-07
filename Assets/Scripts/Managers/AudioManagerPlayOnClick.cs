using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioManagerPlayOnClick : MonoBehaviour
{
    public Config.Types.SFX sfx;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(() => Play());
    }

    public void Play()
    {
        AudioManager.DO.Play(sfx);
    }
}

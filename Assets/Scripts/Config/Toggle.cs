using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Toggle : MonoBehaviour
{
    [SerializeField] RectTransform uiHandleRectTransform;

    UnityEngine.UI.Toggle toggle;

    Vector2 handlePosition;

    [SerializeField] Color backgroundActiveColor;
    [SerializeField] Color handleActiveColor;

    public Sprite On;
    public Sprite Off;

    public bool IsSFX;

    Color backgroundDefaultColor, handleDefaultColor;
    Image backgroundImage, handleImage;

    private bool Initialized;

    private void OnEnable()
    {
        if (!Initialized)
        {
            Initialized = true;
            toggle = GetComponent<UnityEngine.UI.Toggle>();

            handlePosition = uiHandleRectTransform.anchoredPosition;

            backgroundImage = uiHandleRectTransform.parent.GetComponent<Image>();
            handleImage = uiHandleRectTransform.GetComponent<Image>();

            backgroundDefaultColor = backgroundImage.color;
            handleDefaultColor = handleImage.color;


            bool OnSwitchToggle = IsSFX ? PlayerPrefs.GetInt(PlayerPrefValues.SFX) == 1 : PlayerPrefs.GetInt(PlayerPrefValues.Music) == 1;

            toggle.isOn = OnSwitchToggle;
            OnSwitch(OnSwitchToggle);
            toggle.onValueChanged.AddListener(OnSwitch);
        }
    }

    void OnSwitch(bool on)
    {
        //uiHandleRectTransform.anchoredPosition = on ? handlePosition * -1 : handlePosition;
        //backgroundImage.color = on ? backgroundActiveColor : backgroundDefaultColor;
        //uiHandleRectTransform.DOAnchorPos(on ? handlePosition * -1 : handlePosition, .4f).SetEase(Ease.InOutBack);
        //backgroundImage.DOColor(on ? backgroundActiveColor : backgroundDefaultColor, .4f);
        //handleImage.color = on ? handleActiveColor : handleDefaultColor;

        backgroundImage.sprite = on ? On : Off;

        AudioManager.DO.Play(Config.Types.SFX.BotonSeleccionar);
    }

    private void OnDestroy()
    {
        toggle.onValueChanged.RemoveListener(OnSwitch);
    }
}
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AvatarManager : MonoBehaviour
{
    public List<SkinScriptableObject> AllSkins;
    public Transform HeadParent;
    public Transform BodyParent;
    public Transform LegsParent;
    public Transform ShoesParent;
    public GameObject SkinPrefab;
    static public AvatarManager instance;
    public GameObject UnlockPanel;
    public TextMeshProUGUI UnlockText;
    private Skin SkinToUnlock;
   

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        SpawnProps();
        SelectType(0);
    }

    public void SelectSkin(Skin skin)
    {
        SkinToUnlock = skin;
    }

    public void AskForUnlock()
    {
        if (SkinToUnlock == null)
            return;
        UnlockPanel.SetActive(true);
        UnlockText.text = "Do you want to buy " + SkinToUnlock.SkinSelected.Name + " for " + SkinToUnlock.SkinSelected.Cost + " coins";
    }

    public void Unlock()
    {
        //if (SkinToUnlock.Cost <= currentmoney)
        PlayerPrefs.SetInt("Skin" + SkinToUnlock.SkinSelected.ID.ToString(), 1);
        UnlockPanel.SetActive(false);
        SkinToUnlock.Unlocked = true;
        SkinToUnlock.Candado.SetActive(false);
    }

    public void Quit()
    {
        MainMenuManager.instance.ResetAllCanvas();
        MainMenuManager.instance.MainMenu();
    }

    public void SelectType(int skinType)
    {
        BodyParent.gameObject.SetActive(false);
        HeadParent.gameObject.SetActive(false);
        LegsParent.gameObject.SetActive(false);
        ShoesParent.gameObject.SetActive(false);
        switch ((SkinType)skinType)
        {
            case SkinType.Body:
                BodyParent.gameObject.SetActive(true);
                break;
            case SkinType.Head:
                HeadParent.gameObject.SetActive(true);
                break;
            case SkinType.Legs:
                LegsParent.gameObject.SetActive(true);
                break;
            case SkinType.Shoes:
                ShoesParent.gameObject.SetActive(true);
                break;
        }
    }

    public void SpawnProps()
    {
        foreach (SkinScriptableObject skin in AllSkins)
        {
            GameObject spawned = Instantiate(SkinPrefab, Vector3.zero, Quaternion.identity, null);
            Skin _skin = spawned.GetComponent<Skin>();
            _skin.SkinSelected = skin;
            _skin.OnSpawn();

            switch (skin.Type)
            {
                case SkinType.Body:
                    spawned.transform.parent = BodyParent;
                    break;
                case SkinType.Head:
                    spawned.transform.parent = HeadParent;
                    break;
                case SkinType.Legs:
                    spawned.transform.parent = LegsParent;
                    break;
                case SkinType.Shoes:
                    spawned.transform.parent = ShoesParent;
                    break;
            }
        }
    }
}
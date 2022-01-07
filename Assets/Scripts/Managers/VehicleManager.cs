using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using UnityEngine.UI;

public class VehicleManager : MonoBehaviour
{
    public List<VehiclesScriptableObject> AllVehicles;
    public GameObject VehiclePrefab;
    static public VehicleManager instance;
    public GameObject UnlockPanel;
    public TextMeshProUGUI UnlockText;
    private Vehicle SkinToUnlock;
    public Transform VehiclesParent;
    public Image CurrentSkin;

    private List<Vehicle> vehicleList;
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        SpawnProps();
        int id = PlayerPrefs.GetInt("VehicleSelected");
        CurrentSkin.sprite = AllVehicles.Single(x => x.ID == (id == 0 ? 6 : id)).VehicleSprite;
    }

    public void SelectSkin(Vehicle vehicle)
    {
        SkinToUnlock = vehicle;
        CurrentSkin.sprite = vehicle.VehicleSelected.VehicleSprite;
    }

    public void AskForUnlock()
    {
        if (SkinToUnlock == null)
            return;

        if (PlayerPrefs.GetInt("Skin" + SkinToUnlock.VehicleSelected.ID.ToString()) == 1)
        {
            return;
        }

        UnlockPanel.SetActive(true);
        UnlockText.text = "Do you want to buy " + SkinToUnlock.VehicleSelected.Name + " for " + SkinToUnlock.VehicleSelected.Cost + " coins";
    }

    public void Unlock()
    {
        //if (SkinToUnlock.Cost <= currentmoney)
        PlayerPrefs.SetInt("Skin" + SkinToUnlock.VehicleSelected.ID.ToString(), 1);
        UnlockPanel.SetActive(false);
        SkinToUnlock.Unlocked = true;
        SkinToUnlock.Candado.SetActive(false);
    }

    public void Quit()
    {
        MainMenuManager.instance.ResetAllCanvas();
        MainMenuManager.instance.MainMenu();
    }

    public void SpawnProps()
    {
        foreach (VehiclesScriptableObject vehicle in AllVehicles)
        {
            GameObject spawned = Instantiate(VehiclePrefab, Vector3.zero, Quaternion.identity, VehiclesParent);
            Vehicle _vehicle = spawned.GetComponent<Vehicle>();
            _vehicle.VehicleSelected = vehicle;
            _vehicle.OnSpawn();


            int unlocked = PlayerPrefs.GetInt("Skin" + _vehicle.VehicleSelected.ID.ToString());
            if (unlocked == 1)
            {
                _vehicle.Unlocked = true;
                _vehicle.Candado.SetActive(false);
            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Vehicle : MonoBehaviour
{
    internal VehiclesScriptableObject VehicleSelected;
    public GameObject Candado;
    public bool Unlocked = false;

    public void OnSpawn()
    {
        GetComponent<MeshFilter>().mesh = VehicleSelected.Mesh;
        GetComponent<MeshRenderer>().materials = VehicleSelected.Materials.ToArray();
        if (PlayerPrefs.GetInt("Skin" + VehicleSelected.ID.ToString()) == 1)
        {
            Candado.SetActive(false);
            Unlocked = true;
        }
    }

    public void Select()
    {
        if (!Unlocked)
        {
            VehicleManager.instance.SelectSkin(this);
        }
        else
        {
            VehicleManager.instance.CurrentSkin.sprite = VehicleSelected.VehicleSprite;
            PlayerPrefs.SetInt("VehicleSelected", VehicleSelected.ID);
        }
    }
}
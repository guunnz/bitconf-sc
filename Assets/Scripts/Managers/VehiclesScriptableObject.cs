using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Vehicle", menuName = "ScriptableObjects/Create Vehicle", order = 1)]
public class VehiclesScriptableObject : ScriptableObject
{
    public int ID;
    public string Name;
    public Sprite VehicleSprite;
    public Mesh Mesh;
    public Material[] Materials;
    public int Cost;

    public void UpdateCounter()
    {
        ID = PlayerPrefs.GetInt("VehicleSkinID") + 1;
        PlayerPrefs.SetInt("VehicleSkinID", ID);
    }
}
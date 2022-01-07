using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public enum eVehicle
{
    Moto = 6,
    Monopatin = 7,
    Hoverboard = 8,
    CafeRacer = 9
}


[System.Serializable]
public class GameVehicle
{
    public GameObject Vehicle;
    public eVehicle Type;
    public Animator Animator;
}

public class VehicleChooserManager : MonoBehaviour
{
    public List<GameVehicle> Vehicles;

    public GameVehicle VehicleSelected;

    static public VehicleChooserManager instance;



    void Start()
    {
        instance = this;
        int car = PlayerPrefs.GetInt("VehicleSelected");
        VehicleSelected = Vehicles.Single(x => (int)x.Type == (car == 0 ? 6 : car));
        VehicleSelected.Vehicle.SetActive(true);
        PlayerMotor.instance.animator = VehicleChooserManager.instance.VehicleSelected.Animator;
    }
}
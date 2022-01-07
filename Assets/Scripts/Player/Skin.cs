using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Skin : MonoBehaviour
{
    internal SkinScriptableObject SkinSelected;
    public GameObject Candado;
    public bool Unlocked = false;

    public void OnSpawn()
    {
        GetComponent<MeshFilter>().mesh = SkinSelected.Mesh;
        GetComponent<MeshRenderer>().materials = SkinSelected.Materials.ToArray();
        if (PlayerPrefs.GetInt("Skin" + SkinSelected.ID.ToString()) == 1)
        {
            Candado.SetActive(false);
            Unlocked = true;
        }
    }

    public void Select()
    {
        if (!Unlocked)
        {
            AvatarManager.instance.SelectSkin(this);
        }
        else
        {
            //Equip Skin
        }
    }
}
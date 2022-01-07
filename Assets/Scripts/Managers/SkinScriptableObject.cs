using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SkinType
{
    Head = 0,
    Body = 1,
    Legs = 2,
    Shoes = 3
}

[CreateAssetMenu(fileName = "Skin", menuName = "ScriptableObjects/Create Skin", order = 1)]
public class SkinScriptableObject : ScriptableObject
{
    public string Name;
    public int ID;
    public SkinType Type;
    public Mesh Mesh;
    public Material[] Materials;
    public Sprite SkinSprite;
    public int Cost;

    public void UpdateCounter()
    {
        ID = PlayerPrefs.GetInt("SkinID") + 1;
        PlayerPrefs.SetInt("SkinID", ID);
    }
}

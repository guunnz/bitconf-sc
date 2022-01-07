using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CharacterScriptableObject : ScriptableObject
{
    public Characters Character;

    public MeshFilter Cabeza;
    public MeshFilter Piernas;
    public MeshFilter Torso;
    public MeshFilter Zapatillas;
    public Material material;
}
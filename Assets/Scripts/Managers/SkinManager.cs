using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BodyPart
{
    public MeshRenderer Renderer;
    public SpriteRenderer SpriteRenderer;
    public Sprite SkinSprite;

}

public class SkinManager : MonoBehaviour
{
    public List<SkinScriptableObject> Skins;
    public List<BodyPart> BodyParts;
    public SkinType Type;


    public void EquipSkin(int SkinId)
    {
        SkinScriptableObject skin = Skins.Single(x => x.ID == SkinId);


    }
}
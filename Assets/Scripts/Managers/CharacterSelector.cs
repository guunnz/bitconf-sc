using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterSelector : MonoBehaviour
{
    public List<Character> CharacterList;

    public Characters CharacterSelected;
    public void CharacterSelect(int Character)
    {
        PlayerPrefs.SetInt(PlayerPrefValues.Character, Character);

        CharacterSelected = (Characters)Character;
        CharacterList.ForEach(x => x.ObjectsToTurnOn.ForEach(y => y.SetActive(false)));
        Character character = CharacterList.FirstOrDefault(x => x.character == CharacterSelected);
        character.ObjectsToTurnOn.ForEach(x => x.SetActive(true));
    }
}

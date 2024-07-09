using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Player Persistent Data", menuName = "New Player Persistent Data")]
public class PlayerPersistentData : ScriptableObject
{
    public PlayerPersistentCharacter[] characters;

#if UNITY_EDITOR

    void OnValidate ()
    {
        ResetCharacters();
    }

#endif

    public void ResetCharacters ()
    {
        for(int i = 0; i < characters.Length; i++)
        {
            // original lines
            //characters[i].health = characters[i].characterPrefab.GetComponent<Character>().maxHp;
            //characters[i].isDead = false;

            // refactored lines to prevent MissingReferenceException error on opening project
            if (characters[i].characterPrefab.TryGetComponent<Character>(out var character))
            {
                characters[i].health = character.maxHp;
                characters[i].isDead = false;
            }
        }
    }
}
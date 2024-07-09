using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public Character[] playerTeam;
    public Character[] enemyTeam;

    private List<Character> allCharacters = new List<Character>();

    [Header("Components")]
    public Transform[] playerTeamSpawns;
    public Transform[] enemyTeamSpawns;

    [Header("Data")]
    public PlayerPersistentData playerPersistentData;
    public CharacterSet defaultEnemySet;

    public static GameManager instance;
    public static CharacterSet curEnemySet;

    void OnEnable ()
    {
        Character.onCharacterDeath += OnCharacterKilled;
    }

    void OnDisable ()
    {
        Character.onCharacterDeath -= OnCharacterKilled;
    }

    void Awake ()
    {
        //if(instance == null || instance != this)
        //{
        //    Destroy(gameObject);
        //}
        //else
        //{
            instance = this;
        //}
    }

    void Start ()
    {
        if(curEnemySet == null)
            CreateCharacters(playerPersistentData, defaultEnemySet);
        else
            CreateCharacters(playerPersistentData, curEnemySet);

        TurnManager.instance.Begin();
    }

    // Called at the start of the game - create the character game objects.
    void CreateCharacters (PlayerPersistentData playerData, CharacterSet enemyTeamSet)
    {
        playerTeam = new Character[playerData.characters.Length];
        enemyTeam = new Character[enemyTeamSet.characters.Length];

        int playerSpawnIndex = 0;
        int currentCharacter = 0;
        // Player characters.
        for(int i = 0; i < playerData.characters.Length; i++)
        {
            if(!playerData.characters[i].isDead)
            {
                Character character = CreateCharacter(playerData.characters[i].characterPrefab, playerTeamSpawns[playerSpawnIndex]);
                character.curHp = playerData.characters[i].health;
                playerTeam[i] = character;
                character.aiCharacterName = GenerateCharacterName(currentCharacter);
                playerSpawnIndex++;
                currentCharacter++;
            }
            else
            {
                playerTeam[i] = null;
            }
        }

        // Enemy characters.
        for(int i = 0; i < enemyTeamSet.characters.Length; i++)
        {
            Character character = CreateCharacter(enemyTeamSet.characters[i], enemyTeamSpawns[i]);
            character.aiCharacterName = GenerateCharacterName(currentCharacter);
            currentCharacter++;
            enemyTeam[i] = character;
        }

        allCharacters.AddRange(playerTeam);
        allCharacters.AddRange(enemyTeam);
    }

    string GenerateCharacterName(int id)
    {
        string choosedName = string.Empty;
        switch (id)
        {
            case 0: choosedName = "CharacterRed"; break;
            case 1: choosedName = "CharacterGreen"; break;
            case 2: choosedName = "CharacterBlack"; break;
            case 3: choosedName = "CharacterWhite"; break;
        }

        return choosedName;
    }

    // Spawns in a character.
    Character CreateCharacter (GameObject characterPrefab, Transform spawnPos)
    {
        GameObject obj = Instantiate(characterPrefab, spawnPos.position, spawnPos.rotation);
        return obj.GetComponent<Character>();
    }

    // Called when a character has been killed.
    void OnCharacterKilled (Character character)
    {
        allCharacters.Remove(character);

        int playersRemaining = 0;
        int enemiesRemaining = 0;

        for(int i = 0; i < allCharacters.Count; i++)
        {
            if(allCharacters[i].team == Character.Team.Player)
                playersRemaining++;
            else
                enemiesRemaining++;
        }

        // Did the player team win?
        if(enemiesRemaining == 0)
        {
            PlayerTeamWins();
        }
        // Did the enemy team win?
        else if(playersRemaining == 0)
        {
            EnemyTeamWins();
        }
    }

    // Called when the player team wins.
    void PlayerTeamWins ()
    {
        UpdatePlayerPersistentData();
        Invoke(nameof(LoadMapScene), 0.5f);
    }

    // Called when the enemy team wins.
    void EnemyTeamWins ()
    {
        playerPersistentData.ResetCharacters();
        Invoke(nameof(LoadMapScene), 0.5f);
    }

    // Update player health and death status in their persistent data.
    void UpdatePlayerPersistentData ()
    {
        for(int i = 0; i < playerTeam.Length; i++)
        {
            if(playerTeam[i] != null)
            {
                playerPersistentData.characters[i].health = playerTeam[i].curHp;
            }
            else
            {
                playerPersistentData.characters[i].isDead = true;
            }
        }
    }

    // Go to the map scene.
    void LoadMapScene ()
    {
        SceneManager.LoadScene("Map");
    }
}